using FlamingoModel.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FlamingoShopDemo.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        
        public DbSet<CategoryGroupModel> CategoryGroup { get; set; }
        public DbSet<CategoryDetailModel> CategoryDetail { get; set; }
        public DbSet<ImageModel> Image { get; set; }
        public DbSet<LogAddCategoryDetailModel> LogAddCategoryDetail { get; set; }
        public DbSet<TemplateFlowerModel> TemplateFlower { get; set; }
        public DbSet<DetailTemplateFlowerModel> DetailTemplateFlower { get; set; }
        public DbSet<PromotionModel> Promotion { get; set; }
        public DbSet<UserModel> User { get; set; }
    }
}
