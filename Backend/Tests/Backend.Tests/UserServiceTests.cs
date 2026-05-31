using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using AA2_CS.Repository;
using AA2_CS.Service;

namespace Backend.Tests;

public class UserServiceTests
{
    [Fact]
    public void GetXpRequiredForNextLevel_Nivel1_Devuelve100()
    {
        // Prueba que al llamar a GetXpRequiredForNextLevel con el nivel 1, se devuelve la cantidad correcta de experiencia requerida para alcanzar el siguiente nivel. Se espera que el resultado devuelto sea 100, lo que indica que se necesitan 100 puntos de experiencia para subir del nivel 1 al nivel 2.
        var service = new UserService(null!, null!);

        var result = service.GetXpRequiredForNextLevel(1);

        Assert.Equal(100, result);
    }

    [Fact]
    public void GetXpRequiredForNextLevel_Nivel4_Devuelve800()
    {
        // Prueba que al llamar a GetXpRequiredForNextLevel con el nivel 4, se devuelve la cantidad correcta de experiencia requerida para alcanzar el siguiente nivel. Se espera que el resultado devuelto sea 800, lo que indica que se necesitan 800 puntos de experiencia para subir del nivel 4 al nivel 5.
        var service = new UserService(null!, null!);

        var result = service.GetXpRequiredForNextLevel(4);

        Assert.Equal(800, result);
    }

    [Fact]
    public void AddExperience_UsuarioInexistente_NoLanzaExcepcion()
    {
        // Prueba que al intentar agregar experiencia a un usuario que no existe en la base de datos, el método AddExperience no lanza una excepción. Se espera que el método maneje correctamente la situación sin generar errores, incluso cuando se proporciona un ID de usuario que no corresponde a ningún registro existente.
        using var context = TestDbContextFactory.CreateContext();
        var userService = new UserService(new UserRepository(context), new PurchaseRepository(context));

        userService.AddExperience(999, 100);
    }

    [Fact]
    public void AddExperience_CuandoSubeDeNivel_ActualizaStatsYOro()
    {
        // Prueba que al agregar experiencia a un usuario que alcanza el umbral para subir de nivel, el método AddExperience actualiza correctamente el nivel del usuario, su experiencia restante, su oro y su fuerza. Se espera que después de agregar experiencia suficiente para subir del nivel 1 al nivel 2, el usuario tenga un nivel actualizado a 2, una experiencia restante de 150 (250 - 100), un oro incrementado en 100 y una fuerza incrementada en 1.
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
        // Prueba que al intentar equipar un item que el usuario no ha comprado, el método EquipItem devuelve un mensaje de error indicando que el usuario debe comprar el item primero. Se espera que el resultado devuelto sea "No posees este objeto, debes comprarlo primero.", lo que indica que el sistema está validando correctamente la propiedad de los items antes de permitir equiparlos.
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
        // Prueba que al equipar un item de tipo fuerza que el usuario ha comprado, el método EquipItem actualiza correctamente el item equipado en la propiedad correspondiente del usuario. Se espera que después de equipar el item, la propiedad equippedStrengthId del usuario se actualice con el ID del item equipado, y que el resultado devuelto sea "Item equipped successfully", indicando que el proceso de equipar el item se realizó correctamente.
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
        // Prueba que al intentar desequipar un item proporcionando un tipo de equipo inválido, el método UnequipItem devuelve un mensaje de error indicando que el tipo de equipo es inválido. Se espera que el resultado devuelto sea "Tipo de equipo inválido.", lo que indica que el sistema está validando correctamente los tipos de equipo antes de permitir desequiparlos.
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
        // Prueba que al desequipar un item de tipo fuerza, el método UnequipItem actualiza correctamente la propiedad correspondiente del usuario para reflejar que ya no tiene un item equipado en esa categoría. Se espera que después de desequipar el item, la propiedad equippedStrengthId del usuario se establezca en null, y que el resultado devuelto sea "Item unequipped successfully", indicando que el proceso de desequipar el item se realizó correctamente.
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
