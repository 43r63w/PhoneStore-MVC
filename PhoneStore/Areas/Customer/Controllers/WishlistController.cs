using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using PhoneStore.Services;
using System.Security.Claims;

namespace PhoneStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<Wishlist> wishlists = await _unitOfWork.Wishlist.GetAllAsync(u => u.ApplicationUserId == userId, IncludedProperties: "Product");
            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();
            return View(wishlists);
        }

        #region ManagmentWishList
        public async Task<IActionResult> AddToCart(int productId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCart cart = new()
            {
                Product = await _unitOfWork.Product.GetAsync(u => u.Id == productId),
                ProductId = productId,
                ApplicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId),
                ApplicationUserId = userId,
                Count = 1
            };

            _unitOfWork.ShoppingCart.Add(cart);
            _unitOfWork.Save();
            HttpContext.Session.SetInt32(SD.ShoppingCartSessionId, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            TempData["success"] = "Товар доданно до кошика";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int wishId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var wishFromDb = await _unitOfWork.Wishlist.GetAsync(u=>u.Id == wishId);

            _unitOfWork.Wishlist.Remove(wishFromDb);
            _unitOfWork.Save();

            TempData["success"] = "Товар видаленно зі списку бажаного";
            HttpContext.Session.SetInt32(SD.WishlistSessionId, _unitOfWork.Wishlist.GetAll(u => u.ApplicationUserId == userId).Count());
            return RedirectToAction(nameof(Index));
        }

        #endregion


    }
}
