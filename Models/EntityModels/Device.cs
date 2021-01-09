using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TS_WebApp.Models.EntityModels
{
    public class Device
    {
        public string DeviceId { get; set; }
        public int BulbId { get; set; }
        public int TeamId { get; set; }
    }
}