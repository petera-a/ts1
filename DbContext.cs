using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TS_WebApp.Models;
using TS_WebApp.Models.EntityModels;

namespace TS_WebApp
{
    class Context : DbContext
    {
        public Context(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<Context>(null);
        }
       
        public DbSet<Team> Teams { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<BulbActivity> BulbActivities { get; set; }

    }
}