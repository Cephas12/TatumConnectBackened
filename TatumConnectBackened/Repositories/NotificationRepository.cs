using TatumConnectBackened.Data;
using TatumConnectBackened.Entities;

namespace TatumConnectBackened.Repositories
{

    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context) { 
        }
    }
}
