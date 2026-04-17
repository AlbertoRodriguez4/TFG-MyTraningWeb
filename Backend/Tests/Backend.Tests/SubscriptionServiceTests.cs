using AA2_CS.Repository;
using AA2_CS.Service;

namespace Backend.Tests;

public class SubscriptionServiceTests
{
    [Fact]
    public void PurchaseSubscription_SiYaExisteActiva_LanzaExcepcion()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);
        var service = new SubscriptionService(repo);
        var user = TestDbContextFactory.CrearUsuarioBasico("subservice@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        service.PurchaseSubscription(user.id);

        Assert.Throws<InvalidOperationException>(() => service.PurchaseSubscription(user.id));
    }

    [Fact]
    public void PurchaseSubscription_CreaSuscripcionActiva()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new SubscriptionRepository(context);
        var service = new SubscriptionService(repo);
        var user = TestDbContextFactory.CrearUsuarioBasico("subservice2@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        var id = service.PurchaseSubscription(user.id);
        var active = service.GetActiveSubscription(user.id);

        Assert.True(id > 0);
        Assert.NotNull(active);
        Assert.True(active!.IsActive);
    }
}
