﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureList : List<Feature>
    {
        public string GetHeatingBoilerSerial() => GetFeature(FeatureName.Name.HeatingBoilerSerial)?.Value?.Value as string ?? string.Empty;
        public bool IsHeatingBoilerSensorsTemperatureCommonSupplyConnected() => GetFeature(FeatureName.Name.HeatingBoilerSensorsTemperatureCommonSupply)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingBoilerSensorsTemperatureCommonSupply() => GetFeature(FeatureName.Name.HeatingBoilerSensorsTemperatureCommonSupply)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingBoilerSensorsTemperatureMainConnected() => GetFeature(FeatureName.Name.HeatingBoilerSensorsTemperatureMain)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingBoilerSensorsTemperatureMain() => GetFeature(FeatureName.Name.HeatingBoilerSensorsTemperatureMain)?.ValueAsDouble ?? double.NaN;
        public double GetHeatingBoilerTemperature() => GetFeature(FeatureName.Name.HeatingBoilerTemperature)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingBurnerActive() => GetFeature(FeatureName.Name.HeatingBurner)?.Active?.Value ?? false;
        public bool IsGetHeatingBurnerAutomaticStatusOk() => GetFeature(FeatureName.Name.HeatingBurnerAutomatic)?.Status?.Value?.Equals("ok", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public int GetHeatingBurnerAutomaticErrorCode() => GetFeature(FeatureName.Name.HeatingBurnerAutomatic)?.ErrorCode?.Value ?? 0;
        public int GetHeatingBurnerModulation() => GetFeature(FeatureName.Name.HeatingBurnerModulation)?.ValueAsInt ?? 0;
        public decimal GetHeatingBurnerStatisticsHours() => GetFeature(FeatureName.Name.HeatingBurnerStatistics)?.Hours?.Value ?? 0m;
        public long GetHeatingBurnerStatisticsStarts() => GetFeature(FeatureName.Name.HeatingBurnerStatistics)?.Starts?.Value ?? 0;
        public IEnumerable<FeatureName.Circuit> GetHeatingCircuits() => GetFeature(FeatureName.Name.HeatingCircuits)?.Enabled?.Value?.Select(c => int.TryParse(c, out int number) ? (FeatureName.Circuit)number : FeatureName.Circuit.Circuit0).Distinct().ToList() ?? new List<FeatureName.Circuit>();
        public bool IsHeatingCircuitsCircuitActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsCircuit, circuit)?.Active?.Value ?? false;
        public string GetHeatingCircuitsCircuitName(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsCircuit, circuit)?.Name?.Value ?? string.Empty;
        public bool IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsCirculationPump, circuit)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public bool IsHeatingCircuitsFrostprotectionOn(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsFrostprotection, circuit)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public decimal GetHeatingCircuitsHeatingCurveShift(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsHeatingCurve, circuit)?.Shift?.Value ?? 0m;
        public decimal GetHeatingCircuitsHeatingCurveSlope(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsHeatingCurve, circuit)?.Slope?.Value ?? 0m;
        public bool IsHeatingCircuitsHeatingScheduleActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsHeatingSchedule, circuit)?.Active?.Value ?? false;
        public Schedule GetHeatingCircuitsHeatingSchedule(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsHeatingSchedule, circuit)?.Entries?.Value ?? new Schedule();
        public string GetHeatingCircuitsOperatingModesActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingModesActive, circuit)?.Value?.Value?.ToString() ?? string.Empty;
        public bool IsHeatingCircuitsOperatingModesDhwActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingModesDhw, circuit)?.Active?.Value ?? false;
        public bool IsHeatingCircuitsOperatingModesDhwAndHeatingActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingModesDhwAndHeating, circuit)?.Active?.Value ?? false;
        public bool IsHeatingCircuitsOperatingModesForcedNormalActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingModesForcedNormal, circuit)?.Active?.Value ?? false;
        public bool IsHeatingCircuitsOperatingModesForcedReducedActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingModesForcedReduced, circuit)?.Active?.Value ?? false;
        public bool IsHeatingCircuitsOperatingModesStandbyActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingModesStandby, circuit)?.Active?.Value ?? false;
        public string GetHeatingCircuitsOperatingProgramsActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsActive, circuit)?.Value?.Value?.ToString() ?? string.Empty;
        public bool IsHeatingCircuitsOperatingProgramsComfortActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsComfort, circuit)?.Active?.Value ?? false;
        public decimal GetHeatingCircuitsOperatingProgramsComfortTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsComfort, circuit)?.Temperature?.Value ?? 0m;
        public bool IsHeatingCircuitsOperatingProgramsEcoActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsEco, circuit)?.Active?.Value ?? false;
        public decimal GetHeatingCircuitsOperatingProgramsEcoTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsEco, circuit)?.Temperature?.Value ?? 0m;
        public bool IsHeatingCircuitsOperatingProgramsExternalActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsExternal, circuit)?.Active?.Value ?? false;
        public decimal GetHeatingCircuitsOperatingProgramsExternalTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsExternal, circuit)?.Temperature?.Value ?? 0m;
        public bool IsHeatingCircuitsOperatingProgramsHolidayActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsHoliday, circuit)?.Active?.Value ?? false;
        public string GetHeatingCircuitsOperatingProgramsHolidayStart(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsHoliday, circuit)?.Start?.Value ?? string.Empty;
        public string GetHeatingCircuitsOperatingProgramsHolidayEnd(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsHoliday, circuit)?.End?.Value ?? string.Empty;
        public bool IsHeatingCircuitsOperatingProgramsNormalActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsNormal, circuit)?.Active?.Value ?? false;
        public decimal GetHeatingCircuitsOperatingProgramsNormalTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsNormal, circuit)?.Temperature?.Value ?? 0m;
        public bool IsHeatingCircuitsOperatingProgramsReducedActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsReduced, circuit)?.Active?.Value ?? false;
        public decimal GetHeatingCircuitsOperatingProgramsReducedTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsReduced, circuit)?.Temperature?.Value ?? 0m;
        public bool IsHeatingCircuitsOperatingProgramsStandbyActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsOperatingProgramsStandby, circuit)?.Active?.Value ?? false;
        public bool IsHeatingCircuitsSensorsTemperatureRoomConnected(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsSensorsTemperatureRoom, circuit)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingCircuitsSensorsTemperatureRoom(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsSensorsTemperatureRoom, circuit)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingCircuitsSensorsTemperatureSupplyConnected(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsSensorsTemperatureSupply, circuit)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingCircuitsSensorsTemperatureSupply(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsSensorsTemperatureSupply, circuit)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingCircuitsGeofencingActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsGeofencing, circuit)?.Active?.Value ?? false;
        public string GetHeatingCircuitsGeofencingStatus(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0) => GetFeature(FeatureName.Name.HeatingCircuitsGeofencing, circuit)?.Status?.Value ?? string.Empty;
        public bool IsHeatingConfigurationMultiFamilyHouseActive() => GetFeature(FeatureName.Name.HeatingConfigurationMultiFamilyHouse)?.Active?.Value ?? false;
        public string GetHeatingControllerSerial() => GetFeature(FeatureName.Name.HeatingControllerSerial)?.Value?.Value?.ToString() ?? string.Empty;
        public int GetHeatingDeviceTimeOffset() => GetFeature(FeatureName.Name.HeatingDeviceTimeOffset)?.ValueAsInt ?? 0;
        public bool IsHeatingDhwActive() => GetFeature(FeatureName.Name.HeatingDhw)?.Active?.Value ?? false;
        public bool IsHeatingDhwChargingActive() => GetFeature(FeatureName.Name.HeatingDhwCharging)?.Active?.Value ?? false;
        public bool IsHeatingDhwPumpsCirculationOn() => GetFeature(FeatureName.Name.HeatingDhwPumpsCirculation)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public bool IsHeatingDhwPumpsCirculationScheduleActive() => GetFeature(FeatureName.Name.HeatingDhwPumpsCirculationSchedule)?.Active?.Value ?? false;
        public Schedule GetHeatingDhwPumpsCirculationSchedule() => GetFeature(FeatureName.Name.HeatingDhwPumpsCirculationSchedule)?.Entries?.Value ?? new Schedule();
        public bool IsHeatingDhwPumpsPrimaryOn() => GetFeature(FeatureName.Name.HeatingDhwPumpsPrimary)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public bool IsHeatingDhwScheduleActive() => GetFeature(FeatureName.Name.HeatingDhwSchedule)?.Active?.Value ?? false;
        public Schedule GetHeatingDhwSchedule() => GetFeature(FeatureName.Name.HeatingDhwSchedule)?.Entries?.Value ?? new Schedule();
        public bool IsHeatingDhwSensorsTemperatureHotWaterStorageConnected() => GetFeature(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorage)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingDhwSensorsTemperatureHotWaterStorage() => GetFeature(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorage)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingDhwSensorsTemperatureOutletConnected() => GetFeature(FeatureName.Name.HeatingDhwSensorsTemperatureOutlet)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingDhwSensorsTemperatureOutlet() => GetFeature(FeatureName.Name.HeatingDhwSensorsTemperatureOutlet)?.ValueAsDouble ?? double.NaN;
        public double GetHeatingDhwTemperature() => GetFeature(FeatureName.Name.HeatingDhwTemperature)?.ValueAsDouble ?? double.NaN;
        public double GetHeatingDhwTemperatureMain() => GetFeature(FeatureName.Name.HeatingDhwTemperatureMain)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingSensorsTemperatureOutsideConnected() => GetFeature(FeatureName.Name.HeatingSensorsTemperatureOutside)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingSensorsTemperatureOutside() => GetFeature(FeatureName.Name.HeatingSensorsTemperatureOutside)?.ValueAsDouble ?? double.NaN;
        public bool GetHeatingServiceDue() => GetFeature(FeatureName.Name.HeatingServiceTimeBased)?.ServiceDue?.Value ?? false;
        public int GetHeatingServiceIntervalMonths() => GetFeature(FeatureName.Name.HeatingServiceTimeBased)?.ServiceIntervalMonths?.Value ?? 0;
        public int GetHeatingActiveMonthSinceLastService() => GetFeature(FeatureName.Name.HeatingServiceTimeBased)?.ActiveMonthSinceLastService?.Value ?? 0;
        public string GetHeatingLastService() => GetFeature(FeatureName.Name.HeatingServiceTimeBased)?.LastService?.Value ?? string.Empty;
        public bool IsHeatingSolarActive() => GetFeature(FeatureName.Name.HeatingSolar)?.Active?.Value ?? false;
        public IEnumerable<double> GetHeatingSolarPowerProductionDay() => GetFeature(FeatureName.Name.HeatingSolarPowerProduction)?.Day?.Value ?? new List<double>();
        public IEnumerable<double> GetHeatingSolarPowerProductionWeek() => GetFeature(FeatureName.Name.HeatingSolarPowerProduction)?.Week?.Value ?? new List<double>();
        public IEnumerable<double> GetHeatingSolarPowerProductionMonth() => GetFeature(FeatureName.Name.HeatingSolarPowerProduction)?.Month?.Value ?? new List<double>();
        public IEnumerable<double> GetHeatingSolarPowerProductionYear() => GetFeature(FeatureName.Name.HeatingSolarPowerProduction)?.Year?.Value ?? new List<double>();
        public string GetHeatingSolarPowerProductionUnit() => GetFeature(FeatureName.Name.HeatingSolarPowerProduction)?.Unit?.Value ?? string.Empty;
        public bool IsHeatingSolarPumpsCircuitOn() => GetFeature(FeatureName.Name.HeatingSolarPumpsCircuit)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public decimal GetHeatingSolarStatisticsHours() => GetFeature(FeatureName.Name.HeatingSolarStatistics)?.Hours?.Value ?? 0m;
        public bool IsHeatingSolarSensorsTemperatureDhwConnected() => GetFeature(FeatureName.Name.HeatingSolarSensorsTemperatureDhw)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingSolarSensorsTemperatureDhw() => GetFeature(FeatureName.Name.HeatingSolarSensorsTemperatureDhw)?.ValueAsDouble ?? double.NaN;
        public bool IsHeatingSolarSensorsTemperatureCollectorConnected() => GetFeature(FeatureName.Name.HeatingSolarSensorsTemperatureCollector)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        public double GetHeatingSolarSensorsTemperatureCollector() => GetFeature(FeatureName.Name.HeatingSolarSensorsTemperatureCollector)?.ValueAsDouble ?? double.NaN;
        public double GetHeatingSolarPowerCumulativeProduced() => GetFeature(FeatureName.Name.HeatingSolarPowerCumulativeProduced)?.ValueAsDouble ?? 0d;
        public bool IsHeatingSolarRechargeSuppressionOn() => GetFeature(FeatureName.Name.HeatingSolarRechargeSuppression)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;

        public PropertyList? GetFeature(FeatureName.Name name, FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetFirstFeature(name, circuit)?.Properties;
        }

        public DateTime? GetTimestamp(FeatureName.Name name, FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetFirstFeature(name, circuit)?.Timestamp;
        }

        private Feature GetFirstFeature(FeatureName.Name name, FeatureName.Circuit? circuit)
        {
            var nameStr = (string)new FeatureName(name, circuit);
            return this.Where(x => x.Name?.Equals(nameStr, StringComparison.InvariantCultureIgnoreCase) ?? false).FirstOrDefault();
        }
    }
}