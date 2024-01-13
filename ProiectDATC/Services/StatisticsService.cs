using Microsoft.EntityFrameworkCore;
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
        var statistics = await _context.Statistics.ToListAsync();
        return statistics;
    }
}
