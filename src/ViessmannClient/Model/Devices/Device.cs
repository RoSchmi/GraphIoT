﻿using System;

namespace PhilipDaubmeier.ViessmannClient.Model.Devices
{
    public class Device
    {
        public long LongId => long.TryParse(Id, out long id) ? id : 0;
        public string? GatewaySerial { get; set; }
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public string? Fingerprint { get; set; }
        public string? BoilerSerial { get; set; }
        public string? BoilerSerialEditor { get; set; }
        public string? BmuSerial { get; set; }
        public string? BmuSerialEditor { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public string? ModelId { get; set; }
        public string? ModelVersion { get; set; }
        public string? DeviceType { get; set; }
    }
}