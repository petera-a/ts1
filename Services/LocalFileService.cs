using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using TS_WebApp.Models.EntityModels;
using Newtonsoft.Json;

namespace TS_WebApp.Services
{
    public class LocalFileService
    {
        public List<BulbTelemetryDTO> GetLocalMockTelemetry()
        {
            string json = File.ReadAllText(@"C:\Projects\TS-WebApp\TS-WebApp\TestData\MockTelemetry.json");

            return JsonConvert.DeserializeObject<List<BulbTelemetryDTO>>(json);
        }
    }
}