using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICategoryService
    {
        Category GetById(int id); //belirli bir kategoriye ait bilgileri getirir
        Category GetByWithProducts(int id); //belirli bir kategoriye ait tüm ürünleri getirir
        List<Category> GetAll(); //tüm kategorileri getirir
        void Create(Category entity); //yeni kategori oluşturur
        void Update(Category entity); //var olan kategoriyi günceller
        void Delete(Category entity); //var olan kategoriyi siler
        void DeleteFromCategory(int categoryId, int productId); // belirli bir ürünü kategoriden siler
    }
}