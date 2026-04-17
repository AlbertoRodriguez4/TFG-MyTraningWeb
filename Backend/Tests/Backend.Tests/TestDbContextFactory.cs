using AA2_CS.Database;
using AA2_CS.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

internal static class TestDbContextFactory
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"backend-tests-{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static User CrearUsuarioBasico(string email)
    {
        return new User
        {
            name = "Usuario Test",
            email = email,
            passwordhash = "hash",
            role = Roles.userNormal,
            level = 1,
            strength = 10,
            endurance = 10,
            gold = 200,
            experience = 0
        };
    }
}
