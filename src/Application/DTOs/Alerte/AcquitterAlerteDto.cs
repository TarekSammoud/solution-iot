using Application.DTOs.SeuilAlerte;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Alerte
{
    public class AcquitterAlerteDto
    {
        public SeuilAlerteDto SeuilAlerte { get; set; }
        public string? Message { get; set; }

    }
}
