using AA2_CS.Model;
using AA2_CS.Repository;

namespace Backend.Tests;

public class UserRepositoryTests
{
    [Fact]
    public void Register_HasheaPassword()
    {
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
