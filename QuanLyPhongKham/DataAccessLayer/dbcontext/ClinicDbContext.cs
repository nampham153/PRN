using DataAccessLayer.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.dbcontext
{
    public class ClinicDbContext : DbContext
    {
        public ClinicDbContext()
        {

        }

        public ClinicDbContext(DbContextOptions<ClinicDbContext> options) : base(options) { }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithOne(u => u.Account)
                .HasForeignKey<User>(u => u.AccountId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Patient)
                .WithOne(p => p.Account)
                .HasForeignKey<Patient>(p => p.AccountId);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.RefreshToken)
                .WithOne(r => r.Account)
                .HasForeignKey<RefreshToken>(r => r.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.User)
                .WithMany(d => d.MedicalRecords)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict); // tránh xóa bác sĩ thì xóa hồ sơ

            modelBuilder.Entity<TestResult>()
                .HasOne(t => t.User)
                .WithMany(u => u.TestResults)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}


