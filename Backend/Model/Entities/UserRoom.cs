namespace AA2_CS.Model.Entities
{
    public class UserRoom
    {
        public int userid { get; set; }
        public virtual User? User { get; set; }
        public int roomid { get; set; }
        public virtual Room? Room { get; set; }

        public UserRoom() { }

        public UserRoom(int userId, int roomId)
        {
            this.userid = userId;
            this.roomid = roomId;
        }
    }
}
