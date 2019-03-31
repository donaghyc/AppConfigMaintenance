using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppConfigMaintenance.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace AppConfigMaintenance.DataAccess
{
    public class PaymentProcessingContext : DbContext
    {
        public PaymentProcessingContext(DbContextOptions<PaymentProcessingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ConfigSetting>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });
        }

        public DbSet<ConfigSetting> ConfigSettings { get; set; }
    }
}

