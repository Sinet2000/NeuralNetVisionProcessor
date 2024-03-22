using Sieve.Models;

namespace NetVisionProc.Common.Data.Models
{
    public class PagedResponse<T>
        where T : class
    {
        public PagedResponse(SieveModel request, int totalCount)
        {
            SetPageSize(request);
            SetSorting(request);
            SetPageDetails(totalCount);
        }

        public int PageSize { get; private set; }
        public string SortedBy { get; private set; } = null!;
        public int TotalCount { get; private set; }
        public int PageCount { get; private set; }
        public IList<T> Data { get; init; } = new List<T>();
        public int Count => Data.Count;

        private void SetPageSize(SieveModel request)
        {
            PageSize = request.PageSize ?? CommonConst.TablePageSize;
        }

        private void SetSorting(SieveModel request)
        {
            SortedBy = request.Sorts;
        }

        private void SetPageDetails(int totalCount)
        {
            TotalCount = totalCount;
            PageCount = (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }
}
