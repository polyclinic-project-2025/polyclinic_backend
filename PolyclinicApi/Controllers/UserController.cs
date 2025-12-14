using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicApplication.Services.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response.Export;

namespace PolyclinicAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private readonly IUserProfileService _userProfileService;
    private readonly IExportService _exportService;

    public UserController(
        IUserService userService, 
        ITokenService tokenService,
        IUserProfileService userProfileService,
        IExportService exportService)
    {
        _userService = userService;
        _tokenService = tokenService;
        _userProfileService = userProfileService;
        _exportService = exportService;
    }

    //GET: api/user - Solo Admin puede ver todos los usuarios
    [HttpGet]
    // [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllAsync()
    {
        Console.WriteLine("Entroooo");
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        var result = await _userService.GetAllAsync();
        if (!result.IsSuccess)
            return BadRequest(ApiResult<IEnumerable<UserResponse>>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<IEnumerable<UserResponse>>.Ok(result.Value!, "Usuarios obtenidos exitosamente"));
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
        
        var result = await _userService.RemoveUserAsync(id);
        if (!result.IsSuccess)
            return BadRequest(ApiResult<string>.Error(result.ErrorMessage!));
        
        return Ok(ApiResult<string>.Ok(result.Value!, "Usuario eliminado exitosamente"));
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

        var result = await _userService.UpdateUserValueAsync(id, updateUserDto);
        if (!result.IsSuccess)
        {
            Console.WriteLine(result.ErrorMessage);
            return BadRequest(ApiResult<UserResponse>.Error(result.ErrorMessage!));
        }

        return Ok(ApiResult<UserResponse>.Ok(result.Value!, "Usuario actualizado exitosamente"));
    }

    /// <summary>
    /// Obtiene solo el tipo de entidad vinculada a un usuario.
    /// Útil para verificaciones rápidas sin cargar todos los datos.
    /// Debe ir ANTES de {id}/profile para que no sea capturada por esa ruta.
    /// </summary>
    /// <param name="id">ID del usuario en Identity</param>
    /// <returns>"Doctor", "Nurse", "WarehouseManager" o "Patient"</returns>
    [HttpGet("{id}/profile/type")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> GetUserProfileTypeAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("El ID de usuario es requerido.");

        // Verificar autorización: Admin o el propio usuario
        var authorization = Request.Headers["Authorization"].ToString();
        Console.WriteLine($"autorizacion: {authorization}");
        var currentUser = await _tokenService.DecodingAuthAsync(authorization);
        if(!currentUser.IsSuccess) return Unauthorized();
        var user = currentUser.Value;
        Console.WriteLine("Current User ID: " + user?.Id);
        var isAdmin = user!.Roles!.Contains("Admin");
        var isOwner = user!.Id == id;

        if (!isAdmin && !isOwner)
            return Forbid();

        var result = await _userProfileService.GetLinkedEntityTypeAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<string>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<object>.Ok(new { profileType = result.Value }, "Tipo de perfil obtenido"));
    }

    /// <summary>
    /// Obtiene el perfil vinculado a un usuario (Doctor, Nurse, WarehouseManager o Patient).
    /// Retorna la información completa según la categoría más alta del empleado.
    /// </summary>
    /// <param name="id">ID del usuario en Identity</param>
    /// <returns>UserProfileResponse con el tipo y datos del perfil</returns>
    [HttpGet("{id}/profile")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserProfileResponse>> GetUserProfileAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest("El ID de usuario es requerido.");

        // Verificar autorización: Admin o el propio usuario
        var authorization = Request.Headers["Authorization"].ToString();
        Console.WriteLine($"autorizacion: {authorization}");
        var currentUser = await _tokenService.DecodingAuthAsync(authorization);
        if (!currentUser.IsSuccess) 
            return Unauthorized();
        
        var user = currentUser.Value;
        var isAdmin = user!.Roles!.Contains("Admin");
        var isOwner = user!.Id == id;

        if (!isAdmin && !isOwner)
            return Forbid();

        var result = await _userProfileService.GetUserProfileAsync(id);
        if (!result.IsSuccess)
            return NotFound(ApiResult<UserProfileResponse>.NotFound(result.ErrorMessage!));

        return Ok(ApiResult<UserProfileResponse>.Ok(result.Value!, "Perfil obtenido exitosamente"));
    }

    // ============================
    // GET: api/user/export
    // Exportar todos los usuarios a PDF
    // ============================
    [HttpGet("export")]
    public async Task<ActionResult> ExportUsers()
    {
        // Obtener todos los usuarios
        var usersResult = await _userService.GetAllAsync();
        if (!usersResult.IsSuccess)
            return BadRequest(ApiResult<string>.Error(usersResult.ErrorMessage!));

        // Exportar usando el servicio
        var exportResult = await _exportService.ExportDataAsync(usersResult.Value!, "pdf");
        if (!exportResult.IsSuccess)
            return BadRequest(ApiResult<ExportResponse>.Error(exportResult.ErrorMessage!));

        return Ok(ApiResult<ExportResponse>.Ok(exportResult.Value!, "Usuarios exportados exitosamente"));
    }
}