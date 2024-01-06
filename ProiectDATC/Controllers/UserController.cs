using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectDATC.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role).Value.ToString();
            if (role != "ADMIN")
            {
                throw new AccessViolationException("Access denied");
            }
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto model)
    {
        try
        {
            model.Role = "NORMAL";

            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                Role = model.Role,
                Password = model.Password
            };

            var userId = await _userService.CreateUserAsync(newUser);
            var user = await _userService.GetUserByIdAsync(userId);
            var token = _userService.GenerateJwtToken(user);

            return Ok(new { Token = token });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }


    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] UserLogin model)
    {
        try
        {
            var user = await _userService.LoginAsync(model);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            var token = _userService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("currentUser")]
    [Authorize]
    public async Task<IActionResult> GetCurrentLoggedInUser()
    {
        if (User.Identity.IsAuthenticated)
        {
            var uid = User.FindFirst(ClaimTypes.NameIdentifier);
            
            if (uid != null)
            {
                var userId = uid.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var user = await _userService.GetUserByIdAsync(int.Parse(userId));

                    if (user != null)
                    {
                        return Ok(new {user.Id, user.Username, user.Role});
                    }
                    else
                    {
                        return NotFound($"User not found for id: {userId}");
                    }
                }
                else
                {
                    return BadRequest("Id claim has null or empty value");
                }
            }
            else
            {
                return BadRequest("Id claim not found");
            }

        }
        else
        {
            return Unauthorized("User is not authenticated");
        }
    }

    [HttpGet("details/{id}")]
    [Authorize]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role).Value.ToString();
            if (role != "ADMIN")
            {
                throw new AccessViolationException("Access denied");
            }
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<IActionResult> GetUserByEmail(string email)
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role).Value.ToString();
            if (role != "ADMIN")
            {
                throw new AccessViolationException("Access denied");
            }
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPut("edit/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] User model)
    {
        try
        {
            var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (id != uid)
            {
                throw new AccessViolationException("Access denied");
            }
            await _userService.UpdateUserAsync(id, model);
            return Ok("User updated successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var uid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;
            if (id != uid || role != "ADMIN")
            {
                throw new AccessViolationException("Access denied");
            }
            await _userService.DeleteUserAsync(id);
            return Ok("User deleted successfully");
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}
