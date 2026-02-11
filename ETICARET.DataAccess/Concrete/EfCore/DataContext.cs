using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class DataContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=204-HOCAPC1;Database=ETICARET;uid=sa;pwd=1;TrustServerCertificate=True");
        }

        //veritabanındaki ilişkileri ve kuralları burada belirteceğiz
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //ProductCategory tablosu için birleşik anahtar tanımlaması(ProductId ve CategoryId birlikte eşsiz olmalı)
            modelBuilder.Entity<ProductCategory>().HasKey(c => new { c.ProductId, c.CategoryId });
        }

        //Veritabanı tablolarını tanımlayan DbSet özellikleri
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}
