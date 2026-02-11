using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface IProductService
    {
        Product GetById(int id); //belirli bir ürüne ait bilgileri getirir
        List<Product> GetProductByCategory(string category, int page, int pageSize); // sayfalama ile belirli bir kategorideki ürünleri getir
        List<Product> GetAll(); //tüm ürünleri getirir
        Product GetProductDetail(int id); //belirli bir ürüne ait detaylı bilgileri getirir
        void Create(Product entity); //yeni ürün oluşturur
        void Update(Product entity, int[] categoryIds); //var olan ürünü günceller
        void Delete(Product entity); //var olan ürünü siler
        int GetCountByCategory(string category); //belirli bir kategorideki ürün sayısını getirir
    }
}
