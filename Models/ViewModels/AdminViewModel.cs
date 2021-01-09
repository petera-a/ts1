using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TS_WebApp.Models.EntityModels;

namespace TS_WebApp.Models.ViewModels
{
    public class AdminViewModel
    {
        public List<Team> Teams { get; set; }
        public List<Device> Devices { get; set; }
    }
}