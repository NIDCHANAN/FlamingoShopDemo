using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace FlamingoShopDemo.Controllers.Customer
{
    public class CustomerController : BaseController
    {
        private readonly ApplicationDBContext _db;

        public CustomerController(
            ApplicationDBContext db,
            IWebHostEnvironment env
        ) : base(db, env)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            ViewBag.role = "customer";

            var ProductImage = _db.Image.Where(x => (x.TypeImage == 1 || x.TypeImage == 2) && x.StatusUse == 1).ToList();
            var TemplateMaster = _db.TemplateFlower.ToList();

            var model = new ProductViewDto
            {
                Images = ProductImage,
                Template = TemplateMaster
            };

            return View(model);
        }

        public IActionResult CustomFlower()
        {
            ViewBag.role = "customer";

            var GroupDetail = _db.CategoryGroup.Where(x => x.StatusUse == 1).ToList();
            var ProductDetail = _db.CategoryDetail.Where(x => x.StatusUse == 1).ToList();
            var ProductPrice = _db.LogAddCategoryDetail.ToList();

            var ProductImage = _db.Image.Where(x => (x.TypeImage == 1 || x.TypeImage == 2) && x.StatusUse == 1).ToList();
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
                TempDetail = TemplateDetail,
                ProductPrice = ProductPrice
            };

            return View(model);
        }

        public IActionResult checkOut()
        {
            ViewBag.role = "customer";

            return View();
        }

        [HttpPost]
        public IActionResult checkTemplate(CustomBouquetDto request)
        {
            var templateIds = _db.DetailTemplateFlower
                              .Where(x =>
                                  (x.CategoryDetailId == request.flower && x.Qty == request.qty)
                                  || x.CategoryDetailId == request.paper)
                              .GroupBy(x => x.TemplateFlowerId)
                              .Where(g => g.Count() >= 2)
                              .Select(g => g.Key)
                              .ToList();

            if (!templateIds.Any())
                return Ok(new { found = false });

            var templates = (
                             from temp in _db.TemplateFlower
                             where templateIds.Contains(temp.Id)
                             select new
                             {
                                 tempFlowerId = temp.Id,
                                 name = temp.Name,
                                 price = temp.Price,
                                 imagePath = _db.Image
                                     .Where(img => img.RelationId == temp.Id && img.TypeImage == 2)
                                     .Select(img => img.Path)
                                     .FirstOrDefault()
                             }
                         ).ToList();



            return Ok(new
            {
                found = true,
                data = templates
            });

        }

        [HttpPost]
        public IActionResult orderMaster(MasterOrderModel request)
        {
            var orderNo = DateTime.Now.ToString("yyMMddHHmmssfff");

            if (orderNo != null)
            {
                var addOrder = new MasterOrderModel
                {
            
                    UserId  = HttpContext.Session.GetInt32("UserId").Value,
                    UserIdLine = HttpContext.Session.GetString("UserName"),
                    OrderDate = request.OrderDate,
                    AddressDelivary = request.AddressDelivary,
                    TotalPrice = request.TotalPrice,
                    Discount = 0,
                    PaymentType = 0,
                    MasterDelivaryId = 1,
                    DelivaryNo = "",
                    Status = 0,
                    Cdt = DateTime.Now,
                    Udt = DateTime.Now,
                    TelephoneOrder = request.TelephoneOrder,
                    fullNameRecrive = request.fullNameRecrive,
                    OrderNo = orderNo
                };

                _db.MasterOrder.Add(addOrder);
                _db.SaveChanges();

                return Ok(new
                {
                    success = true,
                    orderId = addOrder.Id
                });

            }

            return Ok(new
            {
                success = false,
                orderId = 0
            });

        }


        public IActionResult History()
        {
            ViewBag.role = "customer";

           

            return View();
        }



    }
}
