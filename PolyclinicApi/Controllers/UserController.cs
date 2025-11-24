using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PolyclinicAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    //GET: api/user - Solo Admin puede ver todos los usuarios
    [HttpGet]
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllAsync()
    {
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
        var currentUserId = User.FindFirst("sub")?.Value;
        Console.WriteLine($"Current User ID: {currentUserId}, Target User ID: {id}");
        
        // Verificar si es Admin o el dueño de la cuenta
        var isAdmin = User.IsInRole("Admin");
        var isOwner = currentUserId == id;
        
        if (!isAdmin && !isOwner)
        {
            return Forbid(); // 403 Forbidden
        }
        
        var result = await _userService.RemoveUserAsync(id);
        
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
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Verificar si es Admin o el dueño de la cuenta
        var isAdmin = User.IsInRole("Admin");
        var isOwner = currentUserId == id;
        
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