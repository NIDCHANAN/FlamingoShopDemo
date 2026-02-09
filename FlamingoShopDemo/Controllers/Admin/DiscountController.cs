using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Mvc;

namespace FlamingoShopDemo.Controllers.Admin
{
    public class DiscountController : BaseController
    {
        private readonly ApplicationDBContext _db;

        public DiscountController(ApplicationDBContext db) : base(db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            ViewBag.role = "admin";
            var DiscountList = _db.Promotion.ToList();

            return View(DiscountList);
        }

        [HttpPost]
        public IActionResult AddDiscount([FromForm] PromotionModel request)
        {
            try
            {
                var exists = _db.Promotion
                    .Any(x => x.Name == request.Name.Trim());

                if (!exists)
                {
                    var addDisCount = new PromotionModel
                    {
                        Name = request.Name.Trim(),
                        PercentCal = request.PercentCal,
                        StartDate = request.StartDate,
                        EndDate = request.EndDate,
                        StatusUse = 1,
                        Cdt = DateTime.Now,
                        Udt = DateTime.Now
                    };

                    _db.Promotion.Add(addDisCount);
                    _db.SaveChanges();

                }

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public IActionResult DeleteProduct(int Id)
        {
            try
            {
                var exists = _db.Promotion
                        .Where(x => x.Id == Id).FirstOrDefault();

                if (exists != null)
                {
                    _db.Promotion.Remove(exists);
                    _db.SaveChanges();
                }

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false" + ex.ToString() });
            }

        }

        [HttpPost]
        public IActionResult UpdateDiscount([FromForm] PromotionModel request)
        {
            try
            {
                var exists = _db.Promotion
                    .Where(x => x.Id == request.Id).FirstOrDefault();

                if (exists != null)
                {

                    exists.Name = request.Name.Trim();
                    exists.PercentCal = request.PercentCal;
                    exists.StartDate = request.StartDate;
                    exists.EndDate = request.EndDate;
                    //exists.Qty = request.Qty;
                    exists.Udt = DateTime.Now;

                    _db.Promotion.Update(exists);
                    _db.SaveChanges();
                }

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UpdateStatus([FromBody] UpdateStatusDto model)
        {
            try
            {
                var exists = _db.Promotion
                        .Where(x => x.Id == model.Id).FirstOrDefault();

                if (exists != null)
                {
                    exists.StatusUse = model.StatusUse;
                    exists.Udt = DateTime.Now;

                    _db.SaveChanges();
                }

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false" + ex.ToString() });
            }

        }
    }
}
