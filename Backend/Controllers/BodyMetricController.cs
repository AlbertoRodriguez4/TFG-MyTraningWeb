using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using AA2_CS.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BodyMetricController : ControllerBase
{
    private readonly BodyMetricService _service;

    public BodyMetricController(BodyMetricService service)
    {
        _service = service;
    }

    [HttpGet("my-metrics")]
    [Authorize]
    public async Task<IActionResult> GetMyMetrics()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var metrics = await _service.GetByUserIdAsync(userId);
        return Ok(metrics);
    }

    [HttpGet("my-latest")]
    [Authorize]
    public async Task<IActionResult> GetMyLatest()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var metric = await _service.GetLatestAsync(userId);
        if (metric == null) return NotFound();
        return Ok(metric);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] BodyMetricCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var metric = new BodyMetric
        {
            UserId = userId,
            Date = dto.Date,
            Weight = dto.Weight,
            Height = dto.Height,
            BodyFat = dto.BodyFat,
            Chest = dto.Chest,
            Waist = dto.Waist,
            Arms = dto.Arms,
            Thighs = dto.Thighs,
            ProgressPhotoUrl = dto.ProgressPhotoUrl,
            Notes = dto.Notes
        };

        var created = await _service.AddAsync(metric);
        return Ok(created);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] BodyMetricCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.UserId != userId && !User.IsInRole("userMaster"))
            return Forbid();

        var metric = new BodyMetric
        {
            Id = id,
            UserId = existing.UserId,
            Date = dto.Date,
            Weight = dto.Weight,
            Height = dto.Height,
            BodyFat = dto.BodyFat,
            Chest = dto.Chest,
            Waist = dto.Waist,
            Arms = dto.Arms,
            Thighs = dto.Thighs,
            ProgressPhotoUrl = dto.ProgressPhotoUrl,
            Notes = dto.Notes
        };

        var updated = await _service.UpdateAsync(metric);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        if (existing.UserId != userId && !User.IsInRole("userMaster"))
            return Forbid();

        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { mensaje = "Metrica eliminada." });
    }
}
