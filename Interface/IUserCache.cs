using SampleAppWithCaching.Models;

namespace SampleAppWithCaching.Interface
{
    public interface IUserCache
    {
        List<User> GetCachedUser();
        void ClearCache();
    }
}
