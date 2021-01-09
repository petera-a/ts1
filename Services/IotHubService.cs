using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;

namespace TS_WebApp.Services
{
    public class IotHubService
    {
        public static bool sendLightUpCommandToAllDevicesInTeam(int teamId, List<int> skipTheseBulbIds, bool isNewActivityPresent)
        {
            try
            {
                var team = DbService.GetTeamWithDevice(teamId);

                if (team != null && team.Devices.Any())
                {
                    foreach (var device in team.Devices)
                    {
                        //var activity = DbService.GetActivityByDeviceId(device.DeviceId).FirstOrDefault();

                        // If no activity means it's a notification that's needed for an inactive bulb
                        // If lightupSignalSent - don't light up the bulbs with every scheduled job. Only when there is a new activity.
                        // skipTheseBulbIds is an array of the newly arrived activities. Do not ligth them up, they just got turned on by users
                        if(isNewActivityPresent)
                        {
                            if(!skipTheseBulbIds.Contains(device.BulbId))
                            {
                                var url = "https://ts1.azureiotcentral.com/api/preview/devices/" + device.DeviceId + "/commands/lightUp";

                                string json = "{\"request\":{\"Bright Bulb Id\":" + device.BulbId + "}}";
                                var data = new StringContent(json, Encoding.UTF8, "application/json");

                                HttpRequestMessage request = new HttpRequestMessage();
                                request.Headers.Add("Authorization", "SharedAccessSignature sr=0d405341-5d7a-4d09-b97c-fc4f722d924e&sig=Rjve7yNogh0AQ1Qm1qqMgGLpcpRgOKEosGQvlIooGqY%3D&skn=TSToken&se=1640953771389");
                                request.Content = data;
                                request.Method = HttpMethod.Post;
                                request.RequestUri = new System.Uri(url);

                                if(!AppSettingsUtil.DebugUsingLocalTelemetryData)
                                {
                                    var client = new HttpClient();
                                    var response = client.SendAsync(request);
                                }
                            }
                            
                        }
                       
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}