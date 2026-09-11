using TatumConnectBackened.Common.Models;

namespace TatumConnectBackened.Repositories
{
    internal class PagedRsult<T> : PagedResult<Microsoft.Identity.Client.NativeInterop.Account>
    {
        public List<Entities.Account> items { get; set; }
        public int PagedNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int totalPages { get; set; }
    }
}