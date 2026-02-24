using Azure.Core;
using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using FlamingoShopDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace FlamingoShopDemo.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDBContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        public HomeController(
             ILogger<HomeController> logger,
             ApplicationDBContext db,
             IWebHostEnvironment env,
             IConfiguration config
         ) : base(db, env)
        {
                _db = db;
                _logger = logger;
                _config = config;
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
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserAddress", user.Address);
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

        public async Task<IActionResult> LineCallback(string code)
        {
            if (string.IsNullOrEmpty(code))
                return RedirectToAction("Login");

            var client = new HttpClient();

            var values = new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", "https://157.85.97.187/Home/LineCallback" },
            { "client_id", _config["Line:ChannelId"] },
            { "client_secret", _config["Line:ChannelSecret"] }
        };

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://api.line.me/oauth2/v2.1/token", content);
            var result = await response.Content.ReadAsStringAsync();

            var tokenObj = JObject.Parse(result);

            var accessToken = tokenObj["access_token"].ToString();
            var idToken = tokenObj["id_token"]?.ToString();

            // ====== GET PROFILE ======
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var profileResponse = await client.GetAsync("https://api.line.me/v2/profile");
            var profileResult = await profileResponse.Content.ReadAsStringAsync();

            var profile = JObject.Parse(profileResult);

            string userId = profile["userId"]?.ToString();
            string displayName = profile["displayName"]?.ToString();
            string ProfileImg = profile["pictureUrl"]?.ToString();


            HttpContext.Session.SetString("UserName", displayName);
            HttpContext.Session.SetInt32("UserId", 0);
            HttpContext.Session.SetString("UserEmail", "");
            HttpContext.Session.SetString("UserAddress", "");
            HttpContext.Session.SetString("userIdLine", userId);
            HttpContext.Session.SetString("UserImg", ProfileImg);

            return RedirectToAction("Index", "Customer");

             
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
