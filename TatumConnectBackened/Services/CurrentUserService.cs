using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace TatumConnectBackened.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public Guid UserId
        {
            get
            {
                var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(idClaim, out var id)) return id;
                return Guid.Empty;
            }
        }

        public bool IsAdmin => User?.IsInRole("Admin") == true;

        public bool IsSuperAdmin => User?.IsInRole("SuperAdmin") == true;
    }
}
