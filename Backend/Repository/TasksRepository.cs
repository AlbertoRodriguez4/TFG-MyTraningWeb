using AA2_CS.Database;
using AA2_CS.Model.Entities;
using Microsoft.EntityFrameworkCore;
using TaskEntity = AA2_CS.Model.Entities.Task;

namespace AA2_CS.Repository
{
    public class TasksRepository : IRepository<TaskEntity>
    {
        private readonly AppDbContext _context;
        private readonly UserRepository _userRepository;
        private readonly AchievementRepository _achievementRepository;

        public TasksRepository(AppDbContext context, UserRepository userRepository, AchievementRepository achievementRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _achievementRepository = achievementRepository;
        }

        public int Add(TaskEntity entity)
        {
            _context.Tasks.Add(entity);
            _context.SaveChanges();
            return entity.id;
        }

        public int Delete(TaskEntity entity)
        {
            _context.Tasks.Remove(entity);
            return _context.SaveChanges();
        }

        public List<TaskEntity> FindAll()
        {
            return _context.Tasks.ToList();
        }

        public List<TaskEntity> FindByCharacteristic(string value)
        {
            return _context.Tasks.Where(t => t.name.Contains(value) || t.description.Contains(value)).ToList();
        }

        public TaskEntity FindById(int id)
        {
            return _context.Tasks.FirstOrDefault(t => t.id == id);
        }

        public int Update(TaskEntity entity)
        {
            _context.Tasks.Update(entity);
            return _context.SaveChanges();
        }
        
        public List<TaskEntity> FindByUserId(int userId)
        {
            return _context.Tasks.Where(t => t.userId == userId).ToList();
        }

  
        public TaskEntity GetLastCompletedTask(int userId, int currentTaskId)
        {
            // Devuelve la última tarea completada por el usuario, excluyendo la tarea actual (si se ha completado). Se filtran las tareas del usuario para obtener solo 
            // las que están completadas y no son la tarea actual, se ordenan por fecha de creación descendente, y se devuelve la primera (más reciente) o null si no hay ninguna.
            return _context.Tasks
                .Where(t => t.userId == userId && t.iscompleted && t.id != currentTaskId)
                .OrderByDescending(t => t.createdat)
                .FirstOrDefault();
        }

        // Devuelve la fecha de la última tarea completada por el usuario (o null si no hay)
        public DateTime? GetLastCompletedTaskDate(int userId)
        {
            return _context.Tasks
                .Where(t => t.userId == userId && t.iscompleted)
                .OrderByDescending(t => t.createdat)
                .Select(t => (DateTime?)t.createdat)
                .FirstOrDefault();
        }

        public async System.Threading.Tasks.Task<string> CompleteTask(int taskId)
        {
            var task = FindById(taskId);
            if (task == null) return "Task not found";
            if (task.iscompleted) return "Task is already completed";

            var user = _userRepository.FindById(task.userId);
            if (user == null) return "User not found";

            task.iscompleted = true;

            if (task.trainingfocus == "strength")
            {
                user.strength += task.reward / 10;
            }
            else if (task.trainingfocus == "endurance")
            {
                user.endurance += task.reward / 10;
            }
            else if (task.trainingfocus == "ambas")
            {
                user.strength += task.reward / 20;
                user.endurance += task.reward / 20;
            }

            user.gold += task.reward;

            var lastCompleted = GetLastCompletedTask(user.id, task.id);

            if (lastCompleted != null)
            {
                var previousDate = lastCompleted.createdat.Date;
                var currentDate = task.createdat.Date;

                if ((currentDate - previousDate).TotalDays == 1)
                {
                    user.consistencystreak += 1;
                }
                else if ((currentDate - previousDate).TotalDays > 1)
                {
                    user.consistencystreak = 1;
                }
            }
            else
            {
                user.consistencystreak = 1;
            }

            Update(task);
            _userRepository.Update(user);

            _userRepository.AddExperience(user.id, task.reward);

            await CheckAchievementsAsync(user.id);

            return "Task completed and rewards applied successfully";
        }

        private async System.Threading.Tasks.Task CheckAchievementsAsync(int userId)
        {
            await _achievementRepository.EvaluateUserAchievementsAsync(userId);
        }
    }
}