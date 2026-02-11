
using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ETICARET.WebUI.Controllers
{
    public class CommentController : Controller
    {
        
        private readonly IProductService _productService;
        private readonly ICommentService _commentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(IProductService productService, ICommentService commentService, UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> ShowProductComments(int id)
        {
            var product = _productService.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }

            await PopulateUsernamesAsync(product.Comments);
            return PartialView("_PartialComments", product.Comments.OrderByDescending(c => c.CreateOn).ToList());
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(int productId, string text)
        {
            
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest(new { isSuccess = false, message = "Yorum boş olamaz." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _commentService.Create(new Comment
            {
                ProductId = productId,
                Text = text.Trim(),
                UserId = userId,
                CreateOn = DateTime.Now
            });

            return Json(new { isSuccess = true });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Edit(int id, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest(new { isSuccess = false, message = "Yorum boş olamaz." });
            }

            var comment = _commentService.GetById(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment == null)
            {
                return NotFound(new { isSuccess = false, message = "Yorum bulunamadı." });
            }

            if (comment.UserId != userId)
            {
                return Forbid();
            }

            comment.Text = text.Trim();
            _commentService.Update(comment);

            return Json(new { isSuccess = true });
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var comment = _commentService.GetById(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (comment == null)
            {
                return NotFound(new { isSuccess = false, message = "Yorum bulunamadı." });
            }

            if (comment.UserId != userId)
            {
                return Forbid();
            }

            _commentService.Delete(comment);

            return Json(new { isSuccess = true });
        }

        private async Task PopulateUsernamesAsync(List<Comment> comments)
        {
            var usernames = new Dictionary<string, string>();

            foreach (var userId in comments.Select(c => c.UserId).Distinct())
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
        }
    }
}