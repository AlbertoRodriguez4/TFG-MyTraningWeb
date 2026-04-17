using AA2_CS.Model;
using AA2_CS.Repository;

namespace Backend.Tests;

public class SubscriptionRepositoryTests
{
    [Fact]
    public void Add_SinUsuario_LanzaExcepcion()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);

        var subscription = new Subscription
        {
            userid = 999,
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddMonths(1),
            isActive = true
        };

        Assert.Throws<ArgumentException>(() => repo.Add(subscription));
    }

    [Fact]
    public void Add_YFindByUserId_DevuelveSuscripcionActiva()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);

        var user = TestDbContextFactory.CrearUsuarioBasico("sub1@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        repo.Add(new Subscription
        {
            userid = user.id,
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddMonths(1),
            isActive = true,
            planType = "Premium",
            monthlyPrice = 10m
        });

        var active = repo.FindByUserId(user.id);

        Assert.NotNull(active);
        Assert.True(active.isActive);
    }

    [Fact]
    public void RenewSubscription_ExtiendeFechaFin()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);
        var user = TestDbContextFactory.CrearUsuarioBasico("sub2@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        repo.Add(new Subscription
        {
            userid = user.id,
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddDays(10),
            isActive = true
        });
        var before = repo.FindByUserId(user.id)!.endDate;

        repo.RenewSubscription(user.id, 1);
        var after = repo.FindByUserId(user.id)!.endDate;

        Assert.True(after > before);
    }

    [Fact]
    public void CancelSubscription_Existente_Devuelve1YDesactiva()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);
        var user = TestDbContextFactory.CrearUsuarioBasico("sub3@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        var id = repo.Add(new Subscription
        {
            userid = user.id,
            startDate = DateTime.UtcNow,
            endDate = DateTime.UtcNow.AddMonths(1),
            isActive = true
        });

        var result = repo.CancelSubscription(id);
        var cancelled = repo.FindById(id)!;

        Assert.Equal(1, result);
        Assert.False(cancelled.isActive);
    }

    [Fact]
    public void DeactivateExpiredSubscriptions_DesactivaSoloExpiradas()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);
        var user = TestDbContextFactory.CrearUsuarioBasico("sub4@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        context.Subscriptions.AddRange(
            new Subscription
            {
                userid = user.id,
                startDate = DateTime.UtcNow.AddMonths(-2),
                endDate = DateTime.UtcNow.AddDays(-1),
                isActive = true
            },
            new Subscription
            {
                userid = user.id,
                startDate = DateTime.UtcNow,
                endDate = DateTime.UtcNow.AddDays(7),
                isActive = true
            }
        );
        context.SaveChanges();

        var deactivated = repo.DeactivateExpiredSubscriptions();
        var activeCount = repo.FindAllActive().Count;

        Assert.Equal(1, deactivated);
        Assert.Equal(1, activeCount);
    }
}
