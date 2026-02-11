using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Concrete.EfCore
{
    public class EfCoreCategoryDal : EfCoreGenericRepository<Category, DataContext>, ICategoryDal
    {
        public void DeleteFromCategory(int categoryId, int ProductId)
        {
            using (var context = new DataContext())
            {
                var cmd = "delete from ProductCategories where CategoryId=@p0 and ProductId=@p1";
                context.Database.ExecuteSqlRaw(cmd, categoryId,ProductId);
            }
        }

        //Kategori bilgileri ile birlikte o kategoriye ait ürünleri getirme
        public Category GetByIdWithProducts(int id)
        {
            using (var context = new DataContext())
            {
                return context.Categories
                    .Where(i=>i.Id == id)
                    .Include(i => i.ProductCategories)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(i => i.Images)
                    .FirstOrDefault();
            }
        }

        public override void Delete(Category entity)
        {
            using (var context = new DataContext())
            {
                context.Categories.Remove(entity);
                context.SaveChanges();
            }
        }
    }
}
