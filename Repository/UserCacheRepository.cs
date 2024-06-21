using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using SampleAppWithCaching.Utility;

namespace SampleAppWithCaching.Repository
{
    public class UserCacheRepository
    {
        private readonly ICacheProvider _cacheProvider;

        public UserCacheRepository(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public List<User> GetCachedUser()
        {
            return _cacheProvider.GetFromCache<List<User>>(CacheKey.users);
        }

        public void ClearCache()
        {
            _cacheProvider.ClearCache(CacheKey.users);
        }
    }
}
