using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Iyzipay;
using Iyzipay.Model;
using Iyzipay.Request;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ETICARET.WebUI.Controllers
{
    public class CartController : Controller
    {
        private ICartService _cartService;
        private IProductService _productService;
        private IOrderService _orderService;
        private UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, IProductService productService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _productService = productService;
            _orderService = orderService;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));

            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(x => new CartItemModel()
                {
                    CartItemId = x.Id,
                    ProductId = x.ProductId,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    Quantity = x.Quantity,
                    ImageUrl = x.Product.Images[0].ImageUrl
                }).ToList()
            });
        }

        public IActionResult AddToCart(int productId, int quantity)
        {
            _cartService.AddToCart(_userManager.GetUserId(User), productId, quantity);
            return RedirectToAction("Index");
        }

        public IActionResult DeleteFromCart(int productId)
        {
            _cartService.DeleteFromCart(_userManager.GetUserId(User), productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            OrderModel orderModel = new OrderModel()
            {
                CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(x => new CartItemModel()
                    {
                        CartItemId = x.Id,
                        ProductId = x.ProductId,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        Quantity = x.Quantity,
                        ImageUrl = x.Product.Images[0].ImageUrl
                    }).ToList()
                }
            };
            return View(orderModel);
        }


        [HttpPost]
        public IActionResult Checkout(OrderModel model, string paymentMethod)
        {
            ModelState.Remove("CartModel");
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cart = _cartService.GetCartByUserId(userId);

                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(x => new CartItemModel()
                    {
                        CartItemId = x.Id,
                        ProductId = x.ProductId,
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        Quantity = x.Quantity,
                        ImageUrl = x.Product.Images[0].ImageUrl
                    }).ToList()
                };

                if (paymentMethod == "credit")
                {
                    //Iyzipay ödeme entegrasyonu yapılacak
                    var payment = PaymentProccess(model);

                    if (payment.Result.Status == "success")
                    {
                        SaveOrder(model,payment,userId);
                        ClearCart(cart.Id.ToString());

                        TempData.Put("message", new ResultModel()
                        {
                            Title = "Başarılı",
                            Message = "Siparişiniz başarıyla tamamlandı.",
                            Css = "success"
                        });
                    }
                    else
                    {
                        TempData.Put("message", new ResultModel()
                        {
                            Title = "Hata",
                            Message = "Ödeme işlemi sırasında bir hata oluştu.",
                            Css = "danger"
                        });
                    }
                }
                else
                {
                    SaveOrder(model, userId);
                    ClearCart(cart.Id.ToString());
                    TempData.Put("message", new ResultModel()
                    {
                        Title = "Başarılı",
                        Message = "Siparişiniz başarıyla tamamlandı.",
                        Css = "success"
                    });
                }
            }
            return View(model);
        }
        private void ClearCart(string userId)
        {
            _cartService.ClearCart(userId);
        }



        private void SaveOrder(OrderModel model,string userId)
        {
            Order order = new Order()
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = EnumOrderState.completed,
                PaymentTypes = EnumPaymentTypes.Eft,
                PaymentToken = Guid.NewGuid().ToString(),
                ConversionId = Guid.NewGuid().ToString(),
                PaymentId = Guid.NewGuid().ToString(),
                OrderNote = model.OrderNote,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                Phone = model.Phone,
                Email = model.Email,
                UserId = userId,
            };

            //sepetteki ürünleri siparişe OrderItem olarak ekleyeceğiz
            foreach (var cartItem in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = cartItem.Price,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };

                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }

        private void SaveOrder(OrderModel model, Task<Payment> payment, string userId)
        {
            Order order = new Order()
            {
                OrderNumber = Guid.NewGuid().ToString(),
                OrderState = EnumOrderState.completed,
                PaymentTypes = EnumPaymentTypes.CreditCard,
                PaymentToken = Guid.NewGuid().ToString(),
                ConversionId = payment.Result.ConversationId,
                PaymentId = payment.Result.PaymentId,
                OrderNote = model.OrderNote,
                OrderDate = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                City = model.City,
                Phone = model.Phone,
                Email = model.Email,
                UserId = userId,
            };

            //sepetteki ürünleri siparişe OrderItem olarak ekleyeceğiz
            foreach (var cartItem in model.CartModel.CartItems)
            {
                var orderItem = new Entities.OrderItem()
                {
                    Price = cartItem.Price,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity
                };

                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);
        }


        private async Task<Payment> PaymentProccess(OrderModel model)
        {
            Options options = new Options() 
            { 
                BaseUrl = "https://sandbox-api.iyzipay.com",
                ApiKey = "sandbox-cNnJEaoyNt0sCREL4nOq8PajTLQwWeXz",
                SecretKey = "sandbox-cmJxJfaGlVarqNV3c5ZQcMTwVNh8qswx"
            };

            string externalIpString = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
            var externalIp = IPAddress.Parse(externalIpString);

            //Ödeme isteği oluşturulacak
            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();//Türkçe dilinde işlem yapılacak
            request.ConversationId = Guid.NewGuid().ToString(); //Her ödeme isteği için benzersiz bir id oluşturulacak
            request.Price = model.CartModel.TotalPrice().ToString().Split(',')[0]; //Ödeme tutarı
            request.PaidPrice = model.CartModel.TotalPrice().ToString().Split(',')[0]; //Ödenen tutar
            request.Currency = Currency.TRY.ToString(); //Para birimi
            request.Installment = 1; //Taksit sayısı
            request.BasketId = model.CartModel.CartId.ToString(); //Sepet id'si
            request.PaymentChannel = PaymentChannel.WEB.ToString(); //Ödeme kanalı web üzerinden yapılacak
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString(); //Ödeme grubu ürün olarak belirlenecek

            PaymentCard paymentCard = new PaymentCard() 
            {
                CardHolderName = model.CardName,
                CardNumber = model.CardNumber,
                ExpireMonth = model.ExpirationMonth,
                ExpireYear = model.ExpirationYear,
                Cvc = model.CVV,
                RegisterCard = 0 //Kart bilgilerini kaydetme
            };
            request.PaymentCard = paymentCard;

            Buyer buyer = new Buyer() 
            {
                Id = _userManager.GetUserId(User),
                Name = model.FirstName,
                Surname = model.LastName,
                GsmNumber = model.Phone,
                Email = model.Email,
                IdentityNumber = "11111111111", //TC kimlik numarası
                RegistrationAddress = model.Address,
                Ip = externalIp.ToString(),
                City = model.City,
                Country = "Turkey",
                ZipCode = "34000"
            };
            request.Buyer = buyer;

            Address address = new Address()
            {
                ContactName = model.FirstName + " " + model.LastName,
                City = model.City,
                Country = "Turkey",
                Description = model.Address,
                ZipCode = "34000"
            };
            request.ShippingAddress = address;
            request.BillingAddress = address;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var cartItem in model.CartModel.CartItems)
            {
                basketItem = new BasketItem()
                {
                    Id = cartItem.ProductId.ToString(),
                    Name = cartItem.Name,
                    Category1 = _productService.GetProductDetail(cartItem.ProductId).ProductCategories.FirstOrDefault().CategoryId.ToString(),
                    ItemType = BasketItemType.PHYSICAL.ToString(),
                    Price = (cartItem.Price * cartItem.Quantity).ToString().Split(',')[0]
                };
                basketItems.Add(basketItem);
            }
            request.BasketItems = basketItems;
            Payment payment = await Payment.Create(request, options);
            return payment; //Ödeme isteği gönderilecek ve sonuç döndürülecek(içinde Status,PaymentId,conversationId)
        }

        public IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = _orderService.GetOrders(userId);
            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;

            foreach (var order in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentTypes = order.PaymentTypes;
                orderModel.FirstName = order.FirstName;
                orderModel.LastName = order.LastName;
                orderModel.Address = order.Address;
                orderModel.City = order.City;
                orderModel.Phone = order.Phone;
                orderModel.Email = order.Email;
                orderModel.OrderNote = order.OrderNote;
                orderModel.OrderItems = order.OrderItems.Select(x => new OrderItemModel()
                {
                    OrderItemId = x.Id,
                    Name = x.Product.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ImageUrl = x.Product.Images[0].ImageUrl
                }).ToList();
                orderListModel.Add(orderModel);
            }
            return View(orderListModel);

        }
    }
}
