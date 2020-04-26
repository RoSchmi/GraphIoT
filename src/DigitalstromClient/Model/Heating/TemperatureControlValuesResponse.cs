﻿using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System.Collections.Generic;

namespace PhilipDaubmeier.DigitalstromClient.Model.Heating
{
    public class HeatingValuesZone
    {
        public Zone Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public Dsuid ControlDSUID { get; set; } = string.Empty;
        public bool IsConfigured { get; set; }
        public double? Off { get; set; }
        public double? Comfort { get; set; }
        public double? Economy { get; set; }
        public double? NotUsed { get; set; }
        public double? Night { get; set; }
        public double? Holiday { get; set; }
        public double? Cooling { get; set; }
        public double? CoolingOff { get; set; }
    }

    public class TemperatureControlValuesResponse : IWiremessagePayload
    {
        public List<HeatingValuesZone> Zones { get; set; } = new List<HeatingValuesZone>();
    }
}