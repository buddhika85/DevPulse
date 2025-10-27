namespace UserService.Application.Common.Models
{
    public record class PaginatedResult<T> where T : class
    {
        public IReadOnlyList<T> PageItems { get; init; } = [];

        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }
}
