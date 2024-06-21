using Microsoft.Extensions.Caching.Memory;
using SampleAppWithCaching.Interface;
using SampleAppWithCaching.Models;
using SampleAppWithCaching.UserDbContext;
using SampleAppWithCaching.Utility;

namespace SampleAppWithCaching.Repository
{
    public class UserRepository : IUser
    {
        private readonly UsersDbContext _dbContext;

        public UserRepository(UsersDbContext dbContext)
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
