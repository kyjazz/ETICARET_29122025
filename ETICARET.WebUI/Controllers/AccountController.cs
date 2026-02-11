using ETICARET.Business.Abstract;
using ETICARET.WebUI.EmailService;
using ETICARET.WebUI.Extensions;
using ETICARET.WebUI.Identity;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private ICartService _cartService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        //get
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FullName = model.FullName
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var callbackUrl = Url.Action("ConfirmEmail", "Account", new
                {
                    userId = user.Id,
                    token = code
                });
                string siteUrl = "https://localhost:7196";
                string activeUrl = $"{siteUrl}{callbackUrl}";
                string body = $"Hesabınızı Onaylayın. <br> <br> Hesabınızı onaylamak için linke <a href='{activeUrl}'>tıklayın</a>";
                MailHelper.SendEmail(body, model.Email, "Eticaret hesap aktifleştirme onayı UBY");
                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Geçersiz token",
                    Message = "Geçersiz token",
                    Css = "danger"
                });

                return Redirect("~");
            }

            //usermanager aracılığı ile kullanıcıyı bulalım
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    _cartService.InitialCart(userId);
                    TempData.Put("message", new ResultModel()
                    {
                        Title = "Hesabınız onaylandı",
                        Message = "Hesabınız onaylandı",
                        Css = "success"
                    });
                    return RedirectToAction("Login", "Account");
                }
            }
            TempData.Put("message", new ResultModel()
            {
                Title = "Hesabınız onaylanmadı",
                Message = "Hesabınız onaylanmadı",
                Css = "danger"
            });
            return Redirect("~");
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            ModelState.Remove("ReturnUrl");
            if (!ModelState.IsValid)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Lütfen bilgilerinizi kontrol ediniz",
                    Css = "danger"
                });
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null)
            {
                ModelState.AddModelError("", "Bu email adresi ile kayıtlı kullanıcı bulunamadı");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl ?? "~/");
            }
            if (result.IsLockedOut)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hesabınız kilitlenmiştir",
                    Message = "Hesabınız kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz",
                    Css = "warning"
                });
            }
            ModelState.AddModelError("", "Email adresiniz veya şifreniz yanlış");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new ResultModel()
            {
                Title = "Başarılı",
                Message = "Çıkış işlemi başarılı",
                Css = "success"
            });
            return Redirect("~/");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Lütfen email adresinizi giriniz",
                    Css = "danger"
                });
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Bu email adresi ile kayıtlı kullanıcı bulunamadı",
                    Css = "danger"
                });
                return View();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new
            {
                token = code
            });
            string siteUrl = "https://localhost:7196";
            string activeUrl = $"{siteUrl}{callbackUrl}";
            string body = $"Şifrenizi yenilemek için linke <a href='{activeUrl}'>tıklayın</a>";
            MailHelper.SendEmail(body, email, "Eticaret Şifre Sıfırlama UBY");
            TempData.Put("message", new ResultModel()
            {
                Title = "Başarılı",
                Message = "Şifre sıfırlama linki email adresinize gönderilmiştir",
                Css = "success"
            });
            return RedirectToAction("Login");
        }

        public IActionResult ResetPassword(string token)
        {
            if (token == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Geçersiz token",
                    Css = "danger"
                });
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordModel { Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Bu email adresi ile kayıtlı kullanıcı bulunamadı",
                    Css = "danger"
                });
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Başarılı",
                    Message = "Şifreniz başarıyla sıfırlanmıştır",
                    Css = "success"
                });
                return RedirectToAction("Login");
            }
            TempData.Put("message", new ResultModel()
            {
                Title = "Hata",
                Message = "Şifre sıfırlama işlemi başarısız oldu",
                Css = "danger"
            });
            return View(model);
        }

        //profil yönetimi
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Kullanıcı bulunamadı",
                    Css = "danger"
                });
            }
            var model = new AccountModel()
            {
                FullName = user.FullName,
                Email = user.Email,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Manage(AccountModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Hata",
                    Message = "Kullanıcı bulunamadı",
                    Css = "danger"
                });
                return RedirectToAction("Login");
            }
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.UserName;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData.Put("message", new ResultModel()
                {
                    Title = "Başarılı",
                    Message = "Profiliniz başarıyla güncellenmiştir",
                    Css = "success"
                });
                return RedirectToAction("Manage");
            }
            TempData.Put("message", new ResultModel()
            {
                Title = "Hata",
                Message = "Profil güncelleme işlemi başarısız oldu",
                Css = "danger"
            });
            return View(model);
        }
    }
}
