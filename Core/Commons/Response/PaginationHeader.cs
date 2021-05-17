namespace Response
{
    public class PaginationHeader
    {
        public int CurrentPage { get; set; }
        public int DisplayItems { get; set; }
        public int ResultCount { get; set; }
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }

        public PaginationHeader() { }
        public PaginationHeader(int currentPage, int displayItems, int resultCount, int totalItems, int totalPages)
        {
            this.CurrentPage = currentPage;
            this.DisplayItems = displayItems;
            this.ResultCount = resultCount;
            this.TotalItems = totalItems;
            this.TotalPages = totalPages;
        }
    }
}