using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlamingoShopDemo.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDBContext _db;

        public BaseController(ApplicationDBContext db)
        {
            _db = db;

        }

        [HttpPost]
        public IActionResult DeleteImages(int Id)
        {
            try
            {
                var images = _db.Image.Where(x => x.Id == Id).ToList();

                foreach (var img in images)
                {
                    var fullPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        img.Path.TrimStart('/')
                    );

                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }

                _db.Image.RemoveRange(images);
                _db.SaveChanges();

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false" + ex.ToString() });
            }
        }
    }
}
