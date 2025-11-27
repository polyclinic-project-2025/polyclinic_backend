using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PolyclinicApplication.Common.Interfaces;

namespace PolyclinicAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;

    public UserController(IUserService userService, ITokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    //GET: api/user - Solo Admin puede ver todos los usuarios
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllAsync()
    {
        Console.WriteLine("Entroooo");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var result = await _userService.GetAllAsync();
        
        if (!result.IsSuccess) return BadRequest(result);
        
        return Ok(result);
    }

    //DELETE: api/user/{id} - Admin o el dueño de la cuenta
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<string>> RemoveUserAsync(string id)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        // Obtener el ID del usuario actual del token
        var authorization = Request.Headers["Authorization"].ToString();
        var currentUser = await _tokenService.DecodingAuthAsync(authorization);
        if(!currentUser.IsSuccess) return Unauthorized();
        var user = currentUser.Value;
        Console.WriteLine("Current User ID: " + user?.Id);
        
        // Verificar si es Admin o el dueño de la cuenta
        var isAdmin = user!.Roles!.Contains("Admin");
        var isOwner = user!.Id == id;
        
        if (!isAdmin && !isOwner)
        {
            return Forbid(); // 403 Forbidden
        }
        
        var result = await _userService.RemoveUserAsync(user.Email!);
        
        if (!result.IsSuccess) return BadRequest(result);
        
        return Ok(result);
    }

    //PATCH: api/user/{id} - Admin o el dueño de la cuenta
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UserResponse>> UpdateUserAsync(
        string id, 
        [FromBody] UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        if (updateUserDto == null)
            return BadRequest("Los datos de actualización son requeridos");

        // Obtener el ID del usuario actual del token
        var authorization = Request.Headers["Authorization"].ToString();
        var currentUser = await _tokenService.DecodingAuthAsync(authorization);
        if(!currentUser.IsSuccess) return Unauthorized();
        var user = currentUser.Value;
        Console.WriteLine("Current User ID: " + user?.Id);
        
        // Verificar si es Admin o el dueño de la cuenta
        var isAdmin = user!.Roles!.Contains("Admin");
        var isOwner = user!.Id == id;
        
        if (!isAdmin && !isOwner)
        {
            return Forbid(); // 403 Forbidden
        }

        // Si no es admin, validar que no intente modificar roles
        if (!isAdmin && updateUserDto.Operation != null && updateUserDto.Roles != null)
        {
            return Forbid(); // Solo admin puede modificar roles
        }

        var result = await _userService.UpdateUserValueAsync(id, updateUserDto);
        
        if (!result.IsSuccess) 
            return BadRequest(result);
        
        return Ok(result);
    }
}