using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Mvc;

namespace FlamingoShopDemo.Controllers.Admin
{
    public class MonitorController : BaseController
    {
        private readonly ApplicationDBContext _db;
        public MonitorController(
               ApplicationDBContext db,
               IWebHostEnvironment env
        ) : base(db, env)
        {
            _db = db;
        }
        public IActionResult Index(DateTime? dateSort)
        {
            ViewBag.role = "admin";

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var masterQuery = _db.MasterOrder.AsQueryable();

            if (!dateSort.HasValue)
            {
                masterQuery = masterQuery
                    .Where(x => x.OrderDate >= today && x.OrderDate < tomorrow);
            }
            else
            {
                var startDate = dateSort.Value.Date;
                var endDate = startDate.AddDays(1);

                masterQuery = masterQuery
                    .Where(x => x.OrderDate >= startDate && x.OrderDate < endDate);
            }

            var masterOrder = masterQuery.ToList();

            var masterIds = masterOrder.Select(x => x.Id).ToList();
            var subOrder = new List<SubMasterOrderModel>();
            if (masterIds.Any())
            {
                subOrder = _db.SubMasterOrder
                    .Where(x => masterIds.Contains(x.MasterOrderId))
                    .ToList();
            }


            var submasterIds = subOrder.Select(x => x.TemplateId).ToList();
            var TempFlower = new List<TemplateFlowerModel>();
            if (submasterIds.Any())
            {
                TempFlower = _db.TemplateFlower
                    .Where(x => submasterIds.Contains(x.Id))
                    .ToList();
            }

            var user = _db.User.ToList();

            var model = new MonitorViewDto
            {
                masterOrder = masterOrder,
                SubMasterOrder = subOrder,
                TemplateFlower = TempFlower,
                user = user
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int OrderId, int Status)
        {
           
            var order = _db.MasterOrder.FirstOrDefault(x => x.Id == OrderId);
            if (order != null)
            {
                order.Status = Status;
                _db.SaveChanges();
            }

            return Json(new { success = true });
        }

    }
}
