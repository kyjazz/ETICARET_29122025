using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface IProductDal : IRepository<Product>
    {
        int GetCountByCategory(string category);//belirtilen kategoriye ait ürünlerin sayısını getirir
        Product GetProductDetails(int id); //ürün detaylarını getirir
        List<Product> GetProductsByCategory(string category, int page, int pageSize); //sayfalama ile belirli bir kategorideki ürünleri getirir
        void Update(Product entity, int[] categoryIds); // ürünü ve ona bağlı kategorileri günceller
    }
}
