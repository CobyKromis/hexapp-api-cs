using System;
using System.Collections.Generic;

#nullable disable

namespace hexapp_api_cs.Models
{
    public partial class HealthMetric
    {
        public int HealthMetricId { get; set; }
        public int? UserId { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? Calories { get; set; }
        public int? WaterOz { get; set; }
    }
}
