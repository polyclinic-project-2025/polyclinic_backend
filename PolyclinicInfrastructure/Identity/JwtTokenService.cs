using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Npgsql.Internal;
using PolyclinicApplication.Common.Interfaces;
using PolyclinicApplication.Common.Results;

namespace PolyclinicInfrastructure.Identity;

/// <summary>
/// Implementación del servicio de tokens JWT
/// Encapsula toda la lógica de generación y validación de tokens
/// </summary>
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
        var key = Encoding.ASCII.GetBytes(_secretKey);
        // Crear el token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expirationHours),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
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
}