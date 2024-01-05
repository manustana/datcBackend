// Controllers/ReportController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectDATC.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Use [Authorize] attribute to ensure authentication is required for all actions
public class ReportController : ControllerBase
{
    private readonly ReportService _reportService;
    private readonly UserService _userService;
    public ReportController(ReportService reportService, UserService userService)
    {
        _reportService = reportService;
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllReports()
    {
        try
        {
            var role = User.FindFirst(ClaimTypes.Role).Value.ToString();
            if (role != "ADMIN")
            {
                throw new AccessViolationException("Access denied");
            }
            var reports = await _reportService.GetAllReportsAsync();
            return Ok(reports);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<IActionResult> CreateReport([FromBody] ReportDto model)
    {
        try
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
                            model.UserId = user.Id;

                            await _reportService.CreateReportAsync(model);

                            return Ok("Report creation request sent to the queue");
                        }
                        else
                        {
                            return BadRequest($"User not found for id: {userId}");
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
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }


    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetReportById(int id)
    {
        try
        {
            var report = await _reportService.GetReportByIdAsync(id);

            if (report == null)
            {
                return NotFound("Report not found");
            }

            return Ok(report);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPut("edit/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateReport(int id, [FromBody] ReportDto model)
    {
        try
        {
            await _reportService.UpdateReportAsync(id, model);
            return Ok("Report updated successfully");
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
    public async Task<IActionResult> DeleteReport(int id)
    {
        try
        {
            await _reportService.DeleteReportAsync(id);
            return Ok("Report deleted successfully");
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
