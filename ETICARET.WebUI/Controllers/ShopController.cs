using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class ShopController : Controller
    {
        
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShopController(IProductService productService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _userManager = userManager;
        }

        [Route("products/{category?}")]
        public IActionResult List(string category, int page = 1)
        {
            const int pageSize = 4;
            var products = new ProductListModel()
            {
                PageInfo = new PageInfo()
                {
                    TotalItems = _productService.GetCountByCategory(category),
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    CurrentCategory = category
                },
                Products = _productService.GetProductByCategory(category, page, pageSize)
            };
            return View(products);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = _productService.GetProductDetail(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            var usernames = new Dictionary<string, string>();
            foreach (var userId in product.Comments.Select(c => c.UserId).Distinct())
            {
                if (string.IsNullOrWhiteSpace(userId))
                {
                    continue;
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    usernames[userId] = user.UserName;
                }
            }

            ViewBag.Usernames = usernames;

            return View(new ProductDetailsModel()
            {
                Product = product,
                Categories = product.ProductCategories.Select(i => i.Category).ToList(),
                
                Comments = product.Comments.OrderByDescending(c => c.CreateOn).ToList()
            });
        }
    }
}