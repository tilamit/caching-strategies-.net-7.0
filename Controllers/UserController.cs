using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using SampleAppWithCaching.Utility;

namespace SampleAppWithCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ICacheProvider _cacheProvider;
        private const int CacheTTLInSeconds = 30;
        private readonly MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CacheTTLInSeconds));

        System.Diagnostics.Stopwatch myStopWatch = new System.Diagnostics.Stopwatch();

        private readonly IEasyCachingProviderFactory _factory;
       
        public UserController(IUser user, ICacheProvider cacheProvider, IEasyCachingProviderFactory factory)
        {
            SetThreads();
            _user = user;
            _cacheProvider = cacheProvider;
            this._factory = factory;
        }

        //Client-side caching
        [ResponseCache(CacheProfileName = "UserCacheForOneMin")]
        [HttpGet]
        [Route("/user/GetAllUsers")]
        public async Task<List<User>> GetAllUsers()
        {
            myStopWatch.Start();
            var users = await _user.GetUsers();
            myStopWatch.Stop();

            double total = myStopWatch.Elapsed.TotalSeconds;

            return users;
        }

        //Server-side caching
        [HttpGet]
        [Route("/user/GetUsers")]
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

        private static void SetThreads()
        {
            int miniThreads = Convert.ToInt32(Environment.GetEnvironmentVariable("MIN_THREADS") ?? "1000");
            ThreadPool.SetMinThreads(miniThreads, miniThreads);
            ThreadPool.GetMinThreads(out int workerThreads, out int completionPortThreads);

            Console.WriteLine($"env set threads:{miniThreads}");
            Console.WriteLine($"worker threads:{workerThreads}");
            Console.WriteLine($"completionPort threads:{completionPortThreads}");
        }

        //Distributed caching with Redis
        // GET api/values/redis2  
        [HttpGet]
        [Route("/user/redis2")]
        public async Task<List<User>> GetRedis2()
        {
            string cacheProvider = "redis2";
            string redisKey = "SetCache";
            double duration = CacheTTLInSeconds;

            var users = await _factory.GetOrSetAsync(cacheProvider, redisKey, () => _user.GetUsers(), duration);

            return await Task.Run(() => users.ToList());
        }

        // GET api/values/redis4
        [HttpGet]
        [Route("/user/redis4")]
        public async Task<List<User>> GetRedis4()
        {
            string cacheProvider = "redis4";
            string redisKey = "SetCache";
            double duration = CacheTTLInSeconds;

            //_factory.RemoveCache(cacheProvider, redisKey);

            var users = await _factory.GetOrSetAsync(cacheProvider, redisKey, () => _user.GetUsers(), duration);

            return await Task.Run(() => users.ToList());
        }
    }
}