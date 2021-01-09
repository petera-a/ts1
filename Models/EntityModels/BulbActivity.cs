using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TS_WebApp.Models.EntityModels
{
    [Table("Activities")]
    public class BulbActivity
    {
        [Key]
        public int ActivityId { get; set; }
        public DateTime RegisteredOn { get; set; }
        public string DeviceId { get; set; }
        public int BulbId { get; set; }
        public virtual Device Device { get; set; }
        public bool LightupSignalSent { get; set; }
    }
}