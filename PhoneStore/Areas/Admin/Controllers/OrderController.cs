using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using PhoneStore.Entities.ViewModels;
using PhoneStore.Services;
using SQLitePCL;
using Stripe.Tax;
using System.Security.Claims;

namespace PhoneStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int orderHeaderId)
        {
            OrderVM orderVM = new()
            {
                OrderHeader = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == orderHeaderId, IncludeProperties: "ApplicationUser"),
                OrderDetails = await _unitOfWork.OrderDetail.GetAllAsync(u => u.OrderHeaderId == orderHeaderId, IncludedProperties: "Product"),
            };
            return View(orderVM);
        }





        [HttpPost]
        public async Task<IActionResult> UpdateDetails()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orderFromDb = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == OrderVM.OrderHeader.Id);

            orderFromDb.Name = OrderVM.OrderHeader.Name;
            orderFromDb.StreetAdress = OrderVM.OrderHeader.StreetAdress;
            orderFromDb.City = OrderVM.OrderHeader.City;
            orderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            _unitOfWork.OrderHeader.Update(orderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Інформація про користувача оновленна";

            return RedirectToAction(nameof(Details), new { orderHeaderId = orderFromDb.Id });
        }
































        #region APICALLS
        public async Task<IActionResult> GetAll()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            IEnumerable<OrderHeader> orderHeadersLists;

            if (User.IsInRole(SD.Role_Admin))
            {
                orderHeadersLists = _unitOfWork.OrderHeader.GetAll(IncludedProperties: "ApplicationUser");
            }
            else
            {
                orderHeadersLists = await _unitOfWork.OrderHeader.GetAllAsync(u => u.ApplicationUserId == userId);
            }

            return Json(new { data = orderHeadersLists });
        }

        #endregion


    }
}
