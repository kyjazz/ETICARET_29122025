using System.ComponentModel.DataAnnotations.Schema;

namespace ETICARET.Entities
{
    [Table("Image")] //veritabanı tablosu adı
    public class Image
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; } //resim URL'si
        public int ProductId { get; set; } //ilgili ürün kimliği
        public Product Product { get; set; } //ürün nesnesiyle ilişkilendirme
    }
}
