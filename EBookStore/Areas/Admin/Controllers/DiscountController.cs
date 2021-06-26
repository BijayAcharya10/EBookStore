using EBookStore.DataAccess.Data;
using EBookStore.DataAccess.Repository.IRepository;
using EBookStore.Models;
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
    public class DiscountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        public DiscountController(IUnitOfWork unitOfWork,ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Product product = new Product();
            if(id == null)
            {
                //this is for create
                return View(product);
            }
            //this is for update
            product = _unitOfWork.Product.Get(id.GetValueOrDefault());
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Product product)
        {
            //if (ModelState.IsValid)
            //{
                if(product.Id == 0)
                {
                    // _unitOfWork.Product.Add(product);
                    List<Product> discountObj = _db.Products.Where(x => x.OfferName==null).ToList();

                        discountObj.ForEach(a =>
                        {
                        a.OfferName = product.OfferName;
                        a.Discount = product.Discount;
                        });
                    

                    _db.SaveChanges();
                    
                }
                else
                {
                    _unitOfWork.Product.Update(product);
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));

            //}
            return View(product);
        }

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _unitOfWork.Product.GetAll().Select(c => new { c.Discount, c.OfferName }).Distinct();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            //var objFromDb = _unitOfWork.CoverType.Get(id);
            var objFromDb = _db.Products.Where(x => x.OfferName != null).ToList();
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            objFromDb.ForEach(a =>
            {
                a.OfferName = null;
                a.Discount = 0;
            });
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successfully" });

        }

        #endregion
    }
}
