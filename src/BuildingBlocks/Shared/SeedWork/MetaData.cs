namespace Shared.SeedWork
{
    //paging, sorting, filtering, etc.
    public class MetaData
    {
        public long TotalItems { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
        public int FirstRowOnPage => TotalItems > 0 ? (CurrentPage - 1) * PageSize + 1 : 0;
        public int LastRowOnPage => (int)Math.Min(CurrentPage * PageSize, TotalItems);
    }
}
