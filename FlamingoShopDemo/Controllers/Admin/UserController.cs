using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FlamingoShopDemo.Controllers.Admin
{
    public class UserController : BaseController
    {
        private readonly ApplicationDBContext _db;
        public UserController(
              ApplicationDBContext db,
              IWebHostEnvironment env
          ) : base(db, env)
                {
                    _db = db;
                }
      

        public IActionResult Index()
        {
            ViewBag.role = "admin";

            var User = _db.User.ToList();
            var Images = _db.Image.Where(x => x.TypeImage == 3).ToList();



            var model = new UserViewDto
            {
                User = User,
                Images = Images
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddUser([FromForm] UserDto request)
        {
            try
            {
                var chkTel = _db.User.Where(x => x.Telephone == request.Telephone).FirstOrDefault();
                if (chkTel == null)
                {
                    var hasher = new PasswordHasher<object>();

                    var addUser = new UserModel
                    {
                        Name = request.Name,
                        Telephone = request.Telephone,
                        Email = request.Email,
                        Address = request.Address,
                        Password = hasher.HashPassword(null, request.Password),
                        Status = request.Status,
                        Role = request.Role,
                        Blacklist = 0,
                        ConfirmTel = 1
                    };

                    _db.User.Add(addUser);
                    _db.SaveChanges();

                    if (request.Images != null && addUser.Id != 0)
                    {
                        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");


                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        foreach (var file in request.Images)
                        {
                            var ext = Path.GetExtension(file.FileName);
                            var newFileName = $"Product_{Guid.NewGuid()}{ext}";

                            var filePath = Path.Combine(uploadPath, newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            var dbPath = "/uploads/" + newFileName;

                            var addImages = new ImageModel
                            {
                                RelationId = addUser.Id,
                                Path = dbPath,
                                TypeImage = 3,
                                StatusUse = 1,
                                Cdt = DateTime.Now,
                                Udt = DateTime.Now
                            };

                            _db.Image.Add(addImages);
                        }

                        _db.SaveChanges();
                    }

                    return Json(new { result = "success" });
                }
                else { 
                    return Json(new { result = "duplicate" });

                }

            }
            catch (Exception ex)
            {
                return Json(new { result = "false" + ex.ToString() });
            }

        }

        [HttpPost]
        public IActionResult DeleteUser(int Id)
        {
            try
            {
                var exists = _db.User
                        .Where(x => x.Id == Id).FirstOrDefault();

                if (exists != null)
                {
                    _db.User.Remove(exists);
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
        public IActionResult UpdateUser([FromForm] UserDto request)
        {
            try
            {
                var hasher = new PasswordHasher<object>();
                var exists = _db.User
                    .Where(x => x.Id == request.Id).FirstOrDefault();

                if (exists != null)
                {

                    exists.Name = request.Name;
                    exists.Telephone = request.Telephone;
                    exists.Email = request.Email;
                    exists.Address = request.Address;
                    exists.Password = string.IsNullOrWhiteSpace(request.Password)
                                    ? exists.Password
                                    : hasher.HashPassword(null, request.Password);
                    exists.Status = request.Status;
                    exists.Role = request.Role;
                    exists.Blacklist = 0;
                    exists.ConfirmTel = 1;

                    _db.User.Update(exists);
                    _db.SaveChanges();

                    if (request.Images != null && exists.Id != 0)
                    {
                        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");


                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        foreach (var file in request.Images)
                        {
                            var ext = Path.GetExtension(file.FileName);
                            var newFileName = $"{Guid.NewGuid()}{ext}";

                            var filePath = Path.Combine(uploadPath, newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            var dbPath = "/uploads/" + newFileName;

                            var addImages = new ImageModel
                            {
                                RelationId = exists.Id,
                                Path = dbPath,
                                TypeImage = 3,
                                StatusUse = 1,
                                Cdt = DateTime.Now,
                                Udt = DateTime.Now
                            };

                            _db.Image.Add(addImages);
                        }

                        _db.SaveChanges();

                    }

                }

                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult LockUser(int Id, int type)
        {
            try
            {
                var exists = _db.User
                        .Where(x => x.Id == Id).FirstOrDefault();

                if (exists != null)
                {
                    exists.Blacklist = type;
                    
                    _db.User.Update(exists);
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
