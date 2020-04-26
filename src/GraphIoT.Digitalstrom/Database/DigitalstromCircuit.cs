﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PhilipDaubmeier.GraphIoT.Digitalstrom.Database
{
    public class DigitalstromCircuit
    {
        [Key, MaxLength(34), DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Dsuid { get; set; } = null!;
    }
}