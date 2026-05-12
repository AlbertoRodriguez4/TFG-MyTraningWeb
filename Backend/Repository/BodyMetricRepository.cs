using Microsoft.EntityFrameworkCore;
using AA2_CS.Model;
using AA2_CS.Database;

public class BodyMetricRepository
{
    private readonly AppDbContext _context;

    public BodyMetricRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<BodyMetric>> GetByUserIdAsync(int userId)
    {
        return await _context.BodyMetrics
            .Where(bm => bm.UserId == userId)
            .OrderByDescending(bm => bm.Date)
            .ToListAsync();
    }

    public async Task<BodyMetric?> GetByIdAsync(int id)
    {
        return await _context.BodyMetrics.FindAsync(id);
    }

    public async Task<BodyMetric> AddAsync(BodyMetric metric)
    {
        _context.BodyMetrics.Add(metric);
        await _context.SaveChangesAsync();
        return metric;
    }

    public async Task<BodyMetric> UpdateAsync(BodyMetric metric)
    {
        _context.BodyMetrics.Update(metric);
        await _context.SaveChangesAsync();
        return metric;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var metric = await _context.BodyMetrics.FindAsync(id);
        if (metric == null) return false;
        _context.BodyMetrics.Remove(metric);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<BodyMetric?> GetLatestAsync(int userId)
    {
        return await _context.BodyMetrics
            .Where(bm => bm.UserId == userId)
            .OrderByDescending(bm => bm.Date)
            .FirstOrDefaultAsync();
    }
}
