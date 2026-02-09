using FlamingoModel.Models;
using FlamingoShopDemo.Data;
using Microsoft.AspNetCore.Mvc;

namespace FlamingoShopDemo.Controllers.Customer
{
    public class CustomerController : BaseController
    {
        private readonly ApplicationDBContext _db;

        public CustomerController(ApplicationDBContext db) : base(db)
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
                Images = ProductImage,
                Template = TemplateMaster,
                TempDetail = TemplateDetail
            };

            return View();
        }


    }
}
