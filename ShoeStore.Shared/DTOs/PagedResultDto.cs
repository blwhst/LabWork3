namespace ShoeStore.Api.DTOs
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages => Size <= 0
            ? 0
            : (int)Math.Ceiling((double)TotalCount / Size);
    }
}