// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProiectDATC.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class UserService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<User> LoginAsync(UserLogin model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            return null; // Return null if the login is unsuccessful
        }

        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<int> CreateUserAsync(User user)
    {
        // Check if the username is already taken
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            throw new ArgumentException("Username already exists");
        }

        // Hash the password (You should use a proper password hashing mechanism)
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Role = "NORMAL";
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user.Id;
    }

    public async Task UpdateUserAsync(int id, User model)
    {
        var existingUser = await _context.Users.FindAsync(id);

        if (existingUser == null)
        {
            throw new ArgumentException("User not found");
        }

        existingUser.Username = model.Username;
        existingUser.Role = model.Role;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static int GetUserIdFromToken(string token)
    {
        // Extract user ID from the token claims (implement as needed)
        // For example, using System.IdentityModel.Tokens.Jwt:
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        var userId = int.Parse(jsonToken?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value);
        return userId;
    }
}
