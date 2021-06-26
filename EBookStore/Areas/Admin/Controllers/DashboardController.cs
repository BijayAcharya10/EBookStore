using EBookStore.DataAccess.Data;
using EBookStore.DataAccess.Repository.IRepository;
using EBookStore.Models;
using EBookStore.Models.ViewModels;
using EBookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EBookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public DashboardController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.products = _db.Products.Distinct().Count();
            ViewBag.registeredUsers = _db.Users.Distinct().Count();
            ViewBag.todayOrders = _db.OrderHeaders.Where(x => x.OrderDate == DateTime.Today).Count();
            ViewBag.todayCash = _db.OrderHeaders.Where(x => x.OrderDate == DateTime.Today).Sum(x => x.OrderTotal);

            return View();
        }

    }
}
