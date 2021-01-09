using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TS_WebApp.Models.EntityModels
{
    public class TelemetryDTO
    {
        [JsonProperty("Bright Bulb ID")]
        public int BrightBulbId { get; set; }
    }
}