using AA2_CS.Model;
using AA2_CS.Repository;

namespace Backend.Tests;

public class PurchaseRepositoryTests
{
    [Fact]
    public void Add_ConSaldoSuficiente_DescuentaOroYCreaCompra()
    {
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
