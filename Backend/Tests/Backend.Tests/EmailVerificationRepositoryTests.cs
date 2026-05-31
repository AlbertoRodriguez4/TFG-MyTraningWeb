using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class EmailVerificationRepositoryTests
{
    [Fact]
    public async System.Threading.Tasks.Task AddAsyncYGetValidVerificationAsync_DevuelveRegistroVigente()
    {
        // Prueba que al agregar una verificación de email y luego buscarla con GetValidVerificationAsync, se obtiene el registro correcto si no ha expirado ni ha sido usado. Se espera que devuelva el registro agregado.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new EmailVerificationRepository(context);
        var verification = new EmailVerification
        {
            userid = 1,
            code = "123456",
            createdat = DateTime.UtcNow,
            expiresat = DateTime.UtcNow.AddMinutes(10),
            isused = false
        };
        await repo.AddAsync(verification);

        var found = await repo.GetValidVerificationAsync(1, "123456");

        Assert.NotNull(found);
        Assert.Equal("123456", found.code);
    }

    [Fact]
    public async System.Threading.Tasks.Task InvalidatePreviousAsync_MarcaComoUsadasLasNoUsadas()
    {
        // Prueba que al llamar a InvalidatePreviousAsync para un usuario, todas las verificaciones de email no usadas para ese usuario se marcan como usadas. Se espera que después de la llamada, no queden verificaciones no usadas para ese usuario.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new EmailVerificationRepository(context);
        context.EmailVerifications.AddRange(
            new EmailVerification { userid = 2, code = "111111", expiresat = DateTime.UtcNow.AddMinutes(10), isused = false },
            new EmailVerification { userid = 2, code = "222222", expiresat = DateTime.UtcNow.AddMinutes(10), isused = false }
        );
        context.SaveChanges();

        await repo.InvalidatePreviousAsync(2);
        var pending = context.EmailVerifications.Where(v => v.userid == 2 && !v.isused).ToList();

        Assert.Empty(pending);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteExpiredAsync_EliminaSoloCaducados()
    {
        // Prueba que al llamar a DeleteExpiredAsync para un usuario, solo se eliminan las verificaciones de email que han expirado. Se espera que después de la llamada, solo queden las verificaciones no expiradas para ese usuario.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new EmailVerificationRepository(context);
        context.EmailVerifications.AddRange(
            new EmailVerification { userid = 3, code = "exp", expiresat = DateTime.UtcNow.AddMinutes(-1), isused = false },
            new EmailVerification { userid = 3, code = "ok", expiresat = DateTime.UtcNow.AddMinutes(10), isused = false }
        );
        context.SaveChanges();

        await repo.DeleteExpiredAsync(3);

        Assert.Single(context.EmailVerifications.Where(v => v.userid == 3));
        Assert.Equal("ok", context.EmailVerifications.First(v => v.userid == 3).code);
    }
}
