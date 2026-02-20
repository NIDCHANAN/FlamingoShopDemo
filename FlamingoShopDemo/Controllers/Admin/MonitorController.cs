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
        public IActionResult Index(DateTime dateSort)
        {
            ViewBag.role = "admin";

            var thaiTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(
                               DateTime.UtcNow,
                               "SE Asia Standard Time"
                           );
            var today = thaiTime.Date;
            var tomorrow = today.AddDays(1);

            if (dateSort ==  DateTime.MinValue )
            {
                var masterOrder = _db.MasterOrder
                                .Where(x => x.OrderDate >= today && x.OrderDate < tomorrow)
                                .ToList();
                var user  = _db.User.ToList();

                var model = new MonitorViewDto
                {
                    masterOrder = masterOrder,
                    user = user
                };

                return View(model);
            }
            else 
            {
                var masterOrder = _db.MasterOrder.Where(x => x.OrderDate.Date == dateSort.Date).ToList();
                var user = _db.User.ToList();


                var model = new MonitorViewDto
                {
                    masterOrder = masterOrder,
                    user = user
                };

                return View(model);

            }
        }
    }
}
