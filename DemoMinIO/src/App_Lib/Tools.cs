namespace src.App_Lib
{
    public static class Tools
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
        {
            var list = new List<T>();

            await foreach (var item in source)
            {
                list.Add(item);
            }
            
            return list;
        }
    }
}