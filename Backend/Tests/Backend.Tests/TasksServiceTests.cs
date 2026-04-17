using AA2_CS.Model;
using AA2_CS.Repository;
using AA2_CS.Service;
using ModelTask = AA2_CS.Model.Task;

namespace Backend.Tests;

public class TasksServiceTests
{
    [Fact]
    public void CompleteTask_SiNoExiste_DevuelveTaskNotFound()
    {
        using var context = TestDbContextFactory.CreateContext();
        var service = CrearService(context);

        var result = service.CompleteTask(999);

        Assert.Equal("Task not found", result);
    }

    [Fact]
    public void CompleteTask_SiYaEstaCompletada_DevuelveMensaje()
    {
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
        var result = service.CompleteTask(task.id);

        Assert.Equal("Task is already completed", result);
    }

    [Fact]
    public void CompleteTask_SiUsuarioNoExiste_DevuelveUserNotFound()
    {
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
        var result = service.CompleteTask(task.id);

        Assert.Equal("User not found", result);
    }

    [Fact]
    public void CompleteTask_Strength_AplicaRecompensasYOroYXp()
    {
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
        var result = service.CompleteTask(task.id);

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
    public void CompleteTask_ConTareaPreviaConsecutiva_IncrementaRacha()
    {
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
        service.CompleteTask(taskActual.id);

        var updated = context.Users.First(u => u.id == user.id);
        Assert.Equal(2, updated.consistencystreak);
    }

    private static TasksService CrearService(AA2_CS.Database.AppDbContext context)
    {
        var tasksRepository = new TasksRepository(context);
        var userRepository = new UserRepository(context);
        var purchaseRepository = new PurchaseRepository(context);
        var userService = new UserService(userRepository, purchaseRepository);
        return new TasksService(tasksRepository, userRepository, userService);
    }
}
