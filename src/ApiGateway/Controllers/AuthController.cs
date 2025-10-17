using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // DTO simulado para o login
    public class LoginModel {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel login)
    {
        // Em um app real, validar login.Password contra um hash no seu DB.
        // Aqui, vamos apenas simular um login bem-sucedido.
        if (login.Username == "usuario" && login.Password == "senha123")
        {
            var token = GerarTokenJwt(login.Username);
            return Ok(new { token = token });
        }

        return Unauthorized("Usuário ou senha inválidos.");
    }

    private string GerarTokenJwt(string username)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
        var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username), // "Subject" (o usuário)
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // "JWT ID" (um ID único para o token)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // Token expira em 2 horas
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}