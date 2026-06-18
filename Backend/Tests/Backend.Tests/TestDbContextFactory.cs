using AA2_CS.Database;
using AA2_CS.Model.Entities;
using AA2_CS.Model.Common;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

internal static class TestDbContextFactory
{
    public static AppDbContext CreateContext()
    {
        // Este método crea y devuelve una instancia de AppDbContext configurada para usar una base de datos en memoria única para cada prueba. Se espera que cada llamada a este método proporcione un contexto limpio e independiente, lo que permite realizar pruebas sin interferencias entre ellas. La base de datos se asegura de ser creada antes de ser devuelta.
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"backend-tests-{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static User CrearUsuarioBasico(string email)
    {
        // Este método crea y devuelve un objeto User con propiedades predeterminadas para ser utilizado en las pruebas. Se espera que el usuario creado tenga un nivel inicial, atributos básicos de fuerza y resistencia, una cantidad de oro y experiencia iniciales, y que su correo electrónico sea el proporcionado como argumento. Este método facilita la creación de usuarios de prueba con una configuración estándar.
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
