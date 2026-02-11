using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.DataAccess.Abstract
{
    public interface ICategoryDal : IRepository<Category>
    {
        void DeleteFromCategory(int categoryId, int ProductId);//belirtilen ürünü belirtilen kategoriden siler
        Category GetByIdWithProducts(int id); //kategori ile birlikte ürünleri getirir
    }
}
/*
 kategori yönetimi için bazı özel işlemler gerektiğinden bu arayüz tanımlanmıştır.
örneğin , bir kategoriye bağlı ürünleri getirmek için GetByIdWithProducts metodu kullanılır.
 */