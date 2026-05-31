using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AA2_CS.Model.Entities;
using AA2_CS.Model.Common;
using AA2_CS.Service;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace AA2_CS.JWT
{
    public class JWTConfigurer
    {
        private readonly IConfiguration _config;
        private readonly UserService _userService;

        public JWTConfigurer(IConfiguration config, UserService userService)
        {
            _config = config;
            _userService = userService;
        }

        public string GenerateToken(User user)
        {
            // Obtener las estadísticas del usuario para incluirlas como claims en el token. Esto permite que el cliente tenga acceso a esta información
            var (totalStrength, totalEndurance, xpRequiredForNextLevel, xpRemaining) = _userService.GetUserTokenStats(user);

            // Creación de claims para los tokens, se definen los campos que se van a pasar
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Role, user.role),
                new Claim("email", user.email),
                new Claim("name", user.name),
                new Claim("emailVerified", user.isEmailVerified ? "true" : "false"),

                new Claim("level", user.level.ToString()),

                // Stats calculadas
                new Claim("strength", totalStrength.ToString()),
                new Claim("endurance", totalEndurance.ToString()),

                new Claim("gold", user.gold.ToString()),
                new Claim("consistencystreak", user.consistencystreak.ToString()),

                // Claims de experiencia
                new Claim("experience", user.experience.ToString()),       // XP Actual
                new Claim("xpRequired", xpRequiredForNextLevel.ToString()), // Meta del nivel
                new Claim("xpRemaining", xpRemaining.ToString()),            // Cuánto falta

                // Items equipados
                new Claim("equippedStrengthItemId", user.equippedStrengthId?.ToString() ?? ""),
                new Claim("equippedEnduranceItemId", user.equippedEnduranceId?.ToString() ?? ""),

                // Avatar URL
                new Claim("avatarUrl", user.avatarUrl ?? "")
            };

            var signingKey = _config["JwtSettings:Key"] ?? throw new InvalidOperationException("Falta JwtSettings:Key.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // Token válido por 7 días
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
