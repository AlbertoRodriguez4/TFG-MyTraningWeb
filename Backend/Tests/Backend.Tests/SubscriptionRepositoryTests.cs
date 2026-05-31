using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class SubscriptionRepositoryTests
{
    [Fact]
    public void Add_SinUsuario_LanzaExcepcion()
    {
        // Prueba que al intentar agregar una suscripción para un usuario que no existe en la base de datos, se lanza una excepción. Se espera que se lance una ArgumentException indicando que el usuario no existe.
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
        // Prueba que al agregar una suscripción para un usuario existente y luego buscarla por el ID de usuario, se obtiene la suscripción activa correctamente. Se espera que la suscripción devuelta tenga el mismo ID de usuario y esté marcada como activa.
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
        // Prueba que al renovar una suscripción existente, la fecha de fin se extiende correctamente. Se espera que después de renovar la suscripción por un mes, la nueva fecha de fin sea mayor que la fecha de fin original.
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
        // Prueba que al cancelar una suscripción existente, el método devuelve 1 y la suscripción se marca como inactiva. Se espera que el resultado de la cancelación sea 1 y que la propiedad isActive de la suscripción cancelada sea false.
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
        // Prueba que el método DeactivateExpiredSubscriptions desactiva solo las suscripciones que han expirado. Se espera que después de ejecutar el método, solo las suscripciones con fecha de fin anterior a la fecha actual estén marcadas como inactivas, mientras que las suscripciones aún válidas permanezcan activas.
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
