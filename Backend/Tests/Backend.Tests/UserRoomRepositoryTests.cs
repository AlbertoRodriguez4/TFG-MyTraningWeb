using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class UserRoomRepositoryTests
{
    [Fact]
    public void AddYFindByCompositeKeyYDelete_FuncionaCorrectamente()
    {
        // Prueba que al agregar una nueva relación entre usuario y sala, luego buscarla por su clave compuesta (userId y roomId) y finalmente eliminarla, se realizan correctamente todas las operaciones. Se espera que después de agregar la relación, se pueda encontrar utilizando la clave compuesta, y que después de eliminarla, ya no se pueda encontrar.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRoomRepository(context);

        var user = TestDbContextFactory.CrearUsuarioBasico("userroom@test.com");
        var room = new Room { name = "Sala", minlevel = 1, minstats = 1, minconsistency = 0, description = "d", date = "x", localization = "l" };
        context.Users.Add(user);
        context.Rooms.Add(room);
        context.SaveChanges();

        var added = repo.Add(new UserRoom { userid = user.id, roomid = room.id });
        var found = repo.FindByCompositeKey(user.id, room.id);
        var deleted = repo.Delete(user.id, room.id);

        Assert.Equal(1, added);
        Assert.NotNull(found);
        Assert.Equal(1, deleted);
    }
}
