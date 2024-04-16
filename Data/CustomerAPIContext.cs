using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomerAPI.Models;

namespace CustomerAPI.Data
{
    public class CustomerAPIContext: DbContext
    {
        public CustomerAPIContext(DbContextOptions<CustomerAPIContext> options)
          : base(options)
        {
        }

        public DbSet<CustomerAPI.Models.TblCustomer>? TblCustomer { get; set; }
        public DbSet<CustomerAPI.Models.TblUser>? TblUser { get; set; }
        public DbSet<CustomerAPI.Models.TblPermission>? TblPermission { get; set; }
        public DbSet<CustomerAPI.Models.TblRole>? TblRole { get; set; }
        public DbSet<CustomerAPI.Models.UserCred>? UserCred { get; set; }
        public DbSet<CustomerAPI.Models.TblRefreshtoken>? TblRefreshtoken { get; set; }

    }

}
