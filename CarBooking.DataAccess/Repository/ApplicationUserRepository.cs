using CarBooking.DataAccess.data;
using CarBooking.DataAccess.Repository.IRepository;
using CarBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBooking.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private ApplicationDBcontext _db;
        public ApplicationUserRepository(ApplicationDBcontext db): base(db)
        {
            _db = db;
        }

    }
}
