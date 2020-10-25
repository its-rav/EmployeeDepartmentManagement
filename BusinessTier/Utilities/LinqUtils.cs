using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessTier.Utilities
{
    public static class LinqUtils
    {
        public static async Task<List<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T: class
        {
            return await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        }
        public static List<T> ToPaginatedList<T>(this IQueryable<T> source, int pageIndex, int pageSize) where T : class
        {
            return source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList<T>();
        }
    }
}
