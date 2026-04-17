using AA2_CS.Model;
using AA2_CS.Repository;

namespace Backend.Tests;

public class EmailVerificationRepositoryTests
{
    [Fact]
    public async System.Threading.Tasks.Task AddAsyncYGetValidVerificationAsync_DevuelveRegistroVigente()
    {
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
