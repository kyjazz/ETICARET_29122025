using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Abstract
{
    public interface ICartService
    {
        void InitialCart(string userId); //kullanıcı için boş bir sepet oluşturur
        Cart GetCartByUserId(string userId); //kullanıcıya ait sepeti getirir
        void AddToCart(string userId, int productId, int quantity); //sepete ürün ekler
        void DeleteFromCart(string userId, int productId); //sepetten ürün siler
        void ClearCart(string cartId); //sepeti boşaltır
        /*
            Sepet işlemlerini Controller'dan bağımsız olarak yönetmek için bir servis katmanı gereklidir.
            Kullanıcı giriş yaptığında otomatik olarak sepet oluşturma, ürün ekleme, silme ve sepeti temizleme gibi işlemler bu servis aracılığıyla gerçekleştirilir.
         */
    }
}
