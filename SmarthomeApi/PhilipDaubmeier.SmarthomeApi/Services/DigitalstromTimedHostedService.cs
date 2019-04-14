﻿using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.DigitalstromClient.Model;
using PhilipDaubmeier.DigitalstromClient.Model.Core;
using PhilipDaubmeier.DigitalstromClient.Network;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhilipDaubmeier.SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Services
{
    public class DigitalstromTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly PersistenceContext _dbContext;
        private readonly DigitalstromWebserviceClient _dsClient;
        private Timer _timer;

        public DigitalstromTimedHostedService(ILogger<DigitalstromTimedHostedService> logger, PersistenceContext databaseContext, IDigitalstromConnectionProvider connectionProvider)
        {
            _logger = logger;
            _dbContext = databaseContext;
            _dsClient = new DigitalstromWebserviceClient(connectionProvider);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is starting.");

            _timer = new Timer(PollAll, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
        
        private async void PollAll(object state)
        {
            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is polling new sensor values...");
            
            try
            {
                await PollSensorValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Digitalstrom sensor background worker: {ex.Message}");
            }

            _logger.LogInformation($"{DateTime.Now} Digitalstrom Background Service is polling new energy values...");

            try
            {
                await PollEnergyValues();
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{DateTime.Now} Exception occurred in Digitalstrom energy background worker: {ex.Message}");
            }
        }

        private async Task PollSensorValues()
        {
            var sensorValues = (await _dsClient.GetZonesAndSensorValues()).zones;

            _dbContext.Semaphore.WaitOne();
            try
            {
                foreach (var zone in sensorValues)
                    if (zone != null && zone.sensor != null)
                        SaveZoneSensorValues(zone.ZoneID, zone.sensor.ToDictionary(x => x.type, x => x.value));

                _dbContext.SaveChanges();
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        private async Task PollEnergyValues()
        {
            var dsuids = (await _dsClient.GetMeteringCircuits()).FilteredMeterNames.Select(x => x.Key).ToList();
            
            var fetchLastValues = (int)TimeSeriesSpan.Spacing.Spacing10Min;
            var days = new TimeSeriesSpan(DateTime.Now.AddSeconds(-1 * fetchLastValues), TimeSeriesSpan.Spacing.Spacing1Sec, fetchLastValues).IncludedDates();

            Dictionary<DateTime, TimeSeriesStreamCollection<DSUID, int>> timeseriesCollections = null;
            try
            {
                timeseriesCollections = ReadEnergyValuesFromDb(days, dsuids);

                foreach (var dsuid in dsuids)
                    foreach (var timestampedValue in (await _dsClient.GetEnergy(dsuid, (int)TimeSeriesSpan.Spacing.Spacing1Sec, fetchLastValues)).TimeSeries)
                        foreach (var timeseries in timeseriesCollections.Select(x => x.Value[dsuid]))
                            timeseries[timestampedValue.Key.ToLocalTime()] = (int)timestampedValue.Value;

                SaveEnergyValuesToDb(timeseriesCollections);
            }
            catch { throw; }
            finally
            {
                if (timeseriesCollections != null)
                    foreach (var collection in timeseriesCollections)
                        collection.Value.Dispose();
            }
        }

        private Dictionary<DateTime, TimeSeriesStreamCollection<DSUID, int>> ReadEnergyValuesFromDb(IEnumerable<DateTime> days, List<DSUID> dsuids)
        {
            var timeseriesCollections = new Dictionary<DateTime, TimeSeriesStreamCollection<DSUID, int>>();

            _dbContext.Semaphore.WaitOne();
            try
            {
                foreach (var day in days)
                {
                    var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Day == day).FirstOrDefault();
                    if (dbEnergySeries == null)
                        timeseriesCollections.Add(day, DigitalstromEnergyHighresData.InitialEnergySeriesEveryMeter(day, dsuids));
                    else
                        timeseriesCollections.Add(day, dbEnergySeries.EnergySeriesEveryMeter);
                }
                return timeseriesCollections;
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        private void SaveEnergyValuesToDb(Dictionary<DateTime, TimeSeriesStreamCollection<DSUID, int>> timeseriesCollections)
        {
            _dbContext.Semaphore.WaitOne();
            try
            {
                foreach (var collection in timeseriesCollections)
                {
                    var dbEnergySeries = _dbContext.DsEnergyHighresDataSet.Where(x => x.Day == collection.Key).FirstOrDefault();
                    if (dbEnergySeries == null)
                        _dbContext.DsEnergyHighresDataSet.Add(dbEnergySeries = new DigitalstromEnergyHighresData() { Day = collection.Key });

                    dbEnergySeries.EnergySeriesEveryMeter = collection.Value;
                }

                _dbContext.SaveChanges();
            }
            catch { throw; }
            finally
            {
                _dbContext.Semaphore.Release();
            }
        }

        private void SaveZoneSensorValues(int zoneId, Dictionary<int, double> sensorValues)
        {
            int temperatureType = 9;
            int humidityType = 13;

            var time = DateTime.Now;
            var day = time.Date;
            var dbSensorSeries = _dbContext.DsSensorDataSet.Where(x => x.ZoneId == zoneId && x.Day == day).FirstOrDefault();
            if (dbSensorSeries == null)
            {
                var dbZone = _dbContext.DsZones.Where(x => x.Id == zoneId).FirstOrDefault();
                if (dbZone == null)
                    _dbContext.DsZones.Add(dbZone = new DigitalstromZone() { Id = zoneId });
                
                _dbContext.DsSensorDataSet.Add(dbSensorSeries = new DigitalstromZoneSensorData() { ZoneId = zoneId, Zone = dbZone, Day = day });
            }

            if (sensorValues.ContainsKey(temperatureType))
            {
                var series = dbSensorSeries.TemperatureSeries;
                series[time] = sensorValues[temperatureType];
                dbSensorSeries.TemperatureSeries = series;
            }

            if (sensorValues.ContainsKey(humidityType))
            {
                var series = dbSensorSeries.HumiditySeries;
                series[time] = sensorValues[humidityType];
                dbSensorSeries.HumiditySeries = series;
            }
        }
    }
}
