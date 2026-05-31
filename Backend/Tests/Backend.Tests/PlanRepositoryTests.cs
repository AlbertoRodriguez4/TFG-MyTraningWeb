using AA2_CS.Model.Entities;
using AA2_CS.Repository;

namespace Backend.Tests;

public class PlanRepositoryTests
{
    [Fact]
    public void AddYFindById_FuncionaCorrectamente()
    {
        // Prueba que al agregar un nuevo plan y luego buscarlo por su ID, se obtiene el registro correcto. Se espera que el plan encontrado tenga la misma descripción que el plan agregado.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PlanRepository(context);

        var id = repo.Add(new Plan { userid = 1, description = "Plan fuerza" });
        var found = repo.FindById(id);

        Assert.NotNull(found);
        Assert.Equal("Plan fuerza", found.description);
    }

    [Fact]
    public void FindByCharacteristic_BuscaSinMayusculas()
    {
        // Prueba que el método FindByCharacteristic devuelve resultados sin importar mayúsculas o minúsculas en el nombre del plan. Se espera que al buscar "cardio", se encuentre el plan "Cardio diario".
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PlanRepository(context);
        repo.Add(new Plan { userid = 1, description = "Cardio diario" });
        repo.Add(new Plan { userid = 1, description = "Fuerza semanal" });

        var result = repo.FindByCharacteristic("cardio");

        Assert.Single(result);
        Assert.Equal("Cardio diario", result[0].description);
    }

    [Fact]
    public void Update_ModificaDescripcion()
    {
        // Prueba que el método Update modifica correctamente la descripción de un plan existente. Se espera que después de la actualización, el plan tenga la nueva descripción proporcionada.
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PlanRepository(context);
        var id = repo.Add(new Plan { userid = 1, description = "Vieja" });

        var updated = repo.Update(new Plan { id = id, userid = 1, description = "Nueva" });
        var plan = repo.FindById(id);

        Assert.Equal(1, updated);
        Assert.Equal("Nueva", plan.description);
    }

    [Fact]
    public void Delete_Existente_EliminaRegistro()
    {
        using var context = TestDbContextFactory.CreateContext();
        var repo = new PlanRepository(context);
        var id = repo.Add(new Plan { userid = 1, description = "Eliminar" });

        var deleted = repo.Delete(new Plan { id = id });

        Assert.Equal(1, deleted);
        Assert.Null(repo.FindById(id));
    }
}
