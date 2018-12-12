﻿using Extenso.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Demo.Data
{
    public class ApplicationDbContextFactory : IDbContextFactory
    {
        private readonly IConfiguration configuration;

        public ApplicationDbContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private DbContextOptions<ApplicationDbContext> options;

        private DbContextOptions<ApplicationDbContext> Options
        {
            get
            {
                if (options == null)
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                    options = optionsBuilder.Options;
                }
                return options;
            }
        }

        public DbContext GetContext()
        {
            return new ApplicationDbContext(Options);
        }
    }
}