using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } //sipariş kaleminin benzersiz kimliği
        public int OrderId { get; set; } //ilişkili sipariş kimliği
        public Order Order { get; set; } //sipariş nesnesiyle ilişkilendirme
        public int ProductId { get; set; } //ürün kimliği
        public Product Product { get; set; } //ürün nesnesiyle ilişkilendirme
        public int Quantity { get; set; } //ürün adedi
        public decimal Price { get; set; } //ürün fiyatı
    }
}
