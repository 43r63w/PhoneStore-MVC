using PhoneStore.DAL.Data;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.DAL.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationRepository
    {
        private readonly ApplicationdDBContext _dbContext;
        public ApplicationUserRepository(ApplicationdDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Update(ApplicationUser obj)
        {
            _dbContext.ApplicationUsers.Update(obj);
        }
    }
}
