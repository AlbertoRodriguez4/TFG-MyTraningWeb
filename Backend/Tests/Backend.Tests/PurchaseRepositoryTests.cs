using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class PurchaseRepositoryTests
{
    [Fact]
    public void Add_ConSaldoSuficiente_DescuentaOroYCreaCompra()
    {
        // Prueba que al agregar una compra con un usuario que tiene suficiente oro, se descuenta el oro del usuario y se crea el registro de compra. Se espera que el ID de la compra sea mayor a 0, el oro del usuario se reduzca correctamente y se cree un registro en la tabla de compras.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PurchaseRepository(context);

        var user = TestDbContextFactory.CrearUsuarioBasico("purchase1@test.com");
        user.gold = 100;
        var item = new Item { name = "Item 1", type = "Strength", bonus = 5, price = 40 };
        context.Users.Add(user);
        context.Items.Add(item);
        context.SaveChanges();

        var purchase = new Purchase { userid = user.id, itemid = item.id, purchasedate = DateTime.Now };
        var id = repo.Add(purchase);

        var updatedUser = context.Users.First(u => u.id == user.id);
        Assert.True(id > 0);
        Assert.Equal(60, updatedUser.gold);
        Assert.Single(context.Purchases);
    }

    [Fact]
    public void Add_ConSaldoInsuficiente_LanzaExcepcion()
    {
        // Prueba que al agregar una compra con un usuario que no tiene suficiente oro, se lanza una excepción y no se crea el registro de compra. Se espera que se lance una InvalidOperationException y que no se cree ningún registro en la tabla de compras.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PurchaseRepository(context);

        var user = TestDbContextFactory.CrearUsuarioBasico("purchase2@test.com");
        user.gold = 10;
        var item = new Item { name = "Item caro", type = "Endurance", bonus = 10, price = 200 };
        context.Users.Add(user);
        context.Items.Add(item);
        context.SaveChanges();

        var purchase = new Purchase { userid = user.id, itemid = item.id, purchasedate = DateTime.UtcNow };

        Assert.Throws<InvalidOperationException>(() => repo.Add(purchase));
    }

    [Fact]
    public void FindByUserId_DevuelveDTOConDatosDelItemYUsuario()
    {
        // Prueba que el método FindByUserId devuelve una lista de DTOs que contienen los datos del item comprado y el email del usuario. Se espera que la lista devuelta tenga un solo elemento con el nombre del item y el email del usuario correctos.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PurchaseRepository(context);

        var user = TestDbContextFactory.CrearUsuarioBasico("purchase3@test.com");
        var item = new Item { name = "Botas", type = "Endurance", bonus = 3, price = 20, imageUrl = "img.png" };
        context.Users.Add(user);
        context.Items.Add(item);
        context.SaveChanges();
        context.Purchases.Add(new Purchase { userid = user.id, itemid = item.id, purchasedate = DateTime.UtcNow });
        context.SaveChanges();

        var list = repo.FindByUserId(user.id);

        Assert.Single(list);
        Assert.Equal("Botas", list[0].ItemName);
        Assert.Equal("purchase3@test.com", list[0].UserEmail);
    }
}
