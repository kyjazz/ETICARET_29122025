using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class ProductCategory
    {
        public int CategoryId { get; set; } //kategori kimliği
        public Category Category { get; set; } //kategori nesnesiyle ilişkilendirme
        public int ProductId { get; set; } //ürün kimliği
        public Product Product { get; set; } //ürün nesnesiyle ilişkilendirme
    }
}
