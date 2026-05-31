using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class RoomRepositoryTests
{
    [Fact]
    public void CreateRoomWithUser_CreaSalaYRelacionUserRoom()
    {
        // Prueba que el método CreateRoomWithUser crea una nueva sala y establece correctamente la relación entre el usuario y la sala en la tabla UserRooms. Se espera que se cree un nuevo registro en la tabla de salas, un nuevo registro en la tabla de relaciones UserRooms y que el ID de la sala devuelto sea mayor a 0.
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

        var roomId = repo.CreateRoomWithUser(room, user.id, "user");

        Assert.True(roomId > 0);
        Assert.Single(context.Rooms);
        Assert.Single(context.UserRooms);
        Assert.Equal(user.id, context.UserRooms.First().userid);
    }

    [Fact]
    public void SortByLevelAsc_OrdenaCorrectamente()
    {
        // Prueba que el método SortByLevelAsc devuelve las salas ordenadas correctamente por el nivel mínimo en orden ascendente. Se espera que la lista devuelta tenga las salas ordenadas de menor a mayor nivel mínimo.
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
        //  Prueba que el método FindAllWithUsers devuelve una lista de salas junto con los usuarios asociados a cada sala. Se espera que la lista devuelta contenga las salas con sus respectivos usuarios correctamente relacionados.
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
