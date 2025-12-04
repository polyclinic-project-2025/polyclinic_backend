using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Request.Auth;
using PolyclinicApplication.DTOs.Response.Auth;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.Services.Implementations;

namespace PolyclinicApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
[AllowAnonymous]
public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _authService.RegisterAsync(registerDto); // ← Usa await
    
    if(!result.IsSuccess)
    {
        _logger.LogWarning("Usuario no registrado");
        return BadRequest(result.ErrorMessage);
    }

    _logger.LogInformation("Usuario registrado exitosamente: {Email}", registerDto.Email);
    return Ok(result.Value);
}

[HttpPost("login")]
[AllowAnonymous]
public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var result = await _authService.LoginAsync(loginDto); // ← Usa await
    
    if(!result.IsSuccess)
    {
        _logger.LogWarning("Usuario inválido");
        return BadRequest(result.ErrorMessage);
    }

    _logger.LogInformation("Usuario autenticado exitosamente: {Email}", loginDto.Email);
    return Ok(result.Value);
}

}