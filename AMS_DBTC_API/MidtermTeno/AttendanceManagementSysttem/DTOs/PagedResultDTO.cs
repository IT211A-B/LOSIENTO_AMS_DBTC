namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    /// <summary>
    /// Generic paginated API response wrapper.
    /// </summary>
    /// <typeparam name="T">The item type returned in the page.</typeparam>
    public class PagedResultDTO<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new();
    }
}

