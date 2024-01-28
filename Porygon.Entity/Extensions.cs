using System.Reflection;
using Newtonsoft.Json;

namespace Porygon.Entity
{
    public static class Extensions
    {

        public static bool IsNullOrEmpty<T>(IEnumerable<T>? enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            if (enumerable is ICollection<T> collection)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        public static async Task<List<Out>> ParallelForEach<In, Out>(this IEnumerable<In> list, Func<In, Task<Out>> func)
        {
            var array = await Task.WhenAll(list.Select(e => func(e)));
            return array.FilterNulls();
        }

        public static async Task ParallelForEach<In>(this IEnumerable<In> list, Func<In, Task> func)
        {
            await Task.WhenAll(list.Select(e => func(e)));
        }

        public static List<Out> SelectNonNull<In, Out>(this IEnumerable<In> list, Func<In, Out?> selector)
        {
            return list
                .Select(x => selector(x))
                .FilterNulls();
        }

        public static List<T> FilterNulls<T>(this IEnumerable<T?> list)
        {
            return list
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();
        }

        public static void SetObjectProperty(this object obj, string propertyName, object propertyValue)
        {
            obj.GetType().GetProperty(propertyName)?.SetValue(obj, propertyValue, null);
        }

        public static IEnumerable<PropertyInfo> GetPropertiesByAttribute(this object obj, Type attributeType)
        {
            IEnumerable<PropertyInfo> properties = obj.GetType().GetProperties()
                .Where(p => p.HasAttribute(attributeType));

            if (IsNullOrEmpty(properties))
                return new List<PropertyInfo>();
            return properties;
        }

        public static bool HasAttribute(this PropertyInfo property, Type attributeType)
        {
            return Attribute.GetCustomAttribute(property, attributeType) != null;
        }

        public static Cast? GetObjectPropertyValue<Cast>(this object obj, string propertyName)
        {
            try
            {
                return (Cast?)obj.GetType()?.GetProperty(name: propertyName)?.GetValue(obj, null);
            }
            catch (Exception)
            {
                throw new ArgumentException($"Couldn't get property {propertyName} from object {obj}");
            }
        }

        public static T Clone<T>(this T source)
        {
            if (source is null) return default;

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}
