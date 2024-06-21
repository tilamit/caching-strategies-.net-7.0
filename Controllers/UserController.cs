using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using SampleAppWithCaching.Repository;
using SampleAppWithCaching.UserDbContext;
using SampleAppWithCaching.Utility;

namespace SampleAppWithCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ICacheProvider _cacheProvider;
        private const int CacheTTLInSeconds = 60;
        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CacheTTLInSeconds));

        System.Diagnostics.Stopwatch myStopWatch = new System.Diagnostics.Stopwatch();

        public UserController(IUser user, ICacheProvider cacheProvider)
        {
            _user = user;
            _cacheProvider = cacheProvider;
        }

        //Client-side caching
        [ResponseCache(CacheProfileName = "UserCacheForOneMin")][HttpGet][Route("/user/GetAllUsers")]
        public async Task<List<User>> GetAllUsers()
        {
            myStopWatch.Start();
            var users = await _user.GetUsers();
            myStopWatch.Stop();

            double total = myStopWatch.Elapsed.TotalSeconds;

            return users;
        }

        //Server-side caching
        [HttpGet][Route("/user/GetUsers")]
        public async Task<List<User>> GetUserDetails()
        {
            myStopWatch.Start();
            var users = await GetCachedResponse(CacheKey.users, () => _user.GetUsers());
            myStopWatch.Stop();

            double total = myStopWatch.Elapsed.TotalSeconds;

            return users;
        }

        private async Task<List<User>> GetCachedResponse(string cacheKey, Func<Task<List<User>>> func)
        {
            List<User> users = new List<User>();

            try
            {
                users = _cacheProvider.GetFromCache<List<User>>(cacheKey);
                if (users != null) return users;

                users = await func();

                _cacheProvider.SetCache(cacheKey, users, cacheEntryOptions);
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return users;
        }
    }
}
