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
    }
}
