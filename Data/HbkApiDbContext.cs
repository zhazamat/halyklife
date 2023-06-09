﻿using hbk.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace hbk.Data
{
    public class HbkApiDbContext : DbContext
    {
        public HbkApiDbContext()
        {
        }
       
        public HbkApiDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ThanksBoard>()
                .HasOne<Employee>(e=>e.Receiver)
                .WithMany(t=>t.Messages)
                .HasForeignKey(m=>m.ReceiverId);
            
        /* modelBuilder.Entity<EmployeeMarket>()
        .HasKey(em => new { em.EmployeeId, em.MarketId });  */
    modelBuilder.Entity<EmployeeMarket>()
        .HasOne(em => em.Employee)
        .WithMany(e => e.EmployeeMarkets)
        .HasForeignKey(em => em.EmployeeId);
           
            modelBuilder.Entity<EmployeeMarket>()
                  .HasOne(em => em.Market)
                  .WithMany(m => m.EmployeeMarkets)
                  .HasForeignKey(em => em.MarketId);
            /*
                     modelBuilder.Entity<ThanksBoard>()
                         .HasOne<Category>(e => e.Category)
                         .WithMany(t => t.Messages)
                         .HasForeignKey(m => m.CategoryId);


                      /*  modelBuilder.Entity<Category>()
                        .HasOne<Employee>(e => e.Employee)
                        .WithMany(t => t.Categories)
                        .HasForeignKey(m => m.EmployeeId);*/

        }

      

        public DbSet<Employee> Employees { get; set; }
        public DbSet<ThanksBoard> ThanksBoards { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EmployeeMarket> EmployeeMarkets { get; set; }
     //   public DbSet<ProductCategory> ProductCategory { get; set; }
      

       
    }
}
