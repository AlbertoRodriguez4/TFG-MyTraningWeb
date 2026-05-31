using AA2_CS.Database;
using AA2_CS.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace AA2_CS.Repository
{
    public class ItemRepository : IRepository<Item>
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public int Add(Item entity)
        {
            _context.Items.Add(entity);
            _context.SaveChanges();
            return entity.id;
        }

        public int Delete(Item entity)
        {
            var item = _context.Items.FirstOrDefault(i => i.id == entity.id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public List<Item> FindAll()
        {
            return _context.Items.ToList();
        }

        public List<Item> FindByCharacteristic(string name)
        {
            return _context.Items.Where(i => i.name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public Item FindById(int id)
        {
            return _context.Items.FirstOrDefault(i => i.id == id);
        }

        public int Update(Item entity)
        {
            var item = _context.Items.FirstOrDefault(i => i.id == entity.id);
            if (item != null)
            {
                item.name = entity.name;
                item.type = entity.type;
                item.bonus = entity.bonus;
                item.price = entity.price;
                _context.SaveChanges();
                return 1;
            }
            return 0;
        }

        public List<Item> GetRandomStrengthItems(int count = 2)
        {
            // Selecciona aleatoriamente items de tipo "Strength" basados en el día del año para asegurar variedad diaria, y si no hay coincidencias, selecciona aleatoriamente del conjunto completo de items de fuerza
            int todayDayOfYear = DateTime.Now.DayOfYear;
            var strengthItems = _context.Items.Where(i => i.type == "Strength").ToList();
            var selectedItems = strengthItems.Where(item => item.id % todayDayOfYear == 0).ToList(); // Selecciona items cuyo ID es divisible por el día del año

            if (selectedItems.Count == 0)
            {
                var random = new Random();
                selectedItems = strengthItems.OrderBy(x => random.Next()).Take(count).ToList();
            }

            return selectedItems;
        }

        public List<Item> GetRandomEnduranceItems(int count = 2)
        {
            //Misma logica que para los items de fuerza, pero aplicada a los items de resistencia, lo que permite a los usuarios obtener una selección diaria de items de resistencia para complementar su entrenamiento
            int todayDayOfYear = DateTime.Now.DayOfYear;
            var enduranceItems = _context.Items.Where(i => i.type == "Endurance").ToList();
            var selectedItems = enduranceItems.Where(item => item.id % todayDayOfYear == 0).ToList(); // Selecciona items cuyo ID es divisible por el día del año

            if (selectedItems.Count == 0)
            {
                var random = new Random();
                selectedItems = enduranceItems.OrderBy(x => random.Next()).Take(count).ToList();
            }

            return selectedItems;
        }

        public List<Item> GetRandomItems(int count = 4)
        {
            // Misma logica que en los dos metodos anteriores pero no se filtra por tipo
            int todayDayOfYear = DateTime.Now.DayOfYear;
            var allItems = _context.Items.ToList();
            var selectedItems = allItems.Where(item => item.id % todayDayOfYear == 0).ToList();

            if (selectedItems.Count == 0)
            {
                var random = new Random();
                selectedItems = allItems.OrderBy(x => random.Next()).Take(count).ToList();
            }

            return selectedItems;
        }

        public async Task<Item?> UpdateByIdAsync(int id, Item updatedItem)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.id == id);
            if (item == null)
            {
                return null;
            }

            item.name = updatedItem.name;
            item.type = updatedItem.type;
            item.bonus = updatedItem.bonus;
            item.price = updatedItem.price;

            try
            {
                await _context.SaveChangesAsync();
                return item;
            }
            catch
            {
                return null;
            }
        }
    }
}