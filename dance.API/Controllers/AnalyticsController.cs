using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dance.API.Data;

namespace dance.API.Controllers
{
    [Authorize(Roles = "Admin,Director")]
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AnalyticsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/analytics/groups/occupancy
         [HttpGet("groups/occupancy")]
        public async Task<IActionResult> GetGroupsOccupancy()
        {
            var groups = await _dbContext.Groups
                .Select(g => new
                {
                    g.Group_id,
                    g.Name,
                    g.Status,
                    StudentsCount = _dbContext.Registrations.Count(r => r.Group_id == g.Group_id),
                    MaxCapacity = 15,
                    OccupancyPercent = _dbContext.Registrations.Count(r => r.Group_id == g.Group_id) * 100 / 15
                })
                .OrderByDescending(g => g.OccupancyPercent)
                .ToListAsync();

            var totalGroups = groups.Count;
            var totalStudents = groups.Sum(g => g.StudentsCount);
            var avgOccupancy = totalGroups > 0 ? totalStudents * 100 / (totalGroups * 15) : 0;

            return Ok(new
            {
                groups = groups,
                totalGroups = totalGroups,
                totalStudents = totalStudents,
                averageOccupancyPercent = avgOccupancy
            });
        }

        // GET: api/analytics/directions/popularity
        [HttpGet("directions/popularity")]
        public async Task<IActionResult> GetDirectionsPopularity()
        {
            var popularity = await _dbContext.Directions
                .Select(d => new
                {
                    d.Direction_id,
                    d.Name,
                    GroupsCount = _dbContext.Groups.Count(g => g.Direction_id == d.Direction_id),
                    StudentsCount = _dbContext.Groups
                        .Where(g => g.Direction_id == d.Direction_id)
                        .SelectMany(g => g.Registrations)
                        .Count(),
                    TrainersCount = _dbContext.Trainers.Count(t => t.Direction_id == d.Direction_id),
                    TrainerNames = _dbContext.Trainers
                        .Where(t => t.Direction_id == d.Direction_id)
                        .Select(t => t.FirstName + " " + t.LastName)
                        .ToList()
                })
                .OrderByDescending(d => d.StudentsCount)
                .ToListAsync();

            return Ok(popularity);
        }

        // GET: api/analytics/trainers/workload
        [HttpGet("trainers/workload")]
        public async Task<IActionResult> GetTrainersWorkload()
        {
            var workload = await _dbContext.Trainers
                .Select(t => new
                {
                    t.Trainer_id,
                    t.FirstName,
                    t.LastName,
                    DirectionName = _dbContext.Directions
                        .Where(d => d.Direction_id == t.Direction_id)
                        .Select(d => d.Name)
                        .FirstOrDefault(),
                    GroupsCount = _dbContext.Groups.Count(g => g.Trainer_id == t.Trainer_id),
                    StudentsCount = _dbContext.Groups
                        .Where(g => g.Trainer_id == t.Trainer_id)
                        .SelectMany(g => g.Registrations)
                        .Count(),
                    ClassesCount = _dbContext.Classes.Count(c => c.Trainer_id == t.Trainer_id && c.Status == "Проведено"),
                    UpcomingClasses = _dbContext.Classes.Count(c => c.Trainer_id == t.Trainer_id && c.Status == "Запланировано")
                })
                .OrderByDescending(t => t.StudentsCount)
                .ToListAsync();

            return Ok(workload);
        }

        // GET: api/analytics/financial
        [HttpGet("financial")]
        public async Task<IActionResult> GetFinancialAnalytics()
        {
            var totalRevenue = await _dbContext.Subscriptions.SumAsync(s => s.Amount);
            var activeSubscriptions = await _dbContext.Subscriptions.CountAsync(s => s.Status == "Активен");
            var totalClients = await _dbContext.Clients.CountAsync();
            var totalSubscriptions = await _dbContext.Subscriptions.CountAsync();
            var totalAmount = await _dbContext.Subscriptions.SumAsync(s => s.Amount);

            return Ok(new
            {
                totalRevenue = totalRevenue,
                activeSubscriptions = activeSubscriptions,
                totalClients = totalClients,
                averageRevenuePerClient = totalClients > 0 ? totalRevenue / totalClients : 0,
                totalSubscriptions = totalSubscriptions,
                totalAmount = totalAmount
            });
        }

        // GET: api/analytics/clients/growth
        [HttpGet("clients/growth")]
        public async Task<IActionResult> GetClientsGrowth()
        {
            var growth = await _dbContext.Registrations
                .GroupBy(r => new { r.Registration_date.Year, r.Registration_date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    NewClients = g.Select(r => r.Client_id).Distinct().Count()
                })
                .OrderBy(g => g.Year)
                .ThenBy(g => g.Month)
                .Take(12)
                .ToListAsync();

            return Ok(growth);
        }
    }
}