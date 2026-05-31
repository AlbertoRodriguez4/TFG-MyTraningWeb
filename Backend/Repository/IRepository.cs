namespace AA2_CS.Repository
{
    public interface IRepository<T> where T : class
    {
        // Interfaz genérica para repositorios, definiendo métodos comunes para operaciones CRUD y consultas específicas basadas en características de la entidad
        int Add(T entity);
        int Update(T entity);
        int Delete(T entity);
        T FindById(int id);
        List<T> FindAll();
        List<T> FindByCharacteristic(string value);
    }
}
