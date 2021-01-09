using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using TS_WebApp.Models;
using TS_WebApp.Models.EntityModels;

namespace TS_WebApp.Services
{
    public static class DbService
    {
        public static List<Team> GetTeams()
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                return context.Teams?.ToList();
            }
        }

        public static List<Device> GetDevices()
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                return context.Devices?.ToList();
            }
        }
        public static List<BulbActivity> GetActivityByDeviceId(string deviceId)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                return context.BulbActivities.Where(x => x.Device.DeviceId.Equals(deviceId))?.ToList();
            }
        }

        public static List<BulbActivity> GetBulbActivities()
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                return context.BulbActivities.Include(x=>x.Device)?.ToList();
            }
        }

        // Remove activity from DB
        public static void DeleteActivity(BulbActivity activity)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                context.BulbActivities.Attach(activity);
                context.BulbActivities.Remove(activity);
                context.SaveChanges();
            }
        }

        // Update activity from DB
        public static void UpdateRegisteredOnColumn(BulbActivity activity)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                try
                {
                    var existingActivity = context.BulbActivities.Where(x=>x.DeviceId.Equals(activity.DeviceId)).FirstOrDefault();
                    if(existingActivity != null)
                    {
                        existingActivity.RegisteredOn = activity.RegisteredOn;
                        context.SaveChanges();
                    }
                  
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        // Insert activity from DB
        public static void InsertActivity(BulbActivity activity)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                context.BulbActivities.Add(activity);
                context.SaveChanges();
            }
        }

        public static Team GetTeamWithDevice(int teamId)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                return context.Teams.Where(x=>x.TeamId == teamId).Include(x=>x.Devices).FirstOrDefault();
            }
        }

        public static int GetTeamIdByBulbId(int bulbId)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                return context.Devices.Where(x => x.BulbId == bulbId).Select(x => x.TeamId).FirstOrDefault(); ;
            }
        }

        public static void SetLightupSignalSetForActivities(int teamId)
        {
            using (Context context = new Context(AppSettingsUtil.ConnectionString))
            {
                List<BulbActivity> activitiesToUpdate = context.BulbActivities.Where(x => x.Device.TeamId == teamId).ToList();

                foreach (var activity in activitiesToUpdate)
                {
                    activity.LightupSignalSent = true;

                    context.BulbActivities.Attach(activity);
                    context.Entry<BulbActivity>(activity).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
        }
    }
}