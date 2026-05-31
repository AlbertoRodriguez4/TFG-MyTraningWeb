using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using AA2_CS.Repository;
using AA2_CS.Service;

namespace Backend.Tests;

public class NotificationPreferenceServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task GetByUserIdAsync_SiNoExiste_CreaPreferenciasPorDefecto()
    {
        // Prueba que al llamar a GetByUserIdAsync con un ID de usuario que no tiene preferencias de notificación, se crean y devuelven las preferencias por defecto. Se espera que el DTO devuelto tenga los valores predeterminados establecidos.
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
        // Prueba que al llamar a UpdateAsync con un DTO que tiene InactivityDays menor a 1, el servicio ajusta ese valor al mínimo permitido (1). Se espera que después de la actualización, el DTO devuelto tenga InactivityDays igual a 1.
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
        // Prueba que al llamar a UpdateAsync con un DTO que tiene InactivityDays mayor a 30, el servicio ajusta ese valor al máximo permitido (30). Se espera que después de la actualización, el DTO devuelto tenga InactivityDays igual a 30.
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
        // Prueba que al llamar a ResetDefaultsAsync para un usuario, se restauran los valores de preferencias de notificación a sus valores iniciales por defecto. Se espera que después de la llamada, el DTO devuelto tenga los valores predeterminados establecidos.
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
