using System;
using System.Threading.Tasks;
using PagedList;
using Simple.OData.Client;
namespace LanghuaNew
{
    public class LanghuaODataAsyncPageList<T> : BasePagedList<T> where T : class
    {
        private LanghuaODataAsyncPageList()
        {
        }

        public static async Task<IPagedList<T>> CreateAsync( IBoundClient<T> superset, int pageNumber, int pageSize, int TotalCount)
        {
            var list = new LanghuaODataAsyncPageList<T>();
        
            await list.InitAsync(superset, pageNumber, pageSize, TotalCount).ConfigureAwait(false);
            return list;
        }

        //public static string GetFormatOperator(IBoundClient<T> superset, ExpressionType)
        //{
        //    var list = new LanghuaODataAsyncPageList<T>();

        //    await list.InitAsync(superset, pageNumber, pageSize, TotalCount).ConfigureAwait(false);
        //    return list;
        //}

        private async Task InitAsync(IBoundClient<T> superset, int pageNumber, int pageSize, int TotalCount)
        {
            
            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException("pageNumber: " + pageNumber, "PageNumber cannot be below 1.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException("pageSize: " + pageSize, "PageSize cannot be less than 1.");
            }

            TotalItemCount = TotalCount;
            PageSize = pageSize;
            PageNumber = pageNumber;
            PageCount = (TotalItemCount > 0) ? ((int)Math.Ceiling(((double)TotalItemCount) / PageSize)) : 0;
            HasPreviousPage = PageNumber > 1;
            HasNextPage = PageNumber < PageCount;
            IsFirstPage = PageNumber == 1;
            IsLastPage = PageNumber >= PageCount;
            FirstItemOnPage = ((PageNumber - 1) * PageSize) + 1;
            var num = (FirstItemOnPage + PageSize) - 1;
            LastItemOnPage = (num > TotalItemCount) ? TotalItemCount : num;

            if ((superset != null) && (TotalItemCount > 0))
            {
                Subset.AddRange(
                    (pageNumber == 1)
                        ? await superset.Skip(0).Top(pageSize).FindEntriesAsync()
                        : await superset.Skip(((pageNumber - 1) * pageSize)).Top(pageSize).FindEntriesAsync()
                    );
            }
            
        }

    }
}