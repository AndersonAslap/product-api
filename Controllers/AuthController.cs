using FirstApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace FirstApi.Controllers;

[ApiController]
[Route("api/account")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _singInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        SignInManager<IdentityUser> singInManager, 
        UserManager<IdentityUser> userManager, 
        IOptions<JwtSettings> jwtSettings)
    {
        _singInManager = singInManager;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserViewModel registerUser)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            await _singInManager.SignInAsync(user, false);
            return Ok(await GenerateJwt(registerUser.Email));
        }

        return Problem("Falha ao registar o usuário");
    }

    [HttpPost("sing-in")]
    public async Task<IActionResult> SignIn(SignInUserViewModel signInUser)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var result = await _singInManager.PasswordSignInAsync(signInUser.Email, signInUser.Password, false, true);

        if (result.Succeeded)
        {
            return Ok(await GenerateJwt(signInUser.Email));
        }

        return Problem("Email ou senha incorretos");
    }

    private async Task<string> GenerateJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiresHour),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);

        return encodedToken;
    }
}

