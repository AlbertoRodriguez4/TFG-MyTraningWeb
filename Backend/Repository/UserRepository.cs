using AA2_CS.Database;
using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace AA2_CS.Repository
{
    public class UserRepository
    {
        private readonly AppDbContext _context;
        private readonly PurchaseRepository _purchaseRepository;

        private const int BASE_XP = 100;
        private const double EXPONENT = 1.5;

        public UserRepository(AppDbContext context, PurchaseRepository purchaseRepository)
        {
            _context = context;
            _purchaseRepository = purchaseRepository;
        }

        public int Add(User entity)
        {
            // ENCRIPTAR: Hasheamos la contraseña antes de guardar
            entity.passwordhash = BCrypt.Net.BCrypt.HashPassword(entity.passwordhash);

            _context.Users.Add(entity);
            _context.SaveChanges();
            return entity.id;
        }

        public int Update(User entity)
        {
            var user = _context.Users.FirstOrDefault(u => u.id == entity.id);
            if (user != null)
            {
                user.name = entity.name;
                user.email = entity.email;
                // Si se proporciona una nueva contraseña, hashearla y actualizarla. Se verifica si el campo de contraseña no está vacío y es diferente al hash actual 
                // (lo que indicaría que se ha proporcionado una nueva contraseña), y si es así, se hashea la
                if (!string.IsNullOrEmpty(entity.passwordhash) && entity.passwordhash != user.passwordhash)
                {
                    user.passwordhash = BCrypt.Net.BCrypt.HashPassword(entity.passwordhash);
                }

                user.level = entity.level;
                user.strength = entity.strength;
                user.endurance = entity.endurance;
                user.consistencystreak = entity.consistencystreak;
                user.gold = entity.gold;
                user.experience = entity.experience;

                // Actualizar referencias de equipo
                user.equippedStrengthId = entity.equippedStrengthId;
                user.equippedEnduranceId = entity.equippedEnduranceId;

                user.avatarUrl = entity.avatarUrl;

                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public async Task<ActionResult<User>> UpdateById(int id, User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return new NotFoundObjectResult($"User with ID {id} not found.");

            user.name = updatedUser.name;
            user.email = updatedUser.email;

            if (!string.IsNullOrEmpty(updatedUser.passwordhash) && updatedUser.passwordhash != user.passwordhash)
            {
                user.passwordhash = BCrypt.Net.BCrypt.HashPassword(updatedUser.passwordhash);
            }

            user.level = updatedUser.level;
            user.strength = updatedUser.strength;
            user.endurance = updatedUser.endurance;
            user.consistencystreak = updatedUser.consistencystreak;
            user.gold = updatedUser.gold;
            user.experience = updatedUser.experience;

            // Actualizar referencias de equipo
            user.equippedStrengthId = updatedUser.equippedStrengthId;
            user.equippedEnduranceId = updatedUser.equippedEnduranceId;

            user.avatarUrl = updatedUser.avatarUrl;

            try
            {
                await _context.SaveChangesAsync();
                return new OkObjectResult(user);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to update user: {ex.Message}");
            }
        }

        public int Delete(User entity)
        {
            var user = _context.Users.FirstOrDefault(u => u.id == entity.id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public List<User> FindAll()
        {
            return _context.Users.AsNoTracking().ToList();
        }

        public User FindById(int id)
        {
            return _context.Users
                .Include(u => u.EquippedStrengthItem)
                .Include(u => u.EquippedEnduranceItem)
                .FirstOrDefault(u => u.id == id);
        }

        public List<User> FindByCharacteristic(string name)
        {
            return _context.Users
                .AsNoTracking()
                .Where(u => EF.Functions.ILike(u.name, $"{name}%"))
                .ToList();
        }

        public User? FindByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.email.ToLower() == email.ToLower());
        }

        public User Login(string email, string plainPassword)
        {
            // Limpiamos el email de espacios extra y lo pasamos a minúsculas
            string normalizedEmail = email.Trim().ToLower();

            var user = _context.Users
                .Include(u => u.EquippedStrengthItem)
                .Include(u => u.EquippedEnduranceItem)
                .FirstOrDefault(u => u.email.ToLower() == normalizedEmail);

            if (user == null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, user.passwordhash);
  
            if (isValid)
            {
                return user;
            }
            return null;
        }

        public User Register(User user)
        {
            user.passwordhash = BCrypt.Net.BCrypt.HashPassword(user.passwordhash);
            _context.Users.Add(user);
            _context.SaveChanges();
            return user;
        }

        public List<User> GetTopThreeUsers()
        {
            // Devuelve los 3 usuarios con mayor nivel, incluyendo su equipo equipado. Se ordenan los usuarios por nivel de forma descendente, 
            // se incluyen las referencias a los objetos de equipo equipado para que estén disponibles en el resultado, y se limita el resultado a los 3 primeros usuarios.
            return _context.Users
                .AsNoTracking()
                .Include(u => u.EquippedStrengthItem)
                .Include(u => u.EquippedEnduranceItem)
                .OrderByDescending(u => u.level)
                .Take(3)
                .ToList();
        }
        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(currentPassword, user.passwordhash);

            if (!isCurrentPasswordValid)
                return false;

            user.passwordhash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int GetXpRequiredForNextLevel(int currentLevel)
        {
            return (int)Math.Floor(BASE_XP * Math.Pow(currentLevel, EXPONENT));
        }

        public void AddExperience(int userId, int xpGained)
        {
            var user = FindById(userId);
            if (user == null) return;

            user.experience += xpGained;

            int xpRequired = GetXpRequiredForNextLevel(user.level);

            while (user.experience >= xpRequired)
            {
                user.experience -= xpRequired;
                user.level++;

                user.gold += 50 * user.level;
                user.strength += 1;

                xpRequired = GetXpRequiredForNextLevel(user.level);
            }

            Update(user);
        }

        public string EquipItem(int userId, int itemId)
        {
            var user = FindById(userId);
            if (user == null) return "User not found";

            var purchases = _purchaseRepository.FindByUserId(userId);
            var purchase = purchases.FirstOrDefault(p => p.ItemId == itemId);

            if (purchase == null)
            {
                return "No posees este objeto, debes comprarlo primero.";
            }

            string type = purchase.ItemType.ToLower();

            if (type == "strength" || type == "fuerza")
            {
                user.equippedStrengthId = itemId;
            }
            else if (type == "endurance" || type == "resistencia")
            {
                user.equippedEnduranceId = itemId;
            }
            else
            {
                return "Este tipo de objeto no se puede equipar.";
            }

            Update(user);
            return "Item equipped successfully";
        }

        public string UnequipItem(int userId, string type)
        {
            var user = FindById(userId);
            if (user == null) return "User not found";

            string typeLower = type.ToLower();

            if (typeLower == "strength" || typeLower == "fuerza")
            {
                user.equippedStrengthId = null;
            }
            else if (typeLower == "endurance" || typeLower == "resistencia")
            {
                user.equippedEnduranceId = null;
            }
            else
            {
                return "Tipo de equipo inválido.";
            }

            Update(user);
            return "Item unequipped successfully";
        }

        public (int totalStrength, int totalEndurance, int xpRequiredForNextLevel, int xpRemaining) GetUserTokenStats(User user)
        {
            var purchases = _purchaseRepository.FindByUserId(user.id);

            int bonusStrength = purchases
                .Where(p => p.ItemType != null && p.ItemType.Equals("Strength", StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.ItemBonus);

            int bonusEndurance = purchases
                .Where(p => p.ItemType != null && p.ItemType.Equals("Endurance", StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.ItemBonus);

            int totalStrength = user.strength + bonusStrength;
            int totalEndurance = user.endurance + bonusEndurance;

            int xpRequiredForNextLevel = GetXpRequiredForNextLevel(user.level);
            int xpRemaining = Math.Max(0, xpRequiredForNextLevel - user.experience);

            return (totalStrength, totalEndurance, xpRequiredForNextLevel, xpRemaining);
        }
    }
}
