using SampleAppWithCaching.Models;

namespace SampleAppWithCaching.Interface
{
    public interface IUser
    {
        public Task<List<User>> GetUsers();
    }
}
