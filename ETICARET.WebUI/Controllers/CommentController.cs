using ETICARET.Business.Abstract;
using ETICARET.Entities;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ETICARET.WebUI.Controllers
{
    public class CommentController : Controller
    {
        
        private readonly ICommentService _commentService;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(ICommentService commentService, IProductService productService, UserManager<ApplicationUser> userManager)
        {
            _commentService = commentService;
            _productService = productService;
            _userManager = userManager;
        }

        public IActionResult Index(int? productId)
        {
            var comments = productId.HasValue
                ? _commentService.GetByProductId(productId.Value)
                : _commentService.GetAll();

            ViewBag.Products = new SelectList(_productService.GetAll(), "Id", "Name", productId);
            return View(comments);
        }

        [Authorize]
        public IActionResult Create(int? productId)
        {
            var model = new CommentModel
            {
                ProductId = productId ?? 0
            };

            PopulateProducts(productId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(CommentModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateProducts(model.ProductId);
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var entity = new Comment
            {
                Text = model.Text,
                ProductId = model.ProductId,
                UserId = userId,
                CreateOn = DateTime.Now
            };

            _commentService.Create(entity);
            return RedirectToAction(nameof(Index), new { productId = model.ProductId });
        }

        public IActionResult Details(int id)
        {
            var comment = _commentService.GetById(id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var comment = _commentService.GetById(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (!CanManageComment(comment))
            {
                return Forbid();
            }

            PopulateProducts(comment.ProductId);

            var model = new CommentModel
            {
                Id = comment.Id,
                Text = comment.Text,
                ProductId = comment.ProductId,
                UserId = comment.UserId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(CommentModel model)
        {
            if (!ModelState.IsValid)
            {
                PopulateProducts(model.ProductId);
                return View(model);
            }

            var comment = _commentService.GetById(model.Id);
            if (comment == null)
            {
                return NotFound();
            }

            if (!CanManageComment(comment))
            {
                return Forbid();
            }

            comment.Text = model.Text;
            comment.ProductId = model.ProductId;
            _commentService.Update(comment);

            return RedirectToAction(nameof(Index), new { productId = model.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var comment = _commentService.GetById(id);
            if (comment == null)
            {
                return NotFound();
            }

            if (!CanManageComment(comment))
            {
                return Forbid();
            }

            var productId = comment.ProductId;
            _commentService.Delete(comment);
            return RedirectToAction(nameof(Index), new { productId });
        }

        public IActionResult ShowProductComments(int id)
        {
            var comments = _commentService.GetByProductId(id);
            var usernames = _userManager.Users
                .Where(x => comments.Select(c => c.UserId).Contains(x.Id))
                .ToDictionary(x => x.Id, x => x.UserName ?? "Anonim");

            ViewBag.Usernames = usernames;
            return PartialView("~/Views/Shared/_PartialComments.cshtml", comments);
        }

        private void PopulateProducts(int? selectedProductId = null)
        {
            ViewBag.Products = _productService.GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                    Selected = selectedProductId.HasValue && selectedProductId == x.Id
                })
                .ToList();
        }

        private bool CanManageComment(Comment comment)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return User.IsInRole("admin") || comment.UserId == userId;
        }
    }
}