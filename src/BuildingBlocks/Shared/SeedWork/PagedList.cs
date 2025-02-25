namespace Shared.SeedWork
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, long totalItems, int pageNumber, int pageSize)
        {
            _metaData = new MetaData
            {
                TotalItems = totalItems,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
            AddRange(items);
        }
        private MetaData _metaData { get; }
        public MetaData GetMetaData => _metaData;
        //public int PageNumber { get; }
        //public int PageSize { get; }
        //public int TotalCount { get; }
        //public int TotalPages { get; }
        //public bool HasPreviousPage => PageNumber > 1;
        //public bool HasNextPage => PageNumber < TotalPages;
        //public static PagedList<T> ToPagedList(List<T> items, int count, int pageNumber, int pageSize)
        //{
        //    return new PagedList<T>(items, count, pageNumber, pageSize);
        //}
    }
}
