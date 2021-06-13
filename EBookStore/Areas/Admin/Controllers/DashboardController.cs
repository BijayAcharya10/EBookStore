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
        public DashboardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API Calls
        //[HttpGet]
        //public IActionResult GetCount()
        //{
        //    var allObj = _unitOfWork.Category.GetAll();
        //    return Json(new { data = allObj });
        //}

        #endregion
    }
}
