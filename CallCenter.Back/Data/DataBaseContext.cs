using CallCenter.Model;
using Microsoft.EntityFrameworkCore;

namespace CallCenter.Back.Data
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Call> Calls { get; set; }
        public DbSet<Person> Persons { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Property(p => p.PersonId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Person>().Property(p => p.PersonId).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.FirstName).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.PhoneNumber).IsRequired();
            modelBuilder.Entity<Person>().Property(p => p.FirstName).HasMaxLength(30);
            modelBuilder.Entity<Person>().Property(p => p.LastName).HasMaxLength(30);
            modelBuilder.Entity<Person>().Property(p => p.Patronymic).HasMaxLength(30);
            //modelBuilder.Entity<Person>().Property(p => p.Gender).HasColumnType("INT");

            modelBuilder.Entity<Call>().Property(c => c.CallId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Call>().Property(c => c.CallId).IsRequired();
            modelBuilder.Entity<Call>().Property(c => c.CallDate).IsRequired();
            modelBuilder.Entity<Call>().Property(c => c.CallReport).IsRequired();

            modelBuilder.Entity<Person>()
                .HasMany(p => p.Calls)
                .WithOne()
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
