
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TatumConnectBackened.DTOs;
using TatumConnectBackened.Services;

namespace TatumConnectBackened.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
        {
            try
            {

                var response = await _auth.RegisterAsync(request, ct);
                if (!response.Success)
                {
                    return Conflict(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        [HttpPost("verify-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> Verify([FromBody] VerifyRegistrationOtpRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.VerifyRegistrationOtpAsync(request, ct);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        [HttpPost("resend-registration-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendRegistrationOtp([FromBody] ResendRegistrationOtpRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.ResendRegistrationOtpAsync(request, ct);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
        {
            try
            {
                var response = await _auth.LoginAsync(request, ct);
                if (!response.Success)
                {
                    return Unauthorized(response);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest("An error occured");
            }
        }
    }
}