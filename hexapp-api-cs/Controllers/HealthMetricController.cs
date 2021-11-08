using hexapp_api_cs.Helpers.Authentication;
using hexapp_api_cs.Models;
using hexapp_api_cs.Models.Authentication;
using hexapp_api_cs.Services;
using hexapp_api_cs.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hexapp_api_cs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthMetricController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        private readonly HealthMetricService _healthMetricService;

        public HealthMetricController(HealthMetricService healthMetricService, UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
            _healthMetricService = healthMetricService;
        }

        [HttpPost]
        public async Task<ActionResult<HealthMetric>> Create([FromHeader] string authToken, HMCreate create)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            HealthMetric created = await _healthMetricService.Create(create);

            return Ok(create);
        }

        [HttpGet("lastseven/{userId}")]
        public async Task<ActionResult<List<HealthMetric>>> GetLastSevenDays([FromHeader] string authToken, int userId)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            var metrics = await _healthMetricService.GetLastSevenDays(userId);

            return Ok(metrics);
        }

        [HttpGet("today/{userId}")]
        public async Task<ActionResult<List<HealthMetric>>> GetToday([FromHeader] string authToken, int userId)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            var metrics = await _healthMetricService.GetToday(userId);

            return Ok(metrics);
        }
    }
}
