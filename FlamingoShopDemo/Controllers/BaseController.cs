using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace FlamingoShopDemo.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDBContext _db;

        protected readonly IWebHostEnvironment _env;

        public BaseController(ApplicationDBContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [HttpPost]
        public IActionResult DeleteImages(int Id)
        {
            try
            {
                var images = _db.Image.Where(x => x.Id == Id).ToList();

                foreach (var img in images)
                {
                    var fullPath = Path.Combine(_env.WebRootPath, "uploads");

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


        [HttpPost]
        public  IActionResult SaveImages(IFormFile Image, int type, long id)
        {
            try
            {

                var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

              
                    var ext = Path.GetExtension(Image.FileName);
                    var newFileName = $"Template_{Guid.NewGuid()}{ext}";

                    var filePath = Path.Combine(uploadPath, newFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        Image.CopyTo(stream);
                    }

                    var dbPath = "/uploads/" + newFileName;

                    var addImages = new ImageModel
                    {
                        RelationId = id,
                        Path = dbPath,
                        TypeImage = type,
                        StatusUse = 1,
                        Cdt = DateTime.Now,
                        Udt = DateTime.Now
                    };

                    _db.Image.Add(addImages);

                _db.SaveChanges();

                return Ok(new { success = true, imageUrl = dbPath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
