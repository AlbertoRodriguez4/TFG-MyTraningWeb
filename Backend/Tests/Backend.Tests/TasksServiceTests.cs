using AA2_CS.Model.Entities;
using AA2_CS.Repository;
using AA2_CS.Service;
using ModelTask = AA2_CS.Model.Task;

namespace Backend.Tests;

public class TasksServiceTests
{
    [Fact]
    public async System.Threading.Tasks.Task CompleteTask_SiNoExiste_DevuelveTaskNotFound()
    {
        // Prueba que al intentar completar una tarea que no existe en la base de datos, el método CompleteTask devuelve un mensaje indicando que la tarea no fue encontrada. Se espera que el resultado devuelto sea "Task not found".
        using var context = TestDbContextFactory.CreateContext();
        var service = CrearService(context);

        var result = await service.CompleteTask(999);

        Assert.Equal("Task not found", result);
    }

    [Fact]
    public async System.Threading.Tasks.Task CompleteTask_SiYaEstaCompletada_DevuelveMensaje()
    {
        // Prueba que al intentar completar una tarea que ya está marcada como completada, el método CompleteTask devuelve un mensaje indicando que la tarea ya fue completada. Se espera que el resultado devuelto sea "Task is already completed".
        using var context = TestDbContextFactory.CreateContext();
        var user = TestDbContextFactory.CrearUsuarioBasico("task1@test.com");
        context.Users.Add(user);
        context.SaveChanges();
        var task = new ModelTask
        {
            name = "Task",
            description = "Desc",
            reward = 50,
            difficulty = 1,
            trainingfocus = "strength",
            userId = user.id,
            iscompleted = true,
            createdat = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        context.SaveChanges();

        var service = CrearService(context);
        var result = await service.CompleteTask(task.id);

        Assert.Equal("Task is already completed", result);
    }

    [Fact]
    public async System.Threading.Tasks.Task CompleteTask_SiUsuarioNoExiste_DevuelveUserNotFound()
    {
        // Prueba que al intentar completar una tarea cuyo usuario asociado no existe en la base de datos, el método CompleteTask devuelve un mensaje indicando que el usuario no fue encontrado. Se espera que el resultado devuelto sea "User not found". 
        using var context = TestDbContextFactory.CreateContext();
        var task = new ModelTask
        {
            name = "Task",
            description = "Desc",
            reward = 50,
            difficulty = 1,
            trainingfocus = "strength",
            userId = 999,
            iscompleted = false,
            createdat = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        context.SaveChanges();

        var service = CrearService(context);
        var result = await service.CompleteTask(task.id);

        Assert.Equal("User not found", result);
    }

    [Fact]
    public async System.Threading.Tasks.Task CompleteTask_Strength_AplicaRecompensasYOroYXp()
    {
        // Prueba que al completar una tarea de entrenamiento de fuerza, se aplican correctamente las recompensas al usuario, incluyendo el aumento de fuerza, la ganancia de oro y experiencia, y la actualización de la racha de consistencia. Se espera que después de completar la tarea, el usuario tenga un aumento en su atributo de fuerza, una cantidad específica de oro y experiencia ganados, y que su racha de consistencia se incremente si la tarea anterior también fue completada consecutivamente.
        using var context = TestDbContextFactory.CreateContext();
        var user = TestDbContextFactory.CrearUsuarioBasico("task2@test.com");
        user.strength = 10;
        user.gold = 0;
        user.experience = 0;
        user.consistencystreak = 0;
        context.Users.Add(user);
        context.SaveChanges();
        var task = new ModelTask
        {
            name = "Task strength",
            description = "Desc",
            reward = 50,
            difficulty = 1,
            trainingfocus = "strength",
            userId = user.id,
            iscompleted = false,
            createdat = DateTime.UtcNow
        };
        context.Tasks.Add(task);
        context.SaveChanges();

        var service = CrearService(context);
        var result = await service.CompleteTask(task.id);

        var updatedUser = context.Users.First(u => u.id == user.id);
        var updatedTask = context.Tasks.First(t => t.id == task.id);
        Assert.Equal("Task completed and rewards applied successfully", result);
        Assert.True(updatedTask.iscompleted);
        Assert.Equal(15, updatedUser.strength);
        Assert.Equal(50, updatedUser.gold);
        Assert.Equal(50, updatedUser.experience);
        Assert.Equal(1, updatedUser.consistencystreak);
    }

    [Fact]
    public async System.Threading.Tasks.Task CompleteTask_ConTareaPreviaConsecutiva_IncrementaRacha()
    {
        // Prueba que al completar una tarea con una tarea previa completada de forma consecutiva, la racha de consistencia del usuario se incrementa correctamente. Se espera que después de completar la tarea actual, la racha de consistencia del usuario sea mayor que antes, reflejando la continuidad en la realización de tareas.
        using var context = TestDbContextFactory.CreateContext();
        var user = TestDbContextFactory.CrearUsuarioBasico("task3@test.com");
        user.consistencystreak = 1;
        context.Users.Add(user);
        context.SaveChanges();

        context.Tasks.Add(new ModelTask
        {
            name = "Ayer",
            description = "Desc",
            reward = 20,
            difficulty = 1,
            trainingfocus = "endurance",
            userId = user.id,
            iscompleted = true,
            createdat = DateTime.UtcNow.Date.AddDays(-1)
        });

        var taskActual = new ModelTask
        {
            name = "Hoy",
            description = "Desc",
            reward = 20,
            difficulty = 1,
            trainingfocus = "endurance",
            userId = user.id,
            iscompleted = false,
            createdat = DateTime.UtcNow.Date
        };
        context.Tasks.Add(taskActual);
        context.SaveChanges();

        var service = CrearService(context);
        await service.CompleteTask(taskActual.id);

        var updated = context.Users.First(u => u.id == user.id);
        Assert.Equal(2, updated.consistencystreak);
    }

    private static TasksService CrearService(AA2_CS.Database.AppDbContext context)
    {
        // Método auxiliar para crear una instancia de TasksService con los repositorios y servicios necesarios. Se espera que este método configure correctamente las dependencias para permitir la ejecución de las pruebas relacionadas con la gestión de tareas.
        var tasksRepository = new TasksRepository(context);
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);
        var achievementRepository = new AchievementRepository(context);
        var achievementService = new AchievementService(achievementRepository, userService);
        return new TasksService(tasksRepository, userRepository, userService, achievementService);
    }
}
