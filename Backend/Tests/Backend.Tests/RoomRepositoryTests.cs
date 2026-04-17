using AA2_CS.Model;
using AA2_CS.Repository;

namespace Backend.Tests;

public class RoomRepositoryTests
{
    [Fact]
    public void CreateRoomWithUser_CreaSalaYRelacionUserRoom()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new RoomRepository(context);
        var user = TestDbContextFactory.CrearUsuarioBasico("room1@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        var room = new Room
        {
            name = "Sala 1",
            minlevel = 1,
            minstats = 10,
            minconsistency = 1,
            description = "Desc",
            date = "2026-01-01",
            localization = "Madrid"
        };

        var roomId = repo.CreateRoomWithUser(room, user.id);

        Assert.True(roomId > 0);
        Assert.Single(context.Rooms);
        Assert.Single(context.UserRooms);
        Assert.Equal(user.id, context.UserRooms.First().userid);
    }

    [Fact]
    public void SortByLevelAsc_OrdenaCorrectamente()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new RoomRepository(context);
        context.Rooms.AddRange(
            new Room { name = "B", minlevel = 5, minstats = 20, minconsistency = 1, description = "d", date = "x", localization = "l" },
            new Room { name = "A", minlevel = 1, minstats = 10, minconsistency = 1, description = "d", date = "x", localization = "l" }
        );
        context.SaveChanges();

        var sorted = repo.SortByLevelAsc();

        Assert.Equal(1, sorted[0].minlevel);
        Assert.Equal(5, sorted[1].minlevel);
    }

    [Fact]
    public void FindAllWithUsers_DevuelveSalasConUsuarios()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new RoomRepository(context);
        var user = TestDbContextFactory.CrearUsuarioBasico("room2@test.com");
        context.Users.Add(user);
        var room = new Room { name = "Sala x", minlevel = 1, minstats = 10, minconsistency = 0, description = "d", date = "x", localization = "l" };
        context.Rooms.Add(room);
        context.SaveChanges();
        context.UserRooms.Add(new UserRoom { userid = user.id, roomid = room.id });
        context.SaveChanges();

        var list = repo.FindAllWithUsers();

        Assert.Single(list);
        Assert.Equal("Sala x", list[0].room.name);
        Assert.Single(list[0].users!);
    }
}
