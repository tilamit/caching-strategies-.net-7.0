using Microsoft.Extensions.Caching.Memory;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using System.Collections.Generic;

namespace SampleAppWithCaching.Repository
{
    public class UserCaching
    {
        private readonly ICacheProvider _cacheProvider;
        private const int CacheTTLInSeconds = 10;
        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CacheTTLInSeconds));


        public UserCaching(IMemoryCache memoryCache, UserRepository userRepository)

        {

            //_usersService = usersService;
            //_cacheProvider = cacheProvider;

            //_userRepository = userRepository;

        }

        //public async Task<List<User>> GetCachedAuthorsAsync()
        //{
        //    const string cacheKey = "User";

        //    List<User> aLst = new List<User>();

        //    if (!_memoryCache.TryGetValue(cacheKey, out List<User> cachedData))
        //    {
        //        cachedData = await _userRepository.GetUsersAsync();

        //        _memoryCache.Set(cacheKey, cachedData, TimeSpan.FromMinutes(60));
        //    }

        //    return cachedData;
        //}
    }
}
