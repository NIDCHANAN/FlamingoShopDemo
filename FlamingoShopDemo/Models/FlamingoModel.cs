using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace FlamingoModel.Models
{

    public class CategoryGroupModel
    {
        [Key]
        public int Id { get; set; }
        public string NameCategory { get; set; }
        public int StatusUse { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }


    }

    public class CategoryDetailModel
    {
        [Key]
        public int Id { get; set; }
        public int CategoryGroupId { get; set; }
        public int Qty { get; set; }
        public string NameCategory { get; set; }
        public int StatusUse { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }

    }

    public class LogAddCategoryDetailModel
    {
        [Key]
        public int Id { get; set; }
        public int CategoryDetailId { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
    }

    public class TemplateFlowerModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
        public int StatusUse { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
    }

    public class DetailTemplateFlowerModel
    {
        [Key]
        public int Id { get; set; }
        public int TemplateFlowerId { get; set; }
        public int CategoryDetailId { get; set; }
        public Decimal Qty { get; set; }
        public int? StatusUse { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
    }

    public class ImageModel
    {
        [Key]
        public int Id { get; set; }
        public int RelationId { get; set; }
        
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string Path { get; set; }
        public int TypeImage { get; set; }
        public int StatusUse { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }

    }

    public class PromotionModel
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal PercentCal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? StatusUse { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }
    }

    public class UserModel 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public int  ConfirmTel { get; set; }
        public int  Blacklist { get; set; }
        public int  Status { get; set; }
        public int  Role { get; set; }


    }



    public class ProductViewDto
    {
        public List<CategoryGroupModel> Groups { get; set; }
        public List<CategoryDetailModel> Products { get; set; }
        public List<ImageModel> Images { get; set; }
        public List<TemplateFlowerModel> Template { get; set; }
        public List<TemeplateCategoryDto> TempDetail { get; set; }
    }

    public class AddProductDto : CategoryDetailModel
    {
        public List<IFormFile> Images { get; set; }
        public int Price { get; set; }
    }

    public class UpdateStatusDto
    {
        public int Id { get; set; }
        public int StatusUse { get; set; }
        public int Qty { get; set; }
        public int Price { get; set; }

    }

    public class AddTemplateDto : TemplateFlowerModel
    {
        public List<IFormFile> Images { get; set; }
        public string details { get; set; }
    }

    public class DetailTemplateDto
    {
        [Key]
        public int CategoryDetailId { get; set; }
        public int Qty { get; set; }
    }

    public class TemeplateCategoryDto : DetailTemplateFlowerModel
    { 
        public string NameCategory { get; set; }

        public int CategoryGroupId { get; set; }
        public string CategoryGroupName { get; set; }

    }

    public class UserDto : UserModel
    {
        public List<IFormFile> Images { get; set; }
    }

    public class UserViewDto
    {
        public List<UserModel> User { get; set; }
        public List<ImageModel> Images { get; set; }
    }

    public class BouquetRequestDto
    {
        public string Flower { get; set; } = "";
        public string Wrap { get; set; } = "";
        public int Stems { get; set; }
    }

    public class UserCardDto
    {
        public UserModel User { get; set; }
        public ImageModel Image { get; set; }
    }

}

