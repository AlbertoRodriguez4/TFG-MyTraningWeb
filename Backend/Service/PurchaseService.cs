using AA2_CS.Model.Entities;
using AA2_CS.Model.DTOs;
using AA2_CS.Repository;

namespace AA2_CS.Service
{
    public class PurchaseService : IService<Purchase>
    {
        private readonly PurchaseRepository _repository;

        public PurchaseService(PurchaseRepository repository)
        {
            _repository = repository;
        }

        public int Add(Purchase entity)
        {
            return _repository.Add(entity);
        }

        public int Delete(Purchase entity)
        {
            return _repository.Delete(entity);
        }

        public List<Purchase> FindAll()
        {
            return _repository.FindAll();
        }

        public Purchase FindById(int id)
        {
            return _repository.FindById(id);
        }

        public List<Purchase> FindByCharacteristic(string name)
        {
            return _repository.FindByCharacteristic(name);
        }

        public int Update(Purchase entity)
        {
            return _repository.Update(entity);
        }

   
        public List<PurchaseDTO> FindByUserId(int userId)
        {
            return _repository.FindByUserId(userId);
        }
    }
}