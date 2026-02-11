using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreProductDal : EfCoreGenericRepository<Product, DataContext>, IProductDal
    {
        public int GetCountByCategory(string category)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.AsQueryable();

                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    products = products
                        .Include(i=>i.ProductCategories)
                        .ThenInclude(i=>i.Category)
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                    return products.Count();
                }
                else // tüm ürünlerin sayısını döndür
                {
                    return products.Include(i => i.ProductCategories)
                                      .ThenInclude(i => i.Category)
                                      .Where(i => i.ProductCategories.Any())
                                      .Count();
                }
            }
        }

        public Product GetProductDetails(int id)
        {
            using (var context = new DataContext())
            {
                return context.Products
                    .Where(i=> i.Id == id)
                    .Include("Images")
                    .Include("Comments")
                    .Include(i=>i.ProductCategories)
                    .ThenInclude(i=>i.Category)
                    .FirstOrDefault();
            }
        }

        //belirtilen kategoriye göre ürünleri sayfalayarak getirir
        //her sayfada 10 ürün gösterildiğini varsayarsak
        //1.sayfada => 1-10
        //2.sayfada => 11-20
        //3.sayfada => 21-30
        //kullanıcı 3.sayfayı istiyorsa 21. üründen başlamalıyız
        //(page - 1) * pageSize
        //örneğin page=3 ve pageSize=10 ise
        // (3-1) * 10 = 20
        //yani 20 ürün atlanmalı ve 21. üründen başlanmalı
        //sonrasında ise 10 ürün alınmalı
        public List<Product> GetProductsByCategory(string category, int page, int pageSize)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.Include("Images").AsQueryable();//ürünleri ve resimlerini yükleyerek sorguya hazır hale getir.
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    products = products
                        .Include(i => i.ProductCategories)
                        .ThenInclude(i => i.Category)
                        .Where(i => i.ProductCategories.Any(a => a.Category.Name.ToLower() == category.ToLower()));
                }
                return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();//sayfalama işlemi yaparak ürünleri listele
            }
        }

        /// <summary>
        /// Mevcut bir ürünü ve kategori ilişkilerini günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek ürün bilgilerini içeren Product nesnesi</param>
        /// <param name="categoryIds">Ürünle ilişkilendirilecek kategori ID'lerinin dizisi</param>
        /// <remarks>
        /// Bu metod ürünün Name, Price, Description, Images özelliklerini günceller.
        /// Ayrıca mevcut kategori ilişkilerini siler ve yeni kategori ilişkilerini oluşturur.
        /// </remarks>
        public void Update(Product entity, int[] categoryIds)
        {
            using (var context = new DataContext())
            {
                var products = context.Products.Include(i => i.ProductCategories).FirstOrDefault(i => i.Id == entity.Id); //Güncellenecek ürünü Id ile bul
                if (products is not null)
                {
                    products.Price = entity.Price;
                    products.Name = entity.Name;
                    products.Description = entity.Description;
                    products.ProductCategories = categoryIds.Select(catId => new ProductCategory()
                    {
                        ProductId = entity.Id,
                        CategoryId = catId
                    }).ToList(); //Yeni kategori ilişkilerini oluştur
                    products.Images = entity.Images; //Resimleri güncelle
                }
                context.SaveChanges(); //Değişiklikleri kaydet
            }
        }

        //Ürünü ve ilişkili tüm resimlerini siler
        public override void Delete(Product entity)
        {
            using (var context = new DataContext())
            {
                context.Images.RemoveRange(entity.Images);
                context.Products.Remove(entity);
                context.SaveChanges();
            }
        }

        //Tüm ürünleri veya belirli bir filtreye göre ürünleri getirir
        public override List<Product> GetAll(Expression<Func<Product,bool>>filter = null)
        {
            using (var context = new DataContext())
            {
                return filter == null
                    ? context.Products.Include("Images").ToList()
                    : context.Products.Include("Images").Where(filter).ToList();
            }
        }
    }
}
