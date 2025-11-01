﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dunder_Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AdminAuthController(IConfiguration config)
        {
            _config = config;
        }

        public class LoginRequest { public string username { get; set; } public string password { get; set; } }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            var adminCfg = _config.GetSection("AdminCredentials");
            var configuredUser = adminCfg.GetValue<string>("Username");
            var configuredPass = adminCfg.GetValue<string>("Password");

            if (req.username != configuredUser || req.password != configuredPass)
                return Unauthorized("Credenciais inválidas");

            var jwtCfg = _config.GetSection("Jwt");
            var key = jwtCfg.GetValue<string>("Key");
            var issuer = jwtCfg.GetValue<string>("Issuer");
            var audience = jwtCfg.GetValue<string>("Audience");
            var expiryMinutes = jwtCfg.GetValue<int>("ExpiryMinutes");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, req.username),
                new Claim(ClaimTypes.Name, req.username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, expires = token.ValidTo });
        }
    }
}
