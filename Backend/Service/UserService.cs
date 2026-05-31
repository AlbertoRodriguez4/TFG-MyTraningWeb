using AA2_CS.Model.Entities;
using AA2_CS.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AA2_CS.Service
{
    public class UserService
    {
        private readonly UserRepository _repository;

        public UserService(UserRepository repository)
        {
            _repository = repository;
        }

        public int Add(User entity)
        {
            return _repository.Add(entity);
        }

        public int Update(User entity)
        {
            return _repository.Update(entity);
        }

        public Task<ActionResult<User>> UpdateById(int id, User updatedUser)
        {
            return _repository.UpdateById(id, updatedUser);
        }

        public int Delete(User entity)
        {
            return _repository.Delete(entity);
        }

        public List<User> FindAll()
        {
            return _repository.FindAll();
        }

        public User FindById(int id)
        {
            return _repository.FindById(id);
        }

        public List<User> FindByCharacteristic(string name)
        {
            return _repository.FindByCharacteristic(name);
        }

        public User? FindByEmail(string email)
        {
            return _repository.FindByEmail(email);
        }

        public User Login(string email, string passwordhash)
        {
            return _repository.Login(email, passwordhash);
        }

        public User Register(User user)
        {
            return _repository.Register(user);
        }

        public List<User> GetTopThreeUsers()
        {
            return _repository.GetTopThreeUsers();
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            return await _repository.ChangePassword(userId, currentPassword, newPassword);
        }

        public int GetXpRequiredForNextLevel(int currentLevel)
        {
            return _repository.GetXpRequiredForNextLevel(currentLevel);
        }

        public void AddExperience(int userId, int xpGained)
        {
            _repository.AddExperience(userId, xpGained);
        }

        public string EquipItem(int userId, int itemId)
        {
            return _repository.EquipItem(userId, itemId);
        }

        public string UnequipItem(int userId, string type)
        {
            return _repository.UnequipItem(userId, type);
        }

        public (int totalStrength, int totalEndurance, int xpRequiredForNextLevel, int xpRemaining) GetUserTokenStats(User user)
        {
            return _repository.GetUserTokenStats(user);
        }
    }
}
