using System;
using System.Collections.Generic;

#nullable disable

namespace hexapp_api_cs.Models
{
    public partial class HealthMetric
    {
        public int? HealthMetricId { get; set; }
        public int? UserId { get; set; }
        public DateTime? EntryDate { get; set; }
        public int? Calories { get; set; }
        public int? WaterOz { get; set; }

        public HealthMetric(int? healthMetricId, int? userId, DateTime? entryDate, int? calories, int? waterOz)
        {
            HealthMetricId = healthMetricId;
            UserId = userId;
            EntryDate = entryDate;
            Calories = calories;
            WaterOz = waterOz;
        }

        public static HealthMetric HealthCreate(HMCreate create)
        {
            return new HealthMetric(
                null,
                create.UserId,
                create.EntryDate,
                create.Calories,
                create.WaterOz
            );
        }

    }

    public class HMCreate
    {
        public int UserId { get; set; }
        public DateTime EntryDate { get; set; }
        public int Calories { get; set; }
        public int WaterOz { get; set; }
    }

}
