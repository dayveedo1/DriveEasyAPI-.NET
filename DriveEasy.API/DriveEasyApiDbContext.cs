using DriveEasy.API.DriveEasy.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DriveEasy.API
{
    public class DriveEasyApiDbContext : IdentityDbContext<User>
    {
        private readonly string? _connectionString;

        public DriveEasyApiDbContext()
        {
            
        }

        public DriveEasyApiDbContext(DbContextOptions<DriveEasyApiDbContext> options) : base(options)
        {
            
        }

        private DriveEasyApiDbContext(DbContextOptions<DriveEasyApiDbContext> options, IConfiguration config) : base(options)
        {
            _connectionString = config.GetConnectionString("DriveEasyAPI");
        }

        #region DbModels

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Price> Prices { get; set; }

        #endregion


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString, sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);

                    sqlOptions.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds);

                });

                base.OnConfiguring(optionsBuilder);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
