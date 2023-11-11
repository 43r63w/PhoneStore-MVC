using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.ProjectModel.ProjectLockFile;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using PhoneStore.Entities.ViewModels;
using PhoneStore.Models;
using PhoneStore.Services;
using SQLitePCL;
using System.Diagnostics;
using System.Security.Claims;

namespace PhoneStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;



        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Search(string search)
        {
            List<Product> products = _unitOfWork.Product.Search(search);
            return View(products);         
        }

        public IActionResult Index()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                HttpContext.Session.SetInt32(SD.ShoppingCartSessionId, _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==userId.Value).Count());
                HttpContext.Session.SetInt32(SD.WishlistSessionId, _unitOfWork.Wishlist.GetAll(u => u.ApplicationUserId == userId.Value).Count());
            }

            IEnumerable<Product> productsList = _unitOfWork.Product.GetAll();

            IEnumerable<ProductImage> products = _unitOfWork.ProductImage.GetAll();

            return View(productsList);
        }

        public async Task<IActionResult> Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = await _unitOfWork.Product.GetAsync(u => u.Id == productId, IncludeProperties: "Category,ProductImages"),
                ProductId = productId,
                Count = 1,
            };
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.ApplicationUserId == userId);


            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }
            _unitOfWork.Save();

            TempData["success"] = "Товар доданно до кошика";

            return RedirectToAction(nameof(Index));

        }
        [Authorize]
        public async Task<IActionResult> AddToWish(int productId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var wishListFromDb = await _unitOfWork.Wishlist.GetAsync(u => u.ApplicationUserId == userId && u.ProductId == productId);

            Wishlist wishList = new()
            {
                Product = await _unitOfWork.Product.GetAsync(u => u.Id == productId, IncludeProperties: "ProductImages,Category"),
                ProductId = productId,
                ApplicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId),
                ApplicationUserId = userId
            };


            if (wishListFromDb != null)
            {
                TempData["warning"] = "У вас вже є такий товар в cписку";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _unitOfWork.Wishlist.Add(wishList);
                _unitOfWork.Save();
                TempData["success"] = "Товар додано до списку бажанго";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }

}
