using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Category
    {
        public int Id { get; set; } //kategori benzersiz kimliği
        public string Name { get; set; } //kategori adı
        public List<ProductCategory> ProductCategories { get; set; } //kategoriye ait ürünlerin listes
        public Category()
        {
            ProductCategories = new List<ProductCategory>(); //boş liste ile başlatma
        }
    }
}
