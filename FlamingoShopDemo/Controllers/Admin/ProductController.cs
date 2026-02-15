using Azure.Core;
using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop.Implementation;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FlamingoShopDemo.Controllers.Admin
{
    public class ProductController : BaseController
    {
        private readonly ApplicationDBContext _db;
        private readonly IWebHostEnvironment _env;
        public ProductController(ApplicationDBContext db, IWebHostEnvironment env) : base(db)
        {
            _db = db;
            _env = env;
        }
        public IActionResult Index()
        {
            ViewBag.role = "admin";

            var GroupDetail = _db.CategoryGroup.ToList();
            var ProductDetail = _db.CategoryDetail.ToList();
            var ProductImage = _db.Image.ToList();

            var TemplateMaster = _db.TemplateFlower.ToList();
            var TemplateDetail = _db.DetailTemplateFlower
                                .Join(
                                    _db.CategoryDetail,
                                    d => d.CategoryDetailId,
                                    cd => cd.Id,
                                    (d, cd) => new { d, cd }
                                )
                                .Join(
                                    _db.CategoryGroup,
                                    temp => temp.cd.CategoryGroupId,
                                    cg => cg.Id,
                                    (temp, cg) => new TemeplateCategoryDto
                                    {
                                        Id = temp.d.Id,
                                        TemplateFlowerId = temp.d.TemplateFlowerId,
                                        CategoryDetailId = temp.d.CategoryDetailId,
                                        StatusUse = temp.d.StatusUse,
                                        Qty = temp.d.Qty,
                                        NameCategory = temp.cd.NameCategory,
                                        CategoryGroupId = cg.Id,
                                        CategoryGroupName = cg.NameCategory
                                    }
                                )
                                .ToList();

            var model = new ProductViewDto
            {
                Groups = GroupDetail,
                Products = ProductDetail,
                Images = ProductImage,
                Template = TemplateMaster,
                TempDetail = TemplateDetail
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult AddGroup(string groupName)
        {
            try
            {
                var exists = _db.CategoryGroup
                        .Any(x => x.NameCategory == groupName.Trim());

                if (!exists)
                {
                    var addGroup = new CategoryGroupModel
                    {
                        NameCategory = groupName.Trim(),
                        StatusUse = 1,
                        Cdt = DateTime.Now,
                        Udt = DateTime.Now
                    };

                    _db.CategoryGroup.Add(addGroup);
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
        public IActionResult AddProduct([FromForm] AddProductDto request)
        {
            try
            {

                var exists = _db.CategoryDetail
                    .Any(x => x.NameCategory == request.NameCategory.Trim());

                if (!exists)
                {
                    var addGroup = new CategoryDetailModel
                    {
                        CategoryGroupId = request.CategoryGroupId,
                        NameCategory = request.NameCategory.Trim(),
                        Qty = request.Qty,
                        StatusUse = 1,
                        Cdt = DateTime.Now,
                        Udt = DateTime.Now
                    };

                    _db.CategoryDetail.Add(addGroup);
                    _db.SaveChanges();

                    var addLogPrice = new LogAddCategoryDetailModel
                    {
                        CategoryDetailId = addGroup.Id,
                        Qty = request.Qty,
                        Price = request.Price,
                        Cdt = DateTime.Now,
                        Udt = DateTime.Now
                    };

                    _db.LogAddCategoryDetail.Add(addLogPrice);
                    _db.SaveChanges();


                    if (request.Images != null && addGroup.Id != 0)
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
                                RelationId = addGroup.Id,
                                Path = dbPath,          
                                TypeImage = 1,          
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
        public IActionResult UpdateGroupStatus([FromBody] UpdateStatusDto model)
        {
            try
            {
                var exists = _db.CategoryGroup
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

        [HttpPost]
        public IActionResult DeleteProduct( int Id)
        {
            try
            {
                var exists = _db.CategoryDetail
                        .Where(x => x.Id == Id).FirstOrDefault();

                if (exists != null)
                {
                    _db.CategoryDetail.Remove(exists);
                    _db.SaveChanges();
                }

                var images = _db.Image.Where(x => x.RelationId == Id && x.TypeImage == 1).ToList();

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

       


        [HttpPost]
        public IActionResult DeleteTemplate(int Id)
        {
            try
            {
                var exists = _db.TemplateFlower
                        .Where(x => x.Id == Id).FirstOrDefault();

                if (exists != null)
                {
                    _db.TemplateFlower.Remove(exists);
                    _db.SaveChanges();
                }

                var existsDetail = _db.DetailTemplateFlower
                        .Where(x => x.TemplateFlowerId == Id).ToList();

                if (existsDetail != null)
                {
                    _db.DetailTemplateFlower.RemoveRange(existsDetail);
                    _db.SaveChanges();
                }


                var images = _db.Image.Where(x => x.RelationId == Id && x.TypeImage == 2).ToList();

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


        [HttpPost]
        public IActionResult UpdateProduct([FromForm] AddProductDto request)
        {
            try
            {

                var exists = _db.CategoryDetail
                    .Where(x => x.Id == request.Id).FirstOrDefault();

                if (exists != null)
                {

                    exists.NameCategory = request.NameCategory.Trim();
                    //exists.Qty = request.Qty;
                    exists.Udt = DateTime.Now;

                    _db.CategoryDetail.Update(exists);
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
                                TypeImage = 1,
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
        public IActionResult UpdateProductStatus([FromBody] UpdateStatusDto model)
        {
            try
            {
                var exists = _db.CategoryDetail
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

        [HttpPost]
        public IActionResult AdjustProductStatus([FromForm] UpdateStatusDto model)
        {
            try
            {
                var exists = _db.CategoryDetail
                        .Where(x => x.Id == model.Id).FirstOrDefault();

                if (exists != null)
                {
                    exists.Qty = exists.Qty + model.Qty;
                    exists.Udt = DateTime.Now;

                    _db.SaveChanges();
                }

                if (exists?.StatusUse == 1 && exists?.Qty == 0)
                {
                    exists.StatusUse = 0;

                    _db.SaveChanges();

                }
                else if (exists?.StatusUse == 0 && exists?.Qty >= 1) { 
                    exists.StatusUse = 1;

                    _db.SaveChanges();
                }


                var addLogPrice = new LogAddCategoryDetailModel
                {
                    CategoryDetailId = model.Id,
                    Qty = model.Qty,
                    Price = model.Price,
                    Cdt = DateTime.Now,
                    Udt = DateTime.Now
                };

                _db.LogAddCategoryDetail.Add(addLogPrice);
                _db.SaveChanges();


                return Json(new { result = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { result = "false" + ex.ToString() });
            }

        }

        [HttpPost]
        public IActionResult AddTemplate([FromForm] AddTemplateDto model)
        {
            try
            {
                var details = JsonSerializer.Deserialize<List<DetailTemplateFlowerModel>>(model.details);

                var exists = _db.TemplateFlower
                  .Any(x => x.Name == model.Name.Trim());

                if (!exists)
                {
                    var addTemplate = new TemplateFlowerModel
                    {
                        Name = model.Name.Trim(),
                        Price = model.Price,
                        StatusUse = 1,
                        Cdt = DateTime.Now,
                        Udt = DateTime.Now
                    };

                    _db.TemplateFlower.Add(addTemplate);
                    _db.SaveChanges();

                    if (details != null && details.Any())
                    {
                        var groupedDetails = details
                            .GroupBy(x => x.CategoryDetailId)
                            .Select(g => new
                            {
                                CategoryDetailId = g.Key,
                                Qty = g.Sum(x => x.Qty)
                            })
                            .ToList();

                        foreach (var item in groupedDetails)
                        {
                            var addDetails = new DetailTemplateFlowerModel
                            {
                                TemplateFlowerId = addTemplate.Id,
                                CategoryDetailId = item.CategoryDetailId,
                                Qty = item.Qty,           
                                StatusUse = 1,
                                Cdt = DateTime.Now,
                                Udt = DateTime.Now
                            };

                            _db.DetailTemplateFlower.Add(addDetails);
                        }

                        _db.SaveChanges();
                    }


                    if (model.Images != null && addTemplate.Id != 0)
                    {
                        var uploadPath = Path.Combine(_env.WebRootPath, "uploads");


                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);

                        foreach (var file in model.Images)
                        {
                            var ext = Path.GetExtension(file.FileName);
                            var newFileName = $"Template_{Guid.NewGuid()}{ext}";

                            var filePath = Path.Combine(uploadPath, newFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            var dbPath = "/uploads/" + newFileName;

                            var addImages = new ImageModel
                            {
                                RelationId = addTemplate.Id,
                                Path = dbPath,
                                TypeImage = 2,
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
                return Json(new { result = "false" + ex.ToString() });
            }

        }

        [HttpPost]
        public IActionResult UpdateTempStatus([FromBody] UpdateStatusDto model)
        {
            try
            {
                var exists = _db.TemplateFlower
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

        [HttpPost]
        public IActionResult UpdateTemplate([FromForm] AddTemplateDto request)
        {
            try
            {
                var details = JsonSerializer.Deserialize<List<DetailTemplateFlowerModel>>(request.details);

                var exists = _db.TemplateFlower
                    .Where(x => x.Id == request.Id).FirstOrDefault();

                if (exists != null)
                {

                    exists.Name = request.Name.Trim();
                    exists.Price = request.Price;
                    exists.Udt = DateTime.Now;

                    _db.TemplateFlower.Update(exists);
                    _db.SaveChanges();


                    if (details != null && details.Any())
                    {
                        var removeOld = _db.DetailTemplateFlower.Where(x => x.TemplateFlowerId == request.Id).ToList();
                        _db.RemoveRange(removeOld);
                        _db.SaveChanges();

                        var groupedDetails = details
                            .GroupBy(x => x.CategoryDetailId)
                            .Select(g => new
                            {
                                CategoryDetailId = g.Key,
                                Qty = g.Sum(x => x.Qty)
                            })
                            .ToList();

                        foreach (var item in groupedDetails)
                        {
                            var addDetails = new DetailTemplateFlowerModel
                            {
                                TemplateFlowerId = exists.Id,
                                CategoryDetailId = item.CategoryDetailId,
                                Qty = item.Qty,
                                StatusUse = 1,
                                Cdt = DateTime.Now,
                                Udt = DateTime.Now
                            };

                            _db.DetailTemplateFlower.Add(addDetails);
                        }

                        _db.SaveChanges();
                    }


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
                                TypeImage = 2,
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

    }
}
