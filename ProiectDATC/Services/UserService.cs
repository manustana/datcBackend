// Services/UserService.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProiectDATC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

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
        // Check if a user with the same email or CNP already exists
        if (await _context.Users.AnyAsync(u => u.Email == user.Email))
        {
            throw new ArgumentException("User with that email already exists");
        }

        if (await _context.Users.AnyAsync(u => u.Cnp == user.Cnp))
        {
            throw new ArgumentException("User with that CNP already exists");
        }

        // Validate CNP
        if (!ValidateCNP(user.Cnp))
        {
            throw new ArgumentException("Invalid CNP");
        }

        // Hash the password
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Points = 0;
        // Add the user to the context and save changes
        _context.Users.Add(user);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Check if the exception is due to a unique constraint violation
            if (IsUniqueConstraintViolation(ex))
            {
                throw new ArgumentException("Concurrency issue: Another user with the same email or CNP was created concurrently.");
            }

            // Handle other exceptions or rethrow if necessary
            throw;
        }

        return user.Id;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        const int uniqueConstraintViolationErrorNumber = 2601; // SQL Server error number for unique constraint violation

        return ex.InnerException is SqlException sqlException &&
               sqlException.Number == uniqueConstraintViolationErrorNumber;
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

    public static bool ValidateCNP(string cnp)
    {
        // Check if the CNP has the correct length
        if (cnp.Length != 13)
        {
            return false;
        }

        // Check if all characters are digits
        if (!cnp.All(char.IsDigit))
        {
            return false;
        }

        // Extract individual components
        int year = int.Parse(cnp.Substring(1, 2));
        int month = int.Parse(cnp.Substring(3, 2));
        int day = int.Parse(cnp.Substring(5, 2));

        // Validate date components
        if (!IsValidDate(year, month, day))
        {
            return false;
        }

        // Check the county code
        int countyCode = int.Parse(cnp.Substring(7, 2));
        if (countyCode < 1 || countyCode > 52)
        {
            return false;
        }

        // Validate the rest of the CNP using the control digit
        int controlDigit = int.Parse(cnp.Substring(12, 1));
        int computedControlDigit = ComputeControlDigit(cnp);

        return controlDigit == computedControlDigit;
    }

    private static bool IsValidDate(int year, int month, int day)
    {
        // Add your custom validation logic for the date (e.g., considering leap years)
        // For simplicity, this example only checks the month and day ranges.
        return year >= 0 && month >= 1 && month <= 12 && day >= 1 && day <= 31;
    }

    private static int ComputeControlDigit(string cnp)
    {
        int[] weights = { 2, 7, 9, 1, 4, 6, 3, 5, 8, 2, 7, 9 };

        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += int.Parse(cnp[i].ToString()) * weights[i];
        }

        int remainder = sum % 11;
        return remainder < 10 ? remainder : 1;
    }

}
