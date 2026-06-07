using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartBarcodePOS_Pro.Models;

namespace SmartBarcodePOS_Pro.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<SalesBill> SalesBills => Set<SalesBill>();
    public DbSet<SalesBillItem> SalesBillItems => Set<SalesBillItem>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<ErrorLog> ErrorLogs => Set<ErrorLog>();

    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<SalesReturn> SalesReturns => Set<SalesReturn>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);



        builder.Entity<ErrorLog>(entity =>
        {
            entity.Property(x => x.ControllerName).HasMaxLength(100);
            entity.Property(x => x.ActionName).HasMaxLength(100);
            entity.Property(x => x.RequestPath).HasMaxLength(500);
            entity.Property(x => x.UserName).HasMaxLength(150);
        });

        builder.Entity<AppSetting>(entity =>
        {
            entity.Property(x => x.ShopName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30);
            entity.Property(x => x.GstNumber).HasMaxLength(30);
            entity.Property(x => x.CurrencySymbol).HasMaxLength(10).IsRequired();
            entity.Property(x => x.FooterMessage).HasMaxLength(250).IsRequired();
            entity.Property(x => x.PrintSize).HasMaxLength(20).IsRequired();
        });

        builder.Entity<Product>(e =>
        {
            e.HasIndex(x => x.SKU).IsUnique();
            e.Property(x => x.SKU).HasMaxLength(50).IsRequired();
            e.Property(x => x.ProductName).HasMaxLength(150).IsRequired();
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.CostPrice).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.Brand)
                .WithMany()
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<Category>(e =>
        {
            e.Property(x => x.CategoryName).HasMaxLength(100).IsRequired();

            e.HasOne(x => x.Brand)
                .WithMany()
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SalesBill>(e =>
        {
            e.HasIndex(x => x.BillNo).IsUnique();
            e.Property(x => x.BillNo).HasMaxLength(50).IsRequired();
            e.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountPercent).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        });

        builder.Entity<SalesBillItem>(e =>
        {
            e.Property(x => x.Price).HasColumnType("decimal(18,2)");
            e.Property(x => x.Total).HasColumnType("decimal(18,2)");
            e.HasOne(x => x.SalesBill).WithMany(x => x.Items).HasForeignKey(x => x.SalesBillId);
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Brand>(entity =>
        {
            entity.HasIndex(x => x.BrandName).IsUnique();
            entity.Property(x => x.BrandName).HasMaxLength(100).IsRequired();
        });

        builder.Entity<Customer>(entity =>
        {
            entity.Property(x => x.CustomerName).HasMaxLength(150).IsRequired();
            entity.Property(x => x.MobileNo).HasMaxLength(20).IsRequired();
        });

        builder.Entity<Purchase>(entity =>
        {
            entity.HasIndex(x => x.PurchaseNo).IsUnique();
            entity.Property(x => x.TotalAmount).HasColumnType("decimal(18,2)");
        });

        builder.Entity<PurchaseItem>(entity =>
        {
            entity.Property(x => x.CostPrice).HasColumnType("decimal(18,2)");

            entity.Property(x => x.Total).HasColumnType("decimal(18,2)");
        });

    }
}
