using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
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


        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region APICALLS
        public IActionResult GetAll()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            List<OrderHeader> orderHeadersLists;


            _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            if (User.IsInRole(SD.Role_Admin))
            {
                orderHeadersLists = _unitOfWork.OrderHeader.GetAll().ToList();
            }
            else
            {
                orderHeadersLists = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, IncludedProperties: "Product").ToList();

            }

            return Json(new { data = orderHeadersLists });
        }

        #endregion


    }
}
