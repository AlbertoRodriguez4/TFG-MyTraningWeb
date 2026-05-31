using AA2_CS.Model.Entities;

namespace AA2_CS.Model.DTOs
{
    public class UserRoomDTO
    {
        public Room room { get; set; }
        public List<UserDTO>? users { get; set; }
        public int userid { get; set; }
    }
}
