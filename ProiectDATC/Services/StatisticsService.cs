// Services/StatisticsService.cs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProiectDATC.Models;

public class StatisticsService
{
    private readonly AppDbContext _context;

    public StatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Statistic>> GetStatisticsAsync()
    {
        // Implement your logic to retrieve statistics from the database or any other source
        // For example:
        var statistics = await _context.Statistics.ToListAsync();
        return statistics;
    }
}
