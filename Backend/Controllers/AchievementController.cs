using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AA2_CS.Model.Entities;
using AA2_CS.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AchievementController : ControllerBase
{
    private readonly AchievementService _achievementService;

    public AchievementController(AchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var achievements = await _achievementService.GetAllAsync();
        return Ok(achievements);
    }

    [HttpGet("my-achievements")]
    [Authorize]
    public async Task<IActionResult> GetMyAchievements()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        await _achievementService.EvaluateUserAchievementsAsync(userId);

        var achievements = await _achievementService.GetUserAchievementsAsync(userId);
        return Ok(achievements);
    }

    [HttpGet("my-count")]
    [Authorize]
    public async Task<IActionResult> GetMyAchievementCount()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        if (userId == 0) return Unauthorized();

        var count = await _achievementService.GetUserAchievementCountAsync(userId);
        var newCount = await _achievementService.CountNewAchievementsAsync(userId);
        return Ok(new { total = count, newAchievements = newCount });
    }

    [HttpPost("mark-seen/{userAchievementId}")]
    [Authorize]
    public async Task<IActionResult> MarkAsSeen(int userAchievementId)
    {
        var result = await _achievementService.MarkAsSeenAsync(userAchievementId);
        if (!result) return NotFound();
        return Ok(new { mensaje = "Logro marcado como visto." });
    }

    [HttpPost]
    [Authorize(Roles = "userMaster,userStaff")]
    public async Task<IActionResult> Create([FromBody] Achievement achievement)
    {
        var created = await _achievementService.AddAsync(achievement);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "userMaster,userStaff")]
    public async Task<IActionResult> Update(int id, [FromBody] Achievement achievement)
    {
        var updated = await _achievementService.UpdateAsync(id, achievement);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "userMaster")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _achievementService.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { mensaje = "Logro eliminado." });
    }
}
