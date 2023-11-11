using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneStore.DAL.Repository.IRepository;
using PhoneStore.Entities;
using PhoneStore.Entities.ViewModels;
using PhoneStore.Services;
using SQLitePCL;
using Stripe.Checkout;
using System.Security.Claims;

namespace PhoneStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            HttpContext.Session.SetInt32(SD.ShoppingCartSessionId, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());

            ShoppingCartVM = new()
            {
                ItemsInCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, IncludedProperties: "Product"),
            };

            foreach (var item in ShoppingCartVM.ItemsInCart)
            {
                item.Price = GetPrice(item);
                ShoppingCartVM.OrderTotal += (item.Price * item.Count);
            }

            IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

            return View(ShoppingCartVM);
        }

        #region ManagmentCart
        public async Task<IActionResult> Plus(int cartId)
        {
            var productFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);

            if (productFromDb != null)
            {
                productFromDb.Count += 1;
                _unitOfWork.ShoppingCart.Update(productFromDb);
            }
            _unitOfWork.Save();

            TempData["success"] = "Кошик оновленно";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;
            var productFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);

            if (productFromDb != null)
            {
                if (productFromDb.Count <= 1)
                {
                    _unitOfWork.ShoppingCart.Remove(productFromDb);
                    _unitOfWork.Save();
                    HttpContext.Session.SetInt32(SD.ShoppingCartSessionId, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
                }
                else
                {
                    productFromDb.Count -= 1;
                    _unitOfWork.ShoppingCart.Update(productFromDb);
                    _unitOfWork.Save();
                }
            }

            TempData["success"] = "Кошик оновленно";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {

            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new()
            {
                ItemsInCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, IncludedProperties: "Product"),
                OrderHeader = new(),
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.StreetAdress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdreess;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in ShoppingCartVM.ItemsInCart)
            {
                item.Price = GetPrice(item);

                ShoppingCartVM.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            return View(ShoppingCartVM);

        }

        [ActionName("Summary")]
        [HttpPost]
        public IActionResult SummaryPOST()
        {

            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM.ItemsInCart = _unitOfWork.ShoppingCart.
                GetAll(u => u.ApplicationUserId == userId, IncludedProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.Carrier = "Нова пошта";
            ShoppingCartVM.OrderHeader.TrackingNumber = Guid.NewGuid().ToString();

            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            foreach (var item in ShoppingCartVM.ItemsInCart)
            {
                item.Price = GetPrice(item);

                ShoppingCartVM.OrderHeader.OrderTotal += (item.Price * item.Count);
            }

            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderPlacedAndPaid;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentWaiting;

            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();



            foreach (var item in ShoppingCartVM.ItemsInCart)
            {

                OrderDetail orderDetail = new()
                {
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    ProductId = item.Product.Id,
                    Count = item.Count,
                    Price = item.Price,

                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }


            var domain = "https://localhost:7169/";

            if (User.IsInRole(SD.Role_Customer))
            {
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConformation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ItemsInCart)
                {
                    var lineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "uah",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Name + " " + item.Product.Model,
                            }
                        },
                        Quantity = item.Count,
                    };

                    options.LineItems.Add(lineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);

                _unitOfWork.OrderHeader.UpdatePaymentStatus(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateOrderStatus(ShoppingCartVM.OrderHeader.Id, SD.OrderWaiting, SD.PaymentWaiting);
                _unitOfWork.Save();
                Response.Headers.Add("location", session.Url);
                return new StatusCodeResult(303);

            }
            return RedirectToAction(nameof(OrderConformation), new { id = ShoppingCartVM.OrderHeader.Id });

        }


        public async Task<IActionResult> OrderConformation(int id)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            var orderHeaderFromDb = await _unitOfWork.OrderHeader.GetAsync(u => u.Id == id);

            var service = new SessionService();
            Session session = service.Get(orderHeaderFromDb.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdatePaymentStatus(id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateOrderStatus(id, SD.OrderPlacedAndPaid, SD.PaymentPaid);
                _unitOfWork.Save();
            }

            IEnumerable<ShoppingCart> shoppingCartsLists = await _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserId == userId);

            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartsLists);
            _unitOfWork.Save();
            return View(id);
        }


        public async Task<IActionResult> Remove(int cartId)
        {
            var claim = (ClaimsIdentity)User.Identity;
            var userId = claim.FindFirst(ClaimTypes.NameIdentifier).Value;

            var productFromDb = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == cartId);

            if (productFromDb != null)
            {
                _unitOfWork.ShoppingCart.Remove(productFromDb);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.ShoppingCartSessionId, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Кошик оновленно";
            return RedirectToAction(nameof(Index));

        }

        #endregion

        public double GetPrice(ShoppingCart cart)
        {
            if (cart.Product.IsSale == true)
            {
                return cart.Product.PriceForSale;
            }
            else
            {
                return cart.Product.Price;
            }
        }

    }
}
