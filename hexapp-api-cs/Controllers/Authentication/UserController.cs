using hexapp_api_cs.Helpers.Authentication;
using hexapp_api_cs.Models.Authentication;
using hexapp_api_cs.Services.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hexapp_api_cs.Controllers.Authentication
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;

        public UserController(UserService userService, TokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get([FromHeader] string authToken)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            return await _userService.Get();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> Get([FromHeader] string authToken, int id)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            var user = await _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> Create([FromHeader] string authToken, UserCreate create)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            User created = await _userService.Create(create);

            return Ok(create);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromHeader] string authToken, int id, UserUpdate update)
        {
            if (!AuthenticationHelpers.IsTokenValid(authToken))
            {
                return Unauthorized();
            }

            var user = await _userService.Get(id);

            if (user == null)
            {
                return NotFound();
            }

            _userService.Update(user, update);

            return Ok();
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        {
            User user = await _userService.GetByUsername(username);

            if (user == null)
            {
                return NotFound("username does not exist");
            }

            if (!AuthenticationHelpers.IsPasswordValid(password, user.HashedPassword))
            {
                return Unauthorized("incorrect password");
            }

            string authToken = AuthenticationHelpers.GenerateAuthToken(user);

            await _tokenService.Create(new Token(
                0,
                user.UserId,
                "auth",
                authToken,
                DateTime.UtcNow,
                false,
                true
            ));

            var createdAuthToken = await _tokenService.GetByToken(authToken);

            string refreshToken = AuthenticationHelpers.GenerateRefreshToken(user, createdAuthToken.TokenId);

            await _tokenService.Create(new Token(
                0,
                user.UserId,
                "refresh",
                refreshToken,
                DateTime.UtcNow,
                false,
                true
            ));

            return Ok(
                new Dictionary<string, string>
                {
                    { "authToken", authToken },
                    { "refreshToken", refreshToken }
                }
            );
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken)
        {
            Token token = await _tokenService.GetByToken(refreshToken);

            if (token == null)
            {
                return NotFound("refresh token not found");
            }

            // Check if token is valid
            if ((bool)!token.ValidFlag)
            {
                return Problem("invalidated token");
            }

            if ((bool)token.UsedFlag)
            {
                await _tokenService.InvalidateUserTokens((int)token.UserId);
                return Problem("refresh token already used");
            }

            // Check token validity (Expiration, issuer, audience, etc.)
            if (!AuthenticationHelpers.IsTokenValid(refreshToken))
            {
                return Problem("token failed validation");
            }

            var decodedToken = AuthenticationHelpers.ReadToken(refreshToken);

            await _tokenService.InvalidateToken(int.Parse(decodedToken.Claims.First(c => c.Type == "authTokenId").Value));
            await _tokenService.UseToken(token.TokenId);

            // Generate new token pair
            var user = await _userService.Get((int)token.UserId);

            string authToken = AuthenticationHelpers.GenerateAuthToken(user);

            await _tokenService.Create(new Token(
                0,
                user.UserId,
                "auth",
                authToken,
                DateTime.UtcNow,
                false,
                true
            ));

            var createdAuthToken = await _tokenService.GetByToken(authToken);

            string newRefreshToken = AuthenticationHelpers.GenerateRefreshToken(user, createdAuthToken.TokenId);

            await _tokenService.Create(new Token(
                0,
                user.UserId,
                "refresh",
                newRefreshToken,
                DateTime.UtcNow,
                false,
                true
            ));

            return Ok(
                new Dictionary<string, string>
                {
                    { "authToken", authToken },
                    { "refreshToken", newRefreshToken }
                }
            );
        }
    }
}
