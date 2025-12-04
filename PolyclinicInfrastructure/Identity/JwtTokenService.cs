using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;
using PolyclinicApplication.DTOs.Response;

namespace PolyclinicInfrastructure.Identity;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationHours;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        // Leer configuración JWT
        _secretKey = _configuration["Jwt:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey no configurada");
        
        _issuer = _configuration["Jwt:Issuer"] 
            ?? throw new InvalidOperationException("JWT Issuer no configurado");
        
        _audience = _configuration["Jwt:Audience"] 
            ?? throw new InvalidOperationException("JWT Audience no configurado");
        
        _expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24");
    }

    public async Task<Result<string>> GenerateTokenAsync(
        string userId, 
        string email, 
        IList<string> roles, 
        IDictionary<string, string>? additionalClaims = null)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        // Agregar roles como claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Agregar claims adicionales si existen
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        // Crear la clave de seguridad
        Console.WriteLine($"Secret Key: {_secretKey}");
        var key = Encoding.UTF8.GetBytes(_secretKey);
        
        // Crear el token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        // Retornar el token como string
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Result<string>.Success(tokenString);
    }

    public async Task<Result<ClaimsPrincipal?>> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return Result<ClaimsPrincipal?>.Failure("Token inválido");
        
        var parts = token.Split('.');
        if (parts.Length != 3) return Result<ClaimsPrincipal?>.Failure("Token inválido");

        //validate signature
        try
        {
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.NameIdentifier
            };

            tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken tokenValidated);
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claimsIdentity = new ClaimsIdentity(jwtToken.Claims);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return Result<ClaimsPrincipal?>.Success(claimsPrincipal);
        }
        catch (System.Exception e)
        {
            return Result<ClaimsPrincipal?>.Failure($"Validación de token fallida: {e.Message}");
        }
    }

    public async Task<int> GetTokenExpirationHoursAsync()
    {
        return _expirationHours;
    }

    public async Task<Result<UserResponse>> DecodingAuthAsync(string authHeader)
    {
        var response = await DecodeJwtTokenAsync(authHeader);
        if (!response.IsSuccess) return Result<UserResponse>.Failure(response.ErrorMessage!);

        var jwtToken = response.Value!;

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == JwtRegisteredClaimNames.Sub);
        var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == ClaimTypes.Email);
        var phoneNumberClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone);

        var roleClaims = jwtToken.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).Distinct().ToList();

        if (idClaim is null)
            return Result<UserResponse>.Failure("Token inválido: falta el claim de ID");
            
        if (emailClaim is null)
            return Result<UserResponse>.Failure("Token inválido: falta el claim de Email");

        try
        {
            var userResponse = new UserResponse
            {
                Id = idClaim.Value,
                Email = emailClaim.Value,
                PhoneNumber = phoneNumberClaim?.Value,
                Roles = roleClaims.Any() ? roleClaims : new List<string>()
            };

            return Result<UserResponse>.Success(userResponse);
        }
        catch (Exception ex)
        {
            return Result<UserResponse>.Failure($"Error al decodificar token: {ex.Message}");
        }


        
    }

    public async Task<Result<JwtSecurityToken>> DecodeJwtTokenAsync(string authHeader, bool bearer = true)
    {
        try
        {
            string token;

            if (bearer)
            {
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return await Task.FromResult(Result<JwtSecurityToken>.Failure("Encabezado de autorización inválido"));

                token = authHeader.Substring("Bearer ".Length).Trim();
            }
            else
            {
                token = authHeader;
            }

            if (string.IsNullOrEmpty(token))
                return await Task.FromResult(Result<JwtSecurityToken>.Failure("Token vacío"));

            var handler = new JwtSecurityTokenHandler();
            
            // Validar que el token sea un JWT válido
            if (!handler.CanReadToken(token))
                return await Task.FromResult(Result<JwtSecurityToken>.Failure("Token no es un JWT válido"));

            var jwtToken = handler.ReadJwtToken(token);
            return await Task.FromResult(Result<JwtSecurityToken>.Success(jwtToken));
        }
        catch (Exception ex)
        {
            return await Task.FromResult(Result<JwtSecurityToken>.Failure($"Error al decodificar token: {ex.Message}"));
        }
    }
}