using AA2_CS.Repository;
using AA2_CS.Model.Entities;
using TaskEntity = AA2_CS.Model.Entities.Task;

namespace AA2_CS.Service
{
    public class TasksService : IService<TaskEntity>
    {
        private readonly TasksRepository _repository;

        public TasksService(TasksRepository repository)
        {
            _repository = repository;
        }

        public int Add(TaskEntity entity)
        {
            return _repository.Add(entity);
        }

        public int Delete(TaskEntity entity)
        {
            return _repository.Delete(entity);
        }

        public List<TaskEntity> FindAll()
        {
            return _repository.FindAll();
        }

        public List<TaskEntity> FindByCharacteristic(string name)
        {
            return _repository.FindByCharacteristic(name);
        }

        public TaskEntity FindById(int id)
        {
            return _repository.FindById(id);
        }

        public int Update(TaskEntity entity)
        {
            return _repository.Update(entity);
        }

        public List<TaskEntity> FindByUserId(int userId)
        {
            return _repository.FindByUserId(userId);
        }

        public async System.Threading.Tasks.Task<string> CompleteTask(int taskId)
        {
            return await _repository.CompleteTask(taskId);
        }
    }
}
