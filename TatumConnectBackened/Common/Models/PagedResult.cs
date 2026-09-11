using System.Runtime.InteropServices;

namespace TatumConnectBackened.Common.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage =>
            PageNumber > 1;

        public bool HasNextPage =>
            PageNumber < TotalPages;
    }
    namespace TatumConnect.Backend.Common.Models { }

    public class PaginationParameters
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value switch
                {
                    <= 0 => 10,
                    > MaxPageSize => MaxPageSize,
                    _ => value
                };
            }

        }

    }
}  