using System.ComponentModel.DataAnnotations;

namespace ETICARET.WebUI.Models
{
    public class CommentModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yorum metni zorunludur.")]
        [StringLength(500, ErrorMessage = "Yorum en fazla 500 karakter olabilir.")]
        public string Text { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir ürün seçiniz.")]
        public int ProductId { get; set; }

        public string UserId { get; set; }
    }
}
