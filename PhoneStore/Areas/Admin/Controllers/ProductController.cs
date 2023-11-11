using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using PhoneStore.Entities.ViewModels;
using SQLitePCL;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Upsert(int id)
        {
            ProductVM productVM = new()
            {
                Product = new Product(),

                CategoryLists = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                })
            };
            if (id == 0)
            {
                return View(productVM);
            };

            productVM.Product = await _unitOfWork.Product.GetAsync(u => u.Id == id, IncludeProperties: "ProductImages");


            return View(productVM);

        }

        [HttpPost]
        public ActionResult Upsert(ProductVM productVM, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();

                if (files != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;

                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product-" + productVM.Product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (Directory.Exists(finalPath) == false)
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage image = new ProductImage()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = productVM.Product.Id,
                        };


                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(image);
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }
                TempData["success"] = "Товар оновленно/доданно";
                return RedirectToAction("Index");

            }
            else
            {
                productVM.CategoryLists = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });

                return View(productVM);
            }

        }


        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var imageToBeDeleted = await _unitOfWork.ProductImage.GetAsync(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;

            if (imageToBeDeleted.ImageUrl != null)
            {
                var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.Trim('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.ProductImage.Remove(imageToBeDeleted);
            _unitOfWork.Save();

            TempData["success"] = "Товар оновленно!";
            return RedirectToAction(nameof(Upsert), new { id = productId });

        }



        #region APICALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> productList = _unitOfWork.Product.GetAll().ToList();
            return Json(new { data = productList });
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var productToBeDeleted = await _unitOfWork.Product.GetAsync(u => u.Id == id);

            string imagePath = _webHostEnvironment.WebRootPath + @"\images\products\product-" + productToBeDeleted.Id;

            if (productToBeDeleted != null)
            {
                if (Directory.Exists(imagePath))
                {
                    string[] paths = Directory.GetFiles(imagePath);
                    foreach (string path in paths)
                    {
                        System.IO.File.Delete(path);
                    }
                    Directory.Delete(imagePath);
                }

                _unitOfWork.Product.Remove(productToBeDeleted); 
                _unitOfWork.Save();
            }



            return Json(new { success = true, message = "Товар видаленно!" });
        }

        #endregion



    }
}
