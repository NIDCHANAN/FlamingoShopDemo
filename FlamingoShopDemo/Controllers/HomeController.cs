using Azure.Core;
using FlamingoShopDemo.Data;
using FlamingoShopDemo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;

namespace FlamingoShopDemo.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDBContext _db;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext db) : base(db)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var hasher = new PasswordHasher<object>();
            var user = _db.User
                .FirstOrDefault(x =>
                    (x.Email == email  || x.Telephone == email) &&
                    x.Blacklist != 1 &&
                    x.Status == 1);

            if (user == null)
            {
                TempData["Error"] = "Email หรือ Password ไม่ถูกต้อง";
                return RedirectToAction("Index", "Home");

            }

            var result = hasher.VerifyHashedPassword(
                            null,
                            user.Password,   
                            password         
                        );

            if (result != PasswordVerificationResult.Success)
            {
                TempData["Error"] = "Email หรือ Password ไม่ถูกต้อง";
                return RedirectToAction("Index", "Home");

            }


            var Img = _db.Image.Where(x => x.TypeImage == 3 && x.RelationId == user.Id && x.StatusUse == 1).FirstOrDefault();

            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetInt32("UserId", user.Id);
            if (Img != null)
            { 
                 HttpContext.Session.SetString("UserImg", Img?.Path);
            }

            if (user.Role == 0)
            {
                return RedirectToAction("Index", "Monitor");
            }
            else 
            { 
                return RedirectToAction("Index", "Customer");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
