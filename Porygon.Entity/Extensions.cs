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

            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        public static async Task<List<TModel>> ParallelForEach<T,TModel>(this IEnumerable<T> list, Func<T, Task<TModel>> func)
        {
            var array = await Task.WhenAll(list.Select(e => func(e)));
            return array.FilterNulls();
        }

        public static List<T> FilterNulls<T>(this IEnumerable<T?> list)
        {
            return list
                .Where(x => x != null)
                .Select(x => x!)
                .ToList();
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
