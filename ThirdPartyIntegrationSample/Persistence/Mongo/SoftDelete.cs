using Persistence.Interfaces;

namespace Persistence.Mongo
{
    public static class SoftDelete<T>
    {
        public static bool SupportsSoftDelete { get; } = typeof(ISoftDelete).IsAssignableFrom(typeof(T));
    }
}