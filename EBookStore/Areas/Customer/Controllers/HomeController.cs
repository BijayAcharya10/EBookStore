using EBookStore.DataAccess.Data;
using EBookStore.DataAccess.Repository.IRepository;
using EBookStore.Models;
using EBookStore.Models.ViewModels;
using EBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EBookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public IActionResult Index(string searchString, int page = 1)
        {
            var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            var weeklydate = DateTime.Now.Date.AddDays(-7);


            //For Dashboard
            ViewBag.products = _db.Products.Distinct().Count();
            ViewBag.registeredUsers = _db.Users.Distinct().Count();
            ViewBag.todayOrders = _db.OrderHeaders.Where(x => x.OrderDate >= todaysDate).Count();
            ViewBag.notVerifiedUsers = _db.Users.Where(x => x.EmailConfirmed == true).Distinct().Count();

            //For weekly data
            ViewBag.weeklyOrder = _db.OrderHeaders.Where(x => x.OrderDate >= weeklydate).Count();
            ViewBag.cancelOrderWeekly = _db.OrderHeaders.Where(x => x.OrderStatus == "Cancelled").Count();
            ViewBag.orderProcessing = _db.OrderHeaders.Where(x => x.OrderStatus == "Processing").Count();
            ViewBag.todayShipment = _db.OrderHeaders.Where(x => x.OrderDate >= todaysDate || x.OrderStatus == "Shipped").Count();

            //ViewBag.discountedPrice =_db.Products.Select(x => x.Price / x.Discount).ToList();

            IEnumerable<Product> model = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
            if (!String.IsNullOrEmpty(searchString))
            {
                //model = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").
                //    Where(s => s.Title.Contains(searchString));
                model = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType").
                       Where(x => x.Title.ToLower().Contains(searchString.ToLower()) ||
                       x.Author.ToLower().Contains(searchString.ToLower()));
            }
            var productList = ReflectionIT.Mvc.Paging.PagingList.Create(model, 12, page);
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == claim.Value)
                    .ToList().Count();

                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

            }
            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.
                GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if (ModelState.IsValid)
            {
                //add to cart
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObject.ApplicationUserId && u.ProductId == cartObject.ProductId
                    , includeProperties: "Product"
                    );
                if (cartFromDb == null)
                {
                    //no records exists in database for that product for that user
                    _unitOfWork.ShoppingCart.Add(cartObject);

                }
                else
                {
                    cartFromDb.Count += cartObject.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart
                    .GetAll(c => c.ApplicationUserId == cartObject.ApplicationUserId)
                    .ToList().Count();
                //HttpContext.Session.SetObject(SD.ssShoppingCart, count);
                HttpContext.Session.SetInt32(SD.ssShoppingCart, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productFromDb = _unitOfWork.Product.
                        GetFirstOrDefault(u => u.Id == cartObject.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var weeklydate = DateTime.Now.Date.AddDays(-7);
            //var todaysDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

            //weekly shipment
            var weeklyShipment = _db.OrderHeaders.Where(x => x.OrderDate >= weeklydate && x.OrderStatus == "Shipped").Count();

            //weekly order
            var weeklyOrder = _db.OrderHeaders.Where(x => x.OrderDate >= weeklydate).Count();

            //weekly cancellation
            var weeklyCancellation = _db.OrderHeaders.Where(x => x.OrderDate >= weeklydate && x.OrderStatus == "Cancelled").Count();

            
            return Json(new {weeklyShipment, weeklyOrder, weeklyCancellation });
        }


        public IActionResult GetAllMonthly()
        {
            var monthlydate = DateTime.Now.Date.AddDays(-30);

            //monthly shipment
            var monthlyShipment = _db.OrderHeaders.Where(x => x.OrderDate >= monthlydate && x.OrderStatus == "Shipped").Count();

            //monthly order
            var monthlyOrder = _db.OrderHeaders.Where(x => x.OrderDate >= monthlydate).Count();

            //monthly cancellation
            var monthlyCancellation = _db.OrderHeaders.Where(x => x.OrderDate >= monthlydate && x.OrderStatus == "Cancelled").Count();


            return Json(new { monthlyShipment, monthlyOrder, monthlyCancellation });
        }

        #endregion
    }
}
