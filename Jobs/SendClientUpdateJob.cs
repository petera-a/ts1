using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using TS_WebApp.Models.EntityModels;
using TS_WebApp.Services;
using TS_WebApp.SignalrHubs;

namespace TS_WebApp.Jobs
{
    public class SendClientUpdateJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            // Used to determine if we should notify all other bulbs, besides the latest that was received
            bool isNewActivityPresent = false;

            List<int> bulbIdsToSkipWhenNotifying = new List<int>();
            List<BulbTelemetryDTO> latestTelemetryActivity = new List<BulbTelemetryDTO>();
            List<int> teamsToNotify = new List<int>();


            if (AppSettingsUtil.DebugUsingLocalTelemetryData)
            {
                LocalFileService localFileService = new LocalFileService();
                latestTelemetryActivity = localFileService.GetLocalMockTelemetry();
            }
            else
            {
                // Received new telemetry activity.
                latestTelemetryActivity = GetLatestTelemetryActivity();
            }

            DeleteExpiredTelemetry(latestTelemetryActivity);

            foreach (var teleActivity in latestTelemetryActivity)
            {
                var newActivityFromTelemetry = new BulbActivity
                {
                    BulbId = teleActivity.Telemetry.BrightBulbId,
                    DeviceId = teleActivity.DeviceId,
                    RegisteredOn = teleActivity.CreatedOn
                };

                AddTeamToNotifyToList(teamsToNotify, newActivityFromTelemetry);
            }

            var latestDBActivities = DbService.GetBulbActivities();

            isNewActivityPresent = AddNewActivitiesToDb(isNewActivityPresent, bulbIdsToSkipWhenNotifying, latestTelemetryActivity, latestDBActivities);

            var signalrContext = GlobalHost.ConnectionManager.GetHubContext<StatusUpdateHub>();

            var expiredActivitites = DeleteExpiredActivities(latestDBActivities);

            if (expiredActivitites != null)
            {
                await signalrContext.Clients.All.receiveDataUpdate(
                               JsonConvert.SerializeObject(latestDBActivities),
                               JsonConvert.SerializeObject(expiredActivitites)
                           );
            }
            else
            {
                await signalrContext.Clients.All.receiveDataUpdate(
                              JsonConvert.SerializeObject(latestDBActivities),
                              JsonConvert.SerializeObject(null)
                          );
            }
            foreach (var teamId in teamsToNotify)
            {
                IotHubService.sendLightUpCommandToAllDevicesInTeam(teamId, bulbIdsToSkipWhenNotifying, isNewActivityPresent);
            }
            return;
        }

        private static bool AddNewActivitiesToDb(bool isNewActivityPresent, List<int> bulbIdsToSkipWhenNotifying, List<BulbTelemetryDTO> latestTelemetryActivity, List<BulbActivity> latestDBActivities)
        {
            foreach (var item in latestTelemetryActivity)
            {
                if (!latestDBActivities.Any(x => x.BulbId == item.Telemetry.BrightBulbId))
                {
                    // These flags help determine which bulbs to light up as notification.
                    isNewActivityPresent = true;
                    bulbIdsToSkipWhenNotifying.Add(item.Telemetry.BrightBulbId);

                    var newActivityFromTelemetry = new BulbActivity
                    {
                        BulbId = item.Telemetry.BrightBulbId,
                        DeviceId = item.DeviceId,
                        RegisteredOn = item.CreatedOn
                    };

                    // Add to both DB and local collection so we don't have to re-retreive all activities
                    DbService.InsertActivity(newActivityFromTelemetry);
                    latestDBActivities.Add(newActivityFromTelemetry);

                }
                else if (latestDBActivities.First(x => x.BulbId == item.Telemetry.BrightBulbId).RegisteredOn < item.CreatedOn)
                {
                    // These flags help determine which bulbs to light up as notification.
                    isNewActivityPresent = true;
                    bulbIdsToSkipWhenNotifying.Add(item.Telemetry.BrightBulbId);

                    var activityFromTelemetryToUpdate = new BulbActivity
                    {
                        BulbId = item.Telemetry.BrightBulbId,
                        DeviceId = item.DeviceId,
                        RegisteredOn = item.CreatedOn
                    };

                    // Add to both DB and local collection so we don't have to re-retreive all activities
                    DbService.UpdateRegisteredOnColumn(activityFromTelemetryToUpdate);
                    latestDBActivities.Add(activityFromTelemetryToUpdate);
                }
            }

            return isNewActivityPresent;
        }

        private static void AddTeamToNotifyToList(List<int> teamsToNotify, BulbActivity newActivityFromTelemetry)
        {
            var teamIdForDevice = DbService.GetTeamIdByBulbId(newActivityFromTelemetry.BulbId);
            var activityForDevice = DbService.GetActivityByDeviceId(newActivityFromTelemetry.DeviceId);


            if (!teamsToNotify.Contains(teamIdForDevice))
            {
                teamsToNotify.Add(teamIdForDevice);
            }
        }

        private static List<BulbTelemetryDTO> GetLatestTelemetryActivity()
        {
            TsBlobService tsBlobService = new TsBlobService();
            var last10BlobTelemetryRecords = tsBlobService.GetBulbActivitiesFromBlobStorage().Result;
            var groupedBulbs = last10BlobTelemetryRecords.GroupBy(x => x.Telemetry.BrightBulbId);

            List<BulbTelemetryDTO> latestBulbActivities = new List<BulbTelemetryDTO>();

            foreach (var bulbGroup in groupedBulbs)
            {
                var firstActivity = bulbGroup.FirstOrDefault();

                if (firstActivity != null)
                    latestBulbActivities.Add(firstActivity);
            }

            return latestBulbActivities;
        }

        private List<BulbActivity> DeleteExpiredActivities(List<BulbActivity> latestDbActivities)
        {
            var expiredActivities = latestDbActivities.Where(x => x.RegisteredOn.AddSeconds(AppSettingsUtil.BulbActiveDurationInSeconds) < DateTime.Now).ToList();

            if (expiredActivities.Any())
            {
                // check expired activities and remove them from the DB so we can turn off the lights in the clients.
                foreach (var activity in expiredActivities)
                {
                    latestDbActivities.Remove(activity);
                    DbService.DeleteActivity(activity);
                }

                return expiredActivities;
            }

            return null;
        }

        private List<BulbTelemetryDTO> DeleteExpiredTelemetry(List<BulbTelemetryDTO> latestTelemetryActivity)
        {
            var expiredActivities = latestTelemetryActivity.Where(x => x.CreatedOn.AddSeconds(AppSettingsUtil.BulbActiveDurationInSeconds) < DateTime.Now).ToList();

            if (expiredActivities.Any())
            {
                // check expired activities and remove them from the DB so we can turn off the lights in the clients.
                foreach (var activity in expiredActivities)
                {
                    latestTelemetryActivity.Remove(activity);
                }

                return expiredActivities;
            }

            return null;
        }
    }
}