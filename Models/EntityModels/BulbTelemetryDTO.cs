using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TS_WebApp.Models.EntityModels
{
    public class BulbTelemetryDTO
    {
        public string DeviceId { get; set; }
        public TelemetryDTO Telemetry { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}