using Azure.Core;
using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

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
        public IActionResult orderMaster(OrdertDetailDto request)
        {
            var orderNo = DateTime.Now.ToString("yyMMddHHmmssfff");

            if (orderNo != null && request.OrderSubDraft.Count > 0)
            {
                var addOrder = new MasterOrderModel
                {

                    UserId = HttpContext.Session.GetInt32("UserId").Value,
                    UserIdLine = HttpContext.Session.GetString("UserName"),
                    OrderDate = request.OrderDate,
                    AddressDelivary = request.AddressDelivary,
                    TotalPrice = request.TotalPrice,
                    Discount = request.Discount,
                    PaymentType = 0,
                    MasterDelivaryId = 1,
                    DelivaryNo = "",
                    Status = 0,
                    Cdt = DateTime.Now,
                    Udt = DateTime.Now,
                    TelephoneOrder = request.TelephoneOrder,
                    fullNameRecrive = request.fullNameRecrive,
                    OrderNo = orderNo,
                    PromotionId = request.PromotionId
                };

                _db.MasterOrder.Add(addOrder);
                _db.SaveChanges();

                if (addOrder.Id != null && request.OrderSubDraft.Count > 0)
                {
                    foreach (var sub in request.OrderSubDraft)
                    {
                        var addSub = new SubMasterOrderModel
                        {
                            MasterOrderId = addOrder.Id ?? 0,
                            TemplateId = sub.TemplateId.ToString().Length == 13 ? 0 : (int)sub.TemplateId,
                            GroupCustomerId = sub.TemplateId.ToString().Length == 13 ? sub.TemplateId : 0,
                            Price = sub.Price,
                            Qty = sub.Qty,
                            Discount = 0,
                            Cdt = DateTime.Now,
                            Udt = DateTime.Now
                        };
                        _db.SubMasterOrder.Add(addSub);
                        _db.SaveChanges();

                        if (sub.TemplateId.ToString().Length >= 13)
                        {
                            var temp = request.OrderDetailDraft
                                .Where(x => x.CustomId == sub.TemplateId)
                                .ToList();

                            foreach (var detail in temp)
                            {
                                _db.DetailOrder.Add(new DetailOrderModel
                                {
                                    SubMasterOrderId = addSub.Id,
                                    GroupCustomerId = sub.TemplateId,
                                    CategoryDetailId = detail.FlowerId,
                                    Price = detail.FlowerPrice,
                                    Qty = detail.Stems,
                                    Discount = 0,
                                    Cdt = DateTime.Now,
                                    Udt = DateTime.Now
                                });

                                _db.DetailOrder.Add(new DetailOrderModel
                                {
                                    SubMasterOrderId = addSub.Id,
                                    GroupCustomerId = sub.TemplateId,
                                    CategoryDetailId = detail.PaperId,
                                    Price = detail.PaperPrice,
                                    Qty = 1,
                                    Discount = 0,
                                    Cdt = DateTime.Now,
                                    Udt = DateTime.Now
                                });
                            }
                        }
                        _db.SaveChanges();

                    }

                }


                return Ok(new
                {
                    success = true,
                    id = addOrder.Id,
                });

            }

         
            return Ok(new
            {
                success = false,
                id = request.Id,
            });

        }


        public async Task SendLineMessage(string userId, string message)
        {
            var accessToken = "Q/I+hy8i3TUQAVXgRLvOftE8RZL3q34Tcs1Jc6QePDpeWzC5CFRNGB1ZUkSrnzY3rWjoOt567ztZWCZpwKyQhKadYNqZayxw8MymND08wwSfAVTDhXXV2y5RuWI4DpM997EUome00mJEFuWC1O4urAdB04t89/1O/w1cDnyilFU=";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

                var payload = new
                {
                    to = userId,
                    messages = new[]
                    {
                new
                {
                    type = "text",
                    text = message
                }
            }
                };

                var content = new StringContent(
                    JsonConvert.SerializeObject(payload),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync(
                    "https://api.line.me/v2/bot/message/push",
                    content);

                var result = await response.Content.ReadAsStringAsync();

                Console.WriteLine(result);
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadSlip(int OrderId, IFormFile SlipImage)
        {
            if (SlipImage == null || SlipImage.Length == 0)
                return Json(new { success = false });

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var ext = Path.GetExtension(SlipImage.FileName);
            var newFileName = $"Slip_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                SlipImage.CopyTo(stream);
            }

            var dbPath = "/uploads/" + newFileName;

            var addImages = new ImageModel
            {
                RelationId = OrderId,  
                Path = dbPath,
                TypeImage = 5,    
                StatusUse = 1,
                Cdt = DateTime.Now,
                Udt = DateTime.Now
            };

            _db.Image.Add(addImages);
            _db.SaveChanges();

            var order = _db.MasterOrder.FirstOrDefault(x => x.Id == OrderId);
            if (order != null)
            {
                order.Status = 1;
                _db.SaveChanges();
            }



            var message = $@"🔔 แจ้งเตือนคำสั่งซื้อใหม่
🧾 เลขที่คำสั่งซื้อ: {order.OrderNo}
💰 ยอดรวม: {order.TotalPrice} บาท 
📅 วันที่สั่งซื้อ: {order.OrderDate:dd/MM/yyyy HH:mm}

 กรุณาตรวจสอบรายละเอียด:
https://flamingoflower.digital/Monitor
                                            ";

            SendLineMessage(HttpContext.Session.GetString("userIdLine"), message);


            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult Review(int OrderId, int rang, string comment)
        {
            var master = _db.SubMasterOrder
                .Where(g =>
                    g.MasterOrderId == OrderId
                )
                .ToList();

            if (master == null)
                return Ok(new { found = false });

            foreach (var detail in master)
            {
                detail.Reating = rang;
                detail.ResonDetail = comment;

                _db.SubMasterOrder.Update(detail);
            }
            _db.SaveChanges();

            return Ok(new
            {
                found = true
            });

        }

        [HttpPost]
        public IActionResult checkPromotion([FromBody]PromotionViewDto request)
        {
            var today = DateTime.Today;

            var checkPromotion = _db.Promotion
                .Where(g =>
                    g.StatusUse == 1 &&
                    g.Name.Trim().ToUpper() == request.Name.Trim().ToUpper() &&
                    g.StartDate.Date <= today &&
                    g.EndDate.Date >= today
                )
                .FirstOrDefault();


            if (checkPromotion == null)
                return Ok(new { found = false });

            return Ok(new
            {
                found = true,
                data = checkPromotion.PercentCal,
                id = checkPromotion.Id
            });

        }

        public IActionResult History()
        {
            ViewBag.role = "customer";

            var sessionUserId = HttpContext.Session.GetInt32("UserId");
            var sessionUserIdLine = HttpContext.Session.GetString("UserName");

            List<MasterOrderModel> orders;
            List<SubMasterOrderModel> suborders;
            List<DetailOrderModel> detailorders;
            List<TemplateFlowerModel> templateflowers;

            if (sessionUserId.HasValue && sessionUserId.Value > 0)
            {
                orders = _db.MasterOrder
                    .Where(x => x.UserId == sessionUserId.Value)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(sessionUserIdLine))
            {
                orders = _db.MasterOrder
                    .Where(x => x.UserIdLine == sessionUserIdLine)
                    .ToList();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

            var orderIds = orders.Select(o => o.Id).ToList();

            suborders = _db.SubMasterOrder
                .Where(x => orderIds.Contains(x.MasterOrderId))
                .ToList();

            var suborderIds = suborders.Select(o => o.Id).ToList();

            detailorders = _db.DetailOrder
                .Where(x => suborderIds.Contains(x.SubMasterOrderId))
                .ToList();


            var teamplateId = suborders.Select(o => o.TemplateId).ToList();

            templateflowers = _db.TemplateFlower
                             .Where(x => teamplateId.Contains(x.Id))
                             .ToList();


            var model = new OrderViewDto
            {
               MasterOrder = orders,
               SubMasterOrder = suborders,
               OrdertDetail = detailorders,
               TemplateFlower = templateflowers
            };

            return View(model);
        }


        public IActionResult Question()
        {
            ViewBag.role = "customer";



            return View();
        }



    }
}
