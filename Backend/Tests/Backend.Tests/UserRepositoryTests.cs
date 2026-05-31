using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class UserRepositoryTests
{
    [Fact]
    public void Register_HasheaPassword()
    {
        // Prueba que al registrar un nuevo usuario, la contraseña proporcionada se hashea correctamente antes de ser almacenada en la base de datos. Se espera que el hash de la contraseña no sea igual a la contraseña original y que el hash verifique correctamente la contraseña original utilizando el algoritmo de hashing BCrypt.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);

        var user = new User
        {
            name = "User Register",
            email = "register@test.com",
            passwordhash = "secreta",
            role = Roles.userNormal
        };

        var created = repo.Register(user);

        Assert.NotEqual("secreta", created.passwordhash);
        Assert.True(BCrypt.Net.BCrypt.Verify("secreta", created.passwordhash));
    }

    [Fact]
    public void Login_ConPasswordCorrecta_DevuelveUsuario()
    {
        // Prueba que al intentar iniciar sesión con un correo electrónico y contraseña correctos, el método Login devuelve el usuario correspondiente. Se espera que el usuario devuelto no sea nulo y que su correo electrónico coincida con el proporcionado en la solicitud de inicio de sesión.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);
        repo.Register(new User
        {
            name = "User Login",
            email = "loginrepo@test.com",
            passwordhash = "123456",
            role = Roles.userNormal
        });

        var logged = repo.Login("loginrepo@test.com", "123456");

        Assert.NotNull(logged);
        Assert.Equal("loginrepo@test.com", logged.email);
    }

    [Fact]
    public async System.Threading.Tasks.Task ChangePassword_ConPasswordActualIncorrecta_DevuelveFalse()
    {
        // Prueba que al intentar cambiar la contraseña de un usuario proporcionando una contraseña actual incorrecta, el método ChangePassword devuelve false y no actualiza el hash de la contraseña. Se espera que el resultado devuelto sea false, indicando que el cambio de contraseña no se realizó debido a la contraseña actual incorrecta.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);
        var created = repo.Register(new User
        {
            name = "User Pass",
            email = "pass1@test.com",
            passwordhash = "actual",
            role = Roles.userNormal
        });

        var changed = await repo.ChangePassword(created.id, "incorrecta", "nueva");

        Assert.False(changed);
    }

    [Fact]
    public async System.Threading.Tasks.Task ChangePassword_ConPasswordActualCorrecta_ActualizaHash()
    {
        // Prueba que al intentar cambiar la contraseña de un usuario proporcionando la contraseña actual correcta, el método ChangePassword devuelve true y actualiza el hash de la contraseña. Se espera que el resultado devuelto sea true, indicando que el cambio de contraseña se realizó correctamente, y que al intentar iniciar sesión con la nueva contraseña, se obtenga el usuario correspondiente, confirmando que el hash de la contraseña fue actualizado.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new UserRepository(context);
        var created = repo.Register(new User
        {
            name = "User Pass2",
            email = "pass2@test.com",
            passwordhash = "actual",
            role = Roles.userNormal
        });

        var changed = await repo.ChangePassword(created.id, "actual", "nueva");
        var relogged = repo.Login("pass2@test.com", "nueva");

        Assert.True(changed);
        Assert.NotNull(relogged);
    }
}
