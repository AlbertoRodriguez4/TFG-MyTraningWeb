using AA2_CS.Model;
using AA2_CS.Repository;
using AA2_CS.Service;

namespace Backend.Tests;

public class NotificationPreferenceServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task GetByUserIdAsync_SiNoExiste_CreaPreferenciasPorDefecto()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new NotificationPreferenceService(new NotificationPreferenceRepository(context));

        var dto = await service.GetByUserIdAsync(1);

        Assert.Equal(1, dto.UserId);
        Assert.True(dto.InactivityEnabled);
        Assert.Equal(3, dto.InactivityDays);
        Assert.True(dto.RoomsEnabled);
        Assert.True(dto.PurchasesEnabled);
        Assert.True(dto.SubscriptionExpiryEnabled);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_SiInactivityDaysEsMenorA1_AjustaAMinimo()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new NotificationPreferenceService(new NotificationPreferenceRepository(context));

        var dto = await service.UpdateAsync(2, new NotificationPreferenceDTO
        {
            UserId = 2,
            InactivityEnabled = true,
            InactivityDays = 0,
            RoomsEnabled = false,
            PurchasesEnabled = false,
            SubscriptionExpiryEnabled = true
        });

        Assert.Equal(1, dto.InactivityDays);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_SiInactivityDaysEsMayorA30_AjustaAMaximo()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new NotificationPreferenceService(new NotificationPreferenceRepository(context));

        var dto = await service.UpdateAsync(3, new NotificationPreferenceDTO
        {
            UserId = 3,
            InactivityEnabled = true,
            InactivityDays = 99,
            RoomsEnabled = true,
            PurchasesEnabled = true,
            SubscriptionExpiryEnabled = true
        });

        Assert.Equal(30, dto.InactivityDays);
    }

    [Fact]
    public async System.Threading.Tasks.Task ResetDefaultsAsync_RestauraValoresIniciales()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = new NotificationPreferenceService(new NotificationPreferenceRepository(context));
        await service.UpdateAsync(4, new NotificationPreferenceDTO
        {
            UserId = 4,
            InactivityEnabled = false,
            InactivityDays = 10,
            RoomsEnabled = false,
            PurchasesEnabled = false,
            SubscriptionExpiryEnabled = false
        });

        var dto = await service.ResetDefaultsAsync(4);

        Assert.True(dto.InactivityEnabled);
        Assert.Equal(3, dto.InactivityDays);
        Assert.True(dto.RoomsEnabled);
        Assert.True(dto.PurchasesEnabled);
        Assert.True(dto.SubscriptionExpiryEnabled);
    }
}
