namespace Aiursoft.DbTools;

public interface ISynchronizable<T>
{
    bool EqualsInDb(T obj);
    T Map();
}