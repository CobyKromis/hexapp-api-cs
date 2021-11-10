using hexapp_api_cs.Models.Authentication;
using hexapp_api_cs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hexapp_api_cs.Services
{
    public class HealthMetricService
    {
        private readonly HexContext _context;

        public HealthMetricService()
        {
            _context = new HexContext();
        }

        public async Task<HealthMetric> Create(HMCreate create)
        {
            var entry = HealthMetric.HealthCreate(create);

            await _context.HealthMetrics.AddAsync(entry);
            await _context.SaveChangesAsync();

            return entry;
        }

        public async Task<List<HealthMetric>> GetLastSevenDays(int userId)
        {
            return await _context.HealthMetrics.Where(e => e.UserId == userId && e.EntryDate > DateTime.UtcNow.Date.AddDays(-7) && e.EntryDate <= DateTime.UtcNow.Date).ToListAsync();
        }

        public async Task<List<HealthMetric>> GetToday(int userId)
        {
            return await _context.HealthMetrics.Where(e => e.UserId == userId && e.EntryDate == DateTime.UtcNow.Date).ToListAsync();
        }
    }
}
