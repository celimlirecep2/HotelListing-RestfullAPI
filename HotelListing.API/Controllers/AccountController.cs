using AutoMapper;
using HotelListing.API.Core.Abstract;
using HotelListing.API.Core.Models.Users;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAuthManager authManager,ILogger<AccountController> logger )
        {
            this._authManager = authManager;
            this._logger = logger;
        }
        // api/account/register
        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Register([FromBody]ApiUserDTO apiUserDTO)
        {
                var errors = await _authManager.RegisterAsync(apiUserDTO);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    _logger.LogInformation($"Registration attempt for {apiUserDTO.Email}");
                    return BadRequest(ModelState);
                }
                return Ok(apiUserDTO);
        }

        //api/account/login
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Login([FromBody] LoginDTO model)
        {
                var authController = await _authManager.LoginAsync(model);
                if (authController == null)
                    return Unauthorized();
                _logger.LogInformation($"Registration attempt for {model.Email}");
                return Ok(authController);
        }

        //api/account/refreshtoken
        [HttpPost]
        [Route("refreshtoken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RefreshToken([FromBody] AuthResponseDTO request)
        {
            var authResponse = await _authManager.VerifyRefreshTokenAsync(request);
            if (authResponse == null)
                return Unauthorized();

            return Ok(authResponse);
        }


    }
}
