using BookShop.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShop.Models;
using BookShop.Models.ViewModels;
using BookShop.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace BookShop.Models
{
    public class BookShopContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(local);Database=BookShopDB2;Trusted_Connection=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new Author_BookMap());
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new Order_BookMap());
            modelBuilder.ApplyConfiguration(new Book_TranslatorMap());
            modelBuilder.ApplyConfiguration(new Book_CategoryMap());

            //Use view in databse as query
            modelBuilder.Query<ReadAllBook>().ToView("ReadAllBooks");

            //Use Default values for fields by SQL standards
            modelBuilder.Entity<Book>().Property(b => b.Delete).HasDefaultValueSql("0");

            //Query Filter to show specific data
            //modelBuilder.Entity<Book>().HasQueryFilter(b => b.Delete == false);
            modelBuilder.Entity<Book>().HasQueryFilter(b => (bool)!b.Delete);
            //IdentityDBContext mixing with BookShopDBContext
            
            modelBuilder.Entity<ApplicationRole>()
                .ToTable("AspNetRoles").ToTable("AppRoles");

            modelBuilder.Entity<ApplicationUserRole>().ToTable("AppUserRole");
            modelBuilder.Entity<ApplicationUserRole>()
                .HasOne(userRole => userRole.Role)
                .WithMany(role => role.Users).HasForeignKey(r => r.RoleId);

            modelBuilder.Entity<ApplicationUser>().ToTable("AppUser");
            modelBuilder.Entity<ApplicationUserRole>()
                .HasOne(userRole => userRole.User)
                .WithMany(role => role.Roles).HasForeignKey(r => r.UserId);

            modelBuilder.Entity<ApplicationRoleClaim>().ToTable("AppRoleClaim");
            modelBuilder.Entity<ApplicationRoleClaim>()
                .HasOne(roleClaim => roleClaim.Role)
                .WithMany(claim => claim.Claims).HasForeignKey(c => c.RoleId);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Translator> Translator { get; set; }
        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Author_Book> Author_Books { get; set; }
        public DbSet<Order_Book> Order_Books { get; set; }
        public DbSet<Book_Category> Book_Categories { get; set; }
        public DbSet<Book_Translator> Book_Translators { get; set; }
        public DbQuery<ReadAllBook> ReadAllBooks { get; set; }

        [DbFunction("GetAllAuthor","dbo")]
        public static string GetAllAuthors(int BookID)
        {
            throw new NotImplementedException();
        }

        [DbFunction("GetAllTranslator","dbo")]
        public static string GetAllTranslators(int BookID)
        {
            throw new NotImplementedException();
        }

        [DbFunction("GetAllCategory","dbo")]
        public static string GetAllCategories(int BookID)
        {
            throw new NotImplementedException();
        }
    }
}
