using MongoDB.Bson;

namespace Persistence.Interfaces
{
    public interface IServiceBase<T> where T : class, IDatabaseObject
    {
        T Create(T item);

        void Update(T item);
        
        T SingleByIdOrDefault(ObjectId id);
    }
}