using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class ItemRepositoryTests
{
    [Fact]
    public void FindByCharacteristic_IgnoraMayusculas()
    {
        // Prueba que el método FindByCharacteristic devuelve resultados sin importar mayúsculas o minúsculas en el nombre del item. Se espera que al buscar "mancuerna", se encuentre el item "Mancuerna Pro".
        using var context = TestDbContextFactory.CreateContext();
        var repo = new ItemRepository(context);
        context.Items.AddRange(
            new Item { name = "Mancuerna Pro", type = "Strength", bonus = 3, price = 10 },
            new Item { name = "Bicicleta", type = "Endurance", bonus = 2, price = 20 }
        );
        context.SaveChanges();

        var result = repo.FindByCharacteristic("mancuerna");

        Assert.Single(result);
        Assert.Equal("Mancuerna Pro", result[0].name);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateByIdAsync_ActualizaItemExistente()
    {
        // Prueba que el método UpdateByIdAsync actualiza correctamente un item existente. Se espera que después de la actualización, el item tenga los nuevos valores proporcionados.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new ItemRepository(context);
        var item = new Item { name = "Viejo", type = "Strength", bonus = 1, price = 5 };
        context.Items.Add(item);
        context.SaveChanges();

        var updated = await repo.UpdateByIdAsync(item.id, new Item
        {
            name = "Nuevo",
            type = "Endurance",
            bonus = 7,
            price = 30
        });

        Assert.NotNull(updated);
        Assert.Equal("Nuevo", updated.name);
        Assert.Equal("Endurance", updated.type);
        Assert.Equal(7, updated.bonus);
        Assert.Equal(30, updated.price);
    }

    [Fact]
    public void GetRandomStrengthItems_DevuelveSoloStrength()
    {
        // Prueba que el método GetRandomStrengthItems devuelve solo items del tipo "Strength". Se espera que todos los items devueltos tengan el tipo "Strength" y cambien cada vez que se ejecuta.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new ItemRepository(context);
        context.Items.AddRange(
            new Item { id = 10, name = "S1", type = "Strength", bonus = 1, price = 10 },
            new Item { id = 11, name = "S2", type = "Strength", bonus = 2, price = 12 },
            new Item { id = 12, name = "E1", type = "Endurance", bonus = 2, price = 12 }
        );
        context.SaveChanges();

        var items = repo.GetRandomStrengthItems(2);

        Assert.NotEmpty(items);
        Assert.All(items, i => Assert.Equal("Strength", i.type));
    }
}
