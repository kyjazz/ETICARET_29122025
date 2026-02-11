using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ETICARET.DataAccess.Abstract
{
    public interface ICartDal : IRepository<Cart>
    {
        void ClearCart(string cartId); //belirtilen sepetteki tüm ürünleri temizler
        void DeleteFromCart(int cartId, int productId); //belirtilen ürünü belirtilen sepetten siler
        Cart GetCartByUserId(string userId); //belirtilen kullanıcıya ait sepeti getirir
    }
}
/* Bu Katman Neden Var?
 * Sepet işlemleri için özel veri erişim yöntemlerini tanımlamak ve uygulamak amacıyla kullanılır.
 * ClearCart tüm ürünleri sepetten temizlemek için kullanılır. 
 * DeleteFromCart belirli bir ürünü sepetten silmek için kullanılır.
 * GetCartByUserId belirli bir kullanıcıya ait sepeti almak için kullanılır.
 */
