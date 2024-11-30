using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using SampleAppWithCaching.UserDbContext;

namespace SampleAppWithCaching.Repository
{
    public class UserSqlServerCacheRepository : IUserSqlServerCache
    {
        private readonly UsersDbContext _dbContext;

        public UserSqlServerCacheRepository(UsersDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<User>> GetUsers()
        {
            var aLst = _dbContext.users.ToList();
            return await Task.FromResult(aLst);
        }
    }
}
