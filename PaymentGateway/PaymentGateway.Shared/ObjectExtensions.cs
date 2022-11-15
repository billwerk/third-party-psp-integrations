using System.Reflection;

namespace PaymentGateway.Shared;

public static class ObjectExtensions
{

    public static Dictionary<string, string> ToDictionary(this object @object, Func<PropertyInfo, bool> predicate = null)
        => @object?.GetType()
            .GetProperties()
            .Where(propertyInfo => predicate?.Invoke(propertyInfo) ?? PropertyInfoDefaultPredicate(propertyInfo))
            .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(@object)?.ToString())!;
    
    /// <summary>
    /// Convert object to Dictionary&lt;string, string&gt; structure base on properties which have setters.
    /// For null arg will return null.
    /// </summary>
    public static Dictionary<string, string> ToDictionaryFromPropertiesWithSetter(this object @object)
        => @object.ToDictionary(propertyInfo => propertyInfo.CanWrite);

    public static TObject FillByKeys<TObject, TProperty>(this TObject @object, IDictionary<string, TProperty> dictionary) =>
        @object.FillByKeysWithPropertiesFilter(dictionary, PropertyInfoDefaultPredicate);

    public static TObject FillByKeysWhereMissing<TObject, TProperty>(this TObject @object, IDictionary<string, TProperty> dictionary) =>
        @object.FillByKeysWithPropertiesFilter(dictionary, property => property.GetValue(@object) == default);

    public static TObject FillByKeysWithPropertiesFilter<TObject, TProperty>(this TObject @object,
        IDictionary<string, TProperty> dictionary, Func<PropertyInfo, bool> propertyFilter) =>
        @object.GetType().GetProperties()
            .Where(property => property.CanWrite &&
                               propertyFilter(property) &&
                               dictionary.TryGetValue(property.Name, out var value) &&
                               value?.GetType() == property.PropertyType)
            .Do(properties => properties.ForEach(property => property.SetValue(@object, dictionary[property.Name])))
            .To(_ => @object);

    private static bool PropertyInfoDefaultPredicate(PropertyInfo propertyInfo) => true;
    
    /// <summary>
    /// Convert IDictionary<string, string> dictionary to object by using reflection.
    /// If incoming dictionary is null, return null.
    /// </summary>
    public static T ConvertToObject<T>(this IDictionary<string, string> dictionary)
        where T : class
    {
        if (dictionary == null)
            return null;

        var @object = Activator.CreateInstance<T>();
        var objectProperties = @object.GetType().GetProperties();

        foreach (var item in dictionary)
        {
            var propInfo = objectProperties.FirstOrDefault(x => x.Name == item.Key);
            if (propInfo == null || !propInfo.CanWrite)
                continue;

            var propertyType = propInfo.PropertyType;

            if (propertyType.IsValueType && item.Value == null && Nullable.GetUnderlyingType(propertyType) != null)
                propInfo.SetValue(@object, item.Value);
            else if (propertyType == typeof(DateTime?) || propertyType == typeof(DateTime))
                propInfo.SetValue(@object, DateTime.Parse(item.Value));
            else
                propInfo.SetValue(@object, Convert.ChangeType(item.Value, propertyType));
        }

        return @object;
    }
}
