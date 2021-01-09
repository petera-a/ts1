using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace TS_WebApp
{
    public static class AppSettingsUtil
    {
        public static string ConnectionString {
            get {
                return ConfigurationManager.AppSettings["databaseConnectionString"].ToString();
            }
        }

        public static long BulbActiveDurationInSeconds {
            get {
                return Convert.ToInt64(ConfigurationManager.AppSettings["bulbActiveDurationInSeconds"]);
            }
        }

        public static int StatusUpdateJobIntervalInSeconds {
            get {
                return Convert.ToInt32(ConfigurationManager.AppSettings["Quartz.StatusUpdateJob.IntervalInSeconds"]);

            }
        }
        
        public static bool DebugUsingLocalTelemetryData {
            get {
                bool debugUsingLocalTelemetrydata = false;
                bool.TryParse(ConfigurationManager.AppSettings["Debug.DebugUsingLocalTelemetryData"], out debugUsingLocalTelemetrydata);
                return debugUsingLocalTelemetrydata;
            }
        }
    }
}