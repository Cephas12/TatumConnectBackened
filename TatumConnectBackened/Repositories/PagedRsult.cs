using TatumConnectBackened.Common.Models;

namespace TatumConnectBackened.Repositories
{
    internal class PagedRsult<T> : PagedResult<T>
    {
        // Corrected typos and generic base type. Keep internal usage limited.
        public List<T> Items { get; set; } = new List<T>();
        public int PagedNumber { get; set; }
        public new int PageSize { get; set; }
        public new int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}