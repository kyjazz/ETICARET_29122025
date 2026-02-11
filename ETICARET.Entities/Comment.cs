using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Entities
{
    public class Comment
    {
        public int Id { get; set; }//yorumun benzersiz kimliği
        public string Text { get; set; } //yorum metni
        public int ProductId { get; set; } //yorumun ait olduğu ürün kimliği
        public Product Product { get; set; } //ürün nesnesiyle ilişkilendirme
        public string UserId { get; set; } //yorum yapan kullanıcı kimliği
        public DateTime CreateOn { get; set; } //yorumun oluşturulma tarihi
    }
}
