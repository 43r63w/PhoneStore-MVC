using Microsoft.AspNetCore.Mvc;
using PhoneStore.DAL.Data;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Upsert(int id)
        {
            if (id == 0)
            {
                Category category = new Category();

                return View(category);
            }

            var categoryFromDb = await _unitOfWork.Category.GetAsync(u => u.Id == id);

            return View(categoryFromDb);

        }
        [HttpPost]
        public async Task<IActionResult> Upsert(Category category)
        {
            if (category.Id == 0)
            {
                _unitOfWork.Category.AddAsync(category);
            }
            else
            {
                _unitOfWork.Category.Update(category);
            }
            _unitOfWork.Save();
            TempData["success"] = "Товар оновленно/доданно!";
            return RedirectToAction("Index");
        }

        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Category> categoriesList = _unitOfWork.Category.GetAll().ToList();
            return Json(new { data = categoriesList });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryFromDb = await _unitOfWork.Category.GetAsync(u => u.Id == id);

            _unitOfWork.Category.Remove(categoryFromDb);
            _unitOfWork.Save();



            return Json(new { success = true, message = "Категорію видаленно!" });
        }

        #endregion


    }
}
