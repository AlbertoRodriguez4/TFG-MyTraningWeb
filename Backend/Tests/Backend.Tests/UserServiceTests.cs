using AA2_CS.Model;
using AA2_CS.Repository;
using AA2_CS.Service;

namespace Backend.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetXpRequiredForNextLevel_Nivel1_Devuelve100()
    {
        var service = new UserService(null!, null!);

        var result = service.GetXpRequiredForNextLevel(1);

        Assert.Equal(100, result);
    }

    [Fact]
    public void GetXpRequiredForNextLevel_Nivel4_Devuelve800()
    {
        var service = new UserService(null!, null!);

        var result = service.GetXpRequiredForNextLevel(4);

        Assert.Equal(800, result);
    }

    [Fact]
    public void AddExperience_UsuarioInexistente_NoLanzaExcepcion()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userService = new UserService(new UserRepository(context), new PurchaseRepository(context));

        userService.AddExperience(999, 100);
    }

    [Fact]
    public void AddExperience_CuandoSubeDeNivel_ActualizaStatsYOro()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);

        var user = new User
        {
            name = "User XP",
            email = "xp@test.com",
            passwordhash = "hash",
            role = Roles.userNormal,
            level = 1,
            strength = 0,
            gold = 0,
            experience = 0
        };
        context.Users.Add(user);
        context.SaveChanges();

        userService.AddExperience(user.id, 250);

        var updated = userRepository.FindById(user.id);
        Assert.NotNull(updated);
        Assert.Equal(2, updated.level);
        Assert.Equal(150, updated.experience);
        Assert.Equal(100, updated.gold);
        Assert.Equal(1, updated.strength);
    }

    [Fact]
    public void EquipItem_CuandoNoEstaComprado_DevuelveMensajeError()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);

        var user = TestDbContextFactory.CrearUsuarioBasico("equip@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        var result = userService.EquipItem(user.id, itemId: 99);

        Assert.Equal("No posees este objeto, debes comprarlo primero.", result);
    }

    [Fact]
    public void EquipItem_ConCompraStrength_EquipaItem()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);

        var user = TestDbContextFactory.CrearUsuarioBasico("equip2@test.com");
        var item = new Item { name = "Mancuerna", type = "strength", bonus = 3, price = 10 };
        context.Users.Add(user);
        context.Items.Add(item);
        context.SaveChanges();
        context.Purchases.Add(new Purchase { userid = user.id, itemid = item.id, purchasedate = DateTime.UtcNow });
        context.SaveChanges();

        var result = userService.EquipItem(user.id, item.id);

        var updated = userRepository.FindById(user.id);
        Assert.Equal("Item equipped successfully", result);
        Assert.Equal(item.id, updated.equippedStrengthId);
    }

    [Fact]
    public void UnequipItem_TipoInvalido_DevuelveMensajeError()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);

        var user = TestDbContextFactory.CrearUsuarioBasico("unequip@test.com");
        context.Users.Add(user);
        context.SaveChanges();

        var result = userService.UnequipItem(user.id, "otro");

        Assert.Equal("Tipo de equipo inválido.", result);
    }

    [Fact]
    public void UnequipItem_Strength_BorraEquipado()
    {
        using var context = TestDbContextFactory.CreateContext();
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);

        var user = TestDbContextFactory.CrearUsuarioBasico("unequip2@test.com");
        user.equippedStrengthId = 12;
        context.Users.Add(user);
        context.SaveChanges();

        var result = userService.UnequipItem(user.id, "strength");

        var updated = userRepository.FindById(user.id);
        Assert.Equal("Item unequipped successfully", result);
        Assert.Null(updated.equippedStrengthId);
    }
}
