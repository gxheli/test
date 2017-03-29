using PagedList;
using Simple.OData.Client;
using System.Threading.Tasks;

namespace LanghuaNew
{
    public static class ODataAsyncPageListExtensions
    {

        /// <summary>
        /// Async creates a subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.
        /// </summary>
        /// <typeparam name="T">The type of object the collection should contain.</typeparam>
        /// <typeparam name="TKey">Type For Compare</typeparam>
        /// <param name="superset">The collection of objects to be divided into subsets. If the collection implements <see cref="IQueryable{T}"/>, it will be treated as such.</param>
        /// <param name="pageNumber">The one-based index of the subset of objects to be contained by this instance.</param>
        /// <param name="pageSize">The maximum size of any individual subset.</param>
        /// <returns>A subset of this collection of objects that can be individually accessed by index and containing metadata about the collection of objects the subset was created from.</returns>
        /// <seealso cref="PagedList{T}"/>
        public static Task<IPagedList<T>> ToODataPagedListAsync<T>(this IBoundClient<T> superset, int pageNumber, int pageSize,int TotalCount) where T : class
        {
            return LanghuaODataAsyncPageList<T>.CreateAsync(superset, pageNumber, pageSize, TotalCount);
        }
    }
}