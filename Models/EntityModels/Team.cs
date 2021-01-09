using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TS_WebApp.Models.EntityModels;

namespace TS_WebApp.Models
{
    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }

        public virtual List<Device> Devices { get; set; }
    }
}