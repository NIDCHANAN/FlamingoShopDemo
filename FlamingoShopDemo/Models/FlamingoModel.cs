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
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
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
        public long RelationId { get; set; }
        
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

    public class MasterOrderModel
    {
        [Key]
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? AddressDelivary { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? Discount { get; set; }
        public int? PaymentType { get; set; }
        public int? MasterDelivaryId { get; set; }
        public string? DelivaryNo { get; set; }
        public int? Status { get; set; }
        public DateTime? Cdt { get; set; }
        public DateTime? Udt { get; set; }
        public string? TelephoneOrder { get; set; }
        public string? fullNameRecrive { get; set; }
        public string? UserIdLine { get; set; }
        public string? OrderNo { get; set; }

    }

    public class SubMasterOrderModel
    {
        [Key]
        public int Id { get; set; }
        public int MasterOrderId { get; set; }
        public long GroupCustomerId { get; set; }
        public int TemplateId { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string? ResonDetail { get; set; }
        public int Reating { get; set; }
        public int Qty { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }

    }

    public class DetailOrderModel
    {
        [Key]
        public int Id { get; set; }
        public long GroupCustomerId { get; set; }
        public int SubMasterOrderId { get; set; }
        public int CategoryDetailId { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Qty { get; set; }
        public DateTime Cdt { get; set; }
        public DateTime Udt { get; set; }

    }
    public class ProductViewDto
    {
        public List<CategoryGroupModel> Groups { get; set; }
        public List<CategoryDetailModel> Products { get; set; }
        public List<ImageModel> Images { get; set; }
        public List<TemplateFlowerModel> Template { get; set; }
        public List<TemeplateCategoryDto> TempDetail { get; set; }
        public List<LogAddCategoryDetailModel> ProductPrice { get; set; }
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


    public class CustomBouquetDto
    {
        public int flower { get; set; } = 0;
        public int qty { get; set; } = 0;
        public int paper { get; set; } = 0;
        public int other { get; set; } = 0;

    }

    public class UserCardDto
    {
        public UserModel User { get; set; }
        public ImageModel Image { get; set; }
    }


    public class MonitorViewDto
    {
        public List<MasterOrderModel> masterOrder { get; set; }
        public List<UserModel> user { get; set; }
    }
    public class DetailCustomRequestDto
    {
        public long CustomId { get; set; } 
        public int FlowerId { get; set; }
        public decimal FlowerPrice { get; set; } 
        public int PaperId { get; set; } 
        public decimal PaperPrice { get; set; } 
        public int Stems { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class SubOrderRequestDto
    {
        public long TemplateId { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }

    public class OrdertDetailDto :  MasterOrderModel
    {
        public List<SubOrderRequestDto> OrderSubDraft { get; set; } = new();
        public List<DetailCustomRequestDto> OrderDetailDraft { get; set; } = new();
    }

    public class OrderViewDto
    {
        public List<MasterOrderModel> MasterOrder { get; set; }
        public List<SubMasterOrderModel> SubMasterOrder { get; set; }
        public List<DetailOrderModel> OrdertDetail { get; set; }
        public List<TemplateFlowerModel> TemplateFlower { get; set; }
    }
}

