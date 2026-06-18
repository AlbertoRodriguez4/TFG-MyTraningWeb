using Microsoft.AspNetCore.Mvc;
using AA2_CS.Model.Entities;
using AA2_CS.Model.Common;
using AA2_CS.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace AA2_CS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserRoomController : ControllerBase
    {
        private readonly UserRoomService _userRoomService;
        private readonly RoomService _roomService;
        private readonly IServiceProvider _serviceProvider;

        public UserRoomController(UserRoomService userRoomService, RoomService roomService, IServiceProvider serviceProvider)
        {
            _userRoomService = userRoomService;
            _roomService = roomService;
            _serviceProvider = serviceProvider;
        }

        // GET: api/UserRoom
        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            if (!IsAdmin())
            {
                return Forbid();
            }

            try
            {
                var result = _userRoomService.GetAll();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public IActionResult GetByUserId(int userId)
        {
            if (!CanAccessUserResource(userId))
            {
                return Forbid();
            }

            try
            {
                var result = _userRoomService.FindByUserId(userId);
                // Retornamos OK aunque la lista esté vacía (significa que el usuario no tiene salas)
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("room/{roomId}")]
        [Authorize]
        public IActionResult GetByRoomId(int roomId)
        {
            try
            {
                var result = _userRoomService.FindUsersByRoomId(roomId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add([FromBody] UserRoom userRoom)
        {
            try
            {
                if (!CanAccessUserResource(userRoom.userid))
                {
                    return Forbid();
                }

                var existing = _userRoomService.FindByCompositeKey(userRoom.userid, userRoom.roomid);
                if (existing != null)
                {
                    return BadRequest("User is already in this room.");
                }

                var result = _userRoomService.Add(userRoom);

                if (result > 0)
                {
                    var capturedUserId = userRoom.userid;
                    var capturedRoomId = userRoom.roomid;
                    _ = System.Threading.Tasks.Task.Run(async () =>
                    {
                        try
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();
                            var roomService = scope.ServiceProvider.GetRequiredService<RoomService>();
                            var room = roomService.FindById(capturedRoomId);
                            if (room != null)
                            {
                                await notificationService.SendRoomActivityNotificationIfNeeded(
                                    capturedUserId, room.name, "joined");
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    });
                }

                return result > 0 ? Ok(userRoom) : BadRequest("Failed to add user to room");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{userId}/{roomId}")]
        [Authorize]
        public async Task<IActionResult> Update(int userId, int roomId, [FromBody] UserRoom updatedUserRoom)
        {
            try
            {
                if (!CanAccessUserResource(userId))
                {
                    return Forbid();
                }

                if (userId != updatedUserRoom.userid || roomId != updatedUserRoom.roomid)
                {
                    return BadRequest("IDs in URL do not match IDs in body.");
                }

                var result = await _userRoomService.Update(userId, roomId, updatedUserRoom);
                
                if (result == null)
                {
                    return NotFound("UserRoom relationship not found.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{userId}/{roomId}")]
        [Authorize]
        public IActionResult Delete(int userId, int roomId)
        {
            try
            {
                if (!CanAccessUserResource(userId))
                {
                    return Forbid();
                }

                var result = _userRoomService.Delete(userId, roomId);
                
                if (result == 0)
                {
                    return NotFound("UserRoom relationship not found or could not be deleted.");
                }

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out userId);
        }

        private bool IsAdmin()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value == Roles.userMaster;
        }

        private bool CanAccessUserResource(int userId)
        {
            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return false;
            }

            return currentUserId == userId || IsAdmin();
        }
    }
}
