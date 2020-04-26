﻿using PhilipDaubmeier.DigitalstromClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PhilipDaubmeier.DigitalstromClient.Model.Energy
{
    public class EnergyMeteringResponse : IWiremessagePayload
    {
        public string Type { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;

        [JsonConverter(typeof(StringToIntConverter))]
        public int Resolution { get; set; }

        public List<List<double>> Values { get; set; } = new List<List<double>>();

        public IEnumerable<KeyValuePair<DateTime, double>> TimeSeries => Values.Select(x =>
                new KeyValuePair<DateTime, double>(DateTimeOffset.FromUnixTimeSeconds((long)x.FirstOrDefault()).UtcDateTime, x.LastOrDefault()));
    }
}