namespace InDaCompany.Data.Interfaces;

public interface IBaseDao<T>
{
    List<T> GetAll();

    T GetById(int id);

    void Insert(T entity);

    void Update(T entity);

    void Delete(int id);

    bool Exists(int id);

}
