using Microsoft.Owin;
using Owin;
using Quartz;
using Quartz.Impl;
using System;
using System.Configuration;
using System.Threading.Tasks;
using TS_WebApp.Jobs;

[assembly: OwinStartup(typeof(TS_WebApp.OwinStartup))]

namespace TS_WebApp
{
    public class OwinStartup
    {
        public async void Configuration(IAppBuilder app)
        {
            var scheduledJobIntervalInSeconds = AppSettingsUtil.StatusUpdateJobIntervalInSeconds;
            app.MapSignalR();

            // construct a scheduler factory
            StdSchedulerFactory factory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = await factory.GetScheduler();

            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<SendClientUpdateJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(scheduledJobIntervalInSeconds)
                    .RepeatForever())
            .Build();

            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
