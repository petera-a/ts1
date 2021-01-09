using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Http;
using TS_WebApp.Jobs;
using TS_WebApp.Services;

namespace TS_WebApp.Controllers
{
    public class TestApiController : ApiController
    {
        [HttpGet]
        [Route("api/test/lightUpTeamBulbs")]
        public void LightUpTeamBulbs(int teamId)
        {
            IotHubService.sendLightUpCommandToAllDevicesInTeam(3, null, true);
        }

        [HttpGet]
        [Route("api/test/fullJobOneTimeRun")]
        public async Task<bool> FullJobOneTimeRun()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = await factory.GetScheduler();
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<SendClientUpdateJob>()
                .WithIdentity("OneTimeJobRun", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("OneTimeTrigger", "group1")
                .StartNow()
            .Build();

            await scheduler.ScheduleJob(job, trigger);

            return true;
        }

        [HttpGet]
        [Route("api/test/mockTelemetry")]
        public bool MockTelemetry()
        {
            try
            {
                LocalFileService fileService = new LocalFileService();

                var localTelemetry = fileService.GetLocalMockTelemetry();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}