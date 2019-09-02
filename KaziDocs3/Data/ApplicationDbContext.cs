using KaziDocs3.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaziDocs3.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<DataType> DataTypes { get; set; }
        public DbSet<FormField> FormFields { get; set; }
        public DbSet<FormName> FormNames { get; set; }
        public DbSet<FormType> FormTypes { get; set; }
        public DbSet<FormValue> FormValues { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<KaziDocs3.Domain.Account> Account { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserTokens> Tokens { get; set; }

    }
}
