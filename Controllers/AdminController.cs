using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TS_WebApp.Models;
using TS_WebApp.Models.EntityModels;
using TS_WebApp.Models.ViewModels;
using TS_WebApp.Services;

namespace TS_WebApp.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            AdminViewModel model = new AdminViewModel()
            {
                Teams = DbService.GetTeams(),
                Devices = DbService.GetDevices()
            };

            //var latestBulbsActivity = await TestBlobStuff();

            return View(model);
        }

        public async Task<List<BulbTelemetryDTO>> TestBlobStuff()
        {
            TsBlobService blobService = new TsBlobService();

            return await blobService.GetBulbActivitiesFromBlobStorage();
        }

        public void CreateTeam(Team teamToCreate)
        {
            // Creation of teams
        }
    }
}