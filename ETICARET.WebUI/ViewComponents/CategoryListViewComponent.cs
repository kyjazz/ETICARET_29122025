using ETICARET.Business.Abstract;
using ETICARET.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ETICARET.WebUI.ViewComponents
{
    public class CategoryListViewComponent :ViewComponent
    {
        private readonly ICategoryService _categoryService;
        public CategoryListViewComponent(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        //IViewComponentResult çağırıldığında otomatik olarak çalışacak olan metotdur. ViewComponentResult döndürür.
        public IViewComponentResult Invoke()
        {
            var model = new CategoryListViewModel()
            {
                //RouteDatadan mevcut kategoriyi al
                //URL'de /products/electronics varsa electronics'i alır
                SelectedCategory = RouteData.Values["category"]?.ToString(),
                //tüm kategorileri alır ve modelin Categories özelliğine atar
                Categories = _categoryService.GetAll()
            };

            //Views/shared/components/categorylist/default.cshtml dosyasını render eder ve modele gönderir
            return View(model);
        }
    }
}
