using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Cart
    {
        public int Id { get; set; } //sepetin benzersiz kimliği
        public string UserId { get; set; } //sepetin ait olduğu kullanıcı kimliği
        public List<CartItem> CartItems { get; set; } //sepet içindeki ürünlerin listesi
    }

    public class CartItem
    {
        public int Id { get; set; } //sepet öğesinin benzersiz kimliği
        public int ProductId { get; set; } //ürün kimliği
        public Product Product { get; set; } //ürün nesnesiyle ilişkilendirme
        public Cart Cart { get; set; } //Sepet nesnesiyle ilişkilendirme
        public int CartId { get; set; } //sepet kimliği
        public int Quantity { get; set; } //ürün adedi
    }
}
