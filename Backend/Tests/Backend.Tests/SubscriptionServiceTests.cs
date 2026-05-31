using AA2_CS.Repository;
using AA2_CS.Service;

namespace Backend.Tests;

public class SubscriptionServiceTests
{
    [Fact]
    public void PurchaseSubscription_SiYaExisteActiva_LanzaExcepcion()
    {
        // Prueba que al intentar comprar una suscripción para un usuario que ya tiene una suscripción activa, se lanza una excepción. Se espera que se lance una InvalidOperationException indicando que el usuario ya tiene una suscripción activa.
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
        // Prueba que al comprar una suscripción para un usuario sin suscripción activa, se crea una nueva suscripción activa correctamente. Se espera que el ID de la suscripción devuelta sea mayor a 0, que la suscripción obtenida por GetActiveSubscription no sea nula y que esté marcada como activa.
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
