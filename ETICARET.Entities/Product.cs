using System.ComponentModel.DataAnnotations;

namespace ETICARET.Entities
{
    public class Product
    {
        public int Id { get; set; } //ürünün benzersiz kimliği
        public string Name { get; set; } //ürün adı
        public string Description { get; set; } //ürün açıklaması
        public List<Image> Images { get; set; } //ürün resimleri
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat negatif olamaz.")]
        public decimal Price { get; set; } //ürün fiyatı
        public List<ProductCategory> ProductCategories { get; set; } //ürünün kategorileri
        public List<Comment> Comments { get; set; } //ürün yorumları

        public Product()
        {
            Images = new List<Image>(); //boş liste ile başlatma
            ProductCategories = new List<ProductCategory>(); //boş liste ile başlatma
            Comments = new List<Comment>(); //boş liste ile başlatma
        }
    }
}
