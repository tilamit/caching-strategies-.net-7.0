using EasyCaching.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using System.Text.Json;

namespace SampleAppWithCaching.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserSqlServerCache _user;
        private readonly IDistributedCache _cache;
        private const string CacheKey = "users_cache";

        public UsersController(IUserSqlServerCache user, IDistributedCache cache)
        {
            _user = user;
            _cache = cache;
        }

        [HttpGet]
        [Route("/users/GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = null;
            try
            {
                var cachedUsers = await _cache.GetStringAsync(CacheKey);
                if (cachedUsers != null)
                {
                    return Ok(cachedUsers);
                }

                users = await _user.GetUsers();
                var serializedUsers = JsonSerializer.Serialize(users);

                await _cache.SetStringAsync(CacheKey, serializedUsers, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                });
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            finally
            {
                users = await _user.GetUsers();
            }

            return Ok(users);
        }
    }
}
