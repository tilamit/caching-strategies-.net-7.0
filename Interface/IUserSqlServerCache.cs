using SampleAppWithCaching.Models;

namespace SampleAppWithCaching.Interface
{
    public interface IUserSqlServerCache
    {
        public Task<List<User>> GetUsers();
    }
}
