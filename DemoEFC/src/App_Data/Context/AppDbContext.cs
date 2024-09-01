using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using src.App_Data.Entities;
using System;
using System.Collections;
using System.Reflection.Metadata;

namespace src.App_Data.Context
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(connectionString:
                    App.Instance._DataConfiguration.GetSection("Database:ConnectionString").Value
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // if (System.Diagnostics.Debugger.IsAttached == false) System.Diagnostics.Debugger.Launch();

            base.OnModelCreating(modelBuilder);

            // Only for sql server
            // modelBuilder.UseCollation("Turkish_CI_AS");

            modelBuilder.ApplyConfigurationsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            // In order to seed the data by this way, migrations (add migration and update database) must be run
            // modelBuilder.SeedData();
        }

        public virtual DbSet<Il> Iller => Set<Il>();
        public virtual DbSet<Ilce> Ilceler => Set<Ilce>();
        public virtual DbSet<SemtBucakBelde> SemtBucakBeldeler => Set<SemtBucakBelde>();
        public virtual DbSet<Mahalle> Mahalleler => Set<Mahalle>();
        public DbSet<State> States => Set<State>();
        public DbSet<City> Cities => Set<City>();
    }
}