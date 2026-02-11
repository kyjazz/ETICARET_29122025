using ETICARET.Business.Abstract;
using ETICARET.DataAccess.Abstract;
using ETICARET.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETICARET.Business.Concrete
{
    public class CartManager : ICartService
    {
        private ICartDal _cartDal; //veri erişim katmanı bağımlılığı
        public CartManager(ICartDal cartDal)
        {
            _cartDal = cartDal;
        }
        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetCartByUserId(userId); //kullanıcının sepetini al
            if (cart is not null)
            {
                //sepette ürün var mı kontrol et
                var index = cart.CartItems.FindIndex(x => x.ProductId == productId);

                if (index < 0) //eğer sepette yoksa yeni ekle
                {
                    cart.CartItems.Add(
                        new CartItem
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            CartId = cart.Id
                        }
                    );
                }
                else
                {
                    cart.CartItems[index].Quantity += quantity; //sepette varsa miktarını güncelle
                }
            }
            _cartDal.Update(cart); //sepeti güncelle
        }

        public void ClearCart(string cartId)
        {
            _cartDal.ClearCart(cartId);
        }

        //sepetten belirli bir ürünü sil
        public void DeleteFromCart(string userId, int productId)
        {
            var cart = GetCartByUserId(userId);
            if (cart != null)
            {
                _cartDal.DeleteFromCart(cart.Id,productId);
            }
        }

        //kullanıcının sepetini getir
        public Cart GetCartByUserId(string userId)
        {
            return _cartDal.GetCartByUserId(userId);
        }

        //Yeni bir kullanıcı için boş sepet oluştur
        public void InitialCart(string userId)
        {
            Cart cart = new Cart
            {
                UserId = userId,
            };
            _cartDal.Create(cart);
        }
    }
}
