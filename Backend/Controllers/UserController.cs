using Microsoft.AspNetCore.Mvc;
using AA2_CS.Model;
using AA2_CS.Service;
using Microsoft.AspNetCore.Authorization;
using AA2_CS.Extensions;

namespace AA2_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TasksService _tasksService;
        private readonly ILogger<UserController> _logger;

        public UserController(UserService userService, TasksService tasksService, ILogger<UserController> logger)
        {
            _userService = userService;
            _tasksService = tasksService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult AddUser([FromBody] User user)
        {
            var result = _userService.Add(user);
            return result > 0 ? Ok(user) : BadRequest("Failed to add user");
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<User>> UpdateUser(int id, User user)
        {
            if (!User.IsSelfOrAdmin(id))
            {
                return Forbid();
            }

            var result = await _userService.UpdateById(id, user);
            return result;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult DeleteUser(int id)
        {
            var user = _userService.FindById(id);
            if (user == null) return NotFound();

            var result = _userService.Delete(user);
            return result > 0
                ? Ok(new { message = "User deleted successfully" })
                : BadRequest("Failed to delete user");
        }

        [HttpGet]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult GetAllUsers()
        {
            var users = _userService.FindAll();
            return Ok(users);
        }

        [HttpGet("community")]
        [Authorize]
        public IActionResult GetCommunityUsers()
        {
            try
            {
                var users = _userService.FindAll()
                    .Select(u => new
                    {
                        u.id,
                        u.name,
                        u.level,
                        u.strength,
                        u.endurance,
                        u.avatarUrl
                    })
                    .ToList();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios de comunidad");
                return StatusCode(500, new { message = "Error al obtener usuarios de comunidad" });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUserById(int id)
        {
            if (!User.IsSelfOrAdmin(id))
            {
                return Forbid();
            }

            var user = _userService.FindById(id);
            return user != null ? Ok(user) : NotFound();
        }

        [HttpGet("search/{name}")]
        [Authorize(Roles = Roles.userMaster)]
        public IActionResult FindUsersByCharacteristic(string name)
        {
            var users = _userService.FindByCharacteristic(name);
            return Ok(users);
        }

        [HttpGet("getTopThreeUsers")]
        [Authorize]
        public IActionResult GetTopThreeUsers()
        {
            var users = _userService.GetTopThreeUsers();
            return users != null ? Ok(users) : NotFound();
        }

        [HttpGet("community-stats")]
        [Authorize]
        public IActionResult GetCommunityStats()
        {
            try
            {
                var users = _userService.FindAll();
                var tasks = _tasksService.FindAll();

                var today = DateTime.UtcNow.Date;
                var activeToday = tasks
                    .Where(t => t.iscompleted && t.createdat.Date == today)
                    .Select(t => t.userId)
                    .Distinct()
                    .Count();

                var totalCheckIns = tasks.Count(t => t.iscompleted);

                var stats = new CommunityStatsResponse
                {
                    TotalUsers = users.Count,
                    ActiveToday = activeToday,
                    TotalCheckIns = totalCheckIns
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de comunidad");
                return StatusCode(500, new { message = "Error al obtener estadísticas de comunidad" });
            }
        }

        [HttpPost("equip/{userId}/{itemId}")]
        [Authorize]
        public IActionResult EquipItem(int userId, int itemId)
        {
            if (!User.IsSelfOrAdmin(userId))
            {
                return Forbid();
            }

            var result = _userService.EquipItem(userId, itemId);
            if (result.Contains("not found") || result.Contains("No posees"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("unequip/{userId}/{type}")]
        [Authorize]
        public IActionResult UnequipItem(int userId, string type)
        {
            if (!User.IsSelfOrAdmin(userId))
            {
                return Forbid();
            }

            var result = _userService.UnequipItem(userId, type);
            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = User.GetUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("Token inválido o ID de usuario no encontrado.");
            }

            bool success = await _userService.ChangePassword(userId.Value, request.CurrentPassword, request.NewPassword);

            if (success)
            {
                return Ok(new { message = "Contraseña actualizada correctamente." });
            }

            return BadRequest("La contraseña actual proporcionada es incorrecta.");
        }
    }
}
