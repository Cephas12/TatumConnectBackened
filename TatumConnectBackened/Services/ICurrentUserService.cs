namespace TatumConnectBackened.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        bool IsAdmin { get; }
        bool IsSuperAdmin { get; }
    }
}
