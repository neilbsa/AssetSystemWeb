using AssetSystem.Assets.Domain.Assets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetSystem.Assets.Infrastructure.Persistence.Configurations;

internal sealed class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> b)
    {
        b.ToTable("assets");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Category).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.AssetNumber).HasMaxLength(30);
        b.Property(x => x.Description).HasMaxLength(200);
        b.Property(x => x.Vendor).HasMaxLength(200);
        b.Property(x => x.PurchaseOrderNumber).HasMaxLength(100);
        b.Property(x => x.ControlNumber).HasMaxLength(100);
        b.Property(x => x.Remarks).HasMaxLength(500);
        b.Property(x => x.CreatedBy).HasMaxLength(100);
        b.Property(x => x.UpdatedBy).HasMaxLength(100);

        b.OwnsOne(x => x.Purchase, p =>
        {
            p.Property(x => x.Price).HasColumnName("PurchasePrice").HasPrecision(18, 2);
            p.Property(x => x.Date).HasColumnName("PurchaseDate");
            p.Property(x => x.LifeSpanMonths).HasColumnName("LifeSpanMonths");
            p.Property(x => x.WarrantyMonths).HasColumnName("WarrantyMonths");
        });
        b.Navigation(x => x.Purchase).IsRequired();

        b.OwnsOne(x => x.Invoice, i =>
        {
            i.Property(x => x.Number).HasColumnName("InvoiceNumber").HasMaxLength(100);
            i.Property(x => x.Date).HasColumnName("InvoiceDate");
            i.Property(x => x.SupplierId).HasColumnName("SupplierId");
            i.Property(x => x.SupplierName).HasColumnName("SupplierName").HasMaxLength(200);
        });
        b.Navigation(x => x.Invoice).IsRequired();

        b.HasMany(x => x.Items).WithOne().HasForeignKey("AssetId").OnDelete(DeleteBehavior.Cascade);
        b.Navigation(x => x.Items).UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasMany(x => x.Documents).WithOne().HasForeignKey("AssetId").OnDelete(DeleteBehavior.Cascade);
        b.Navigation(x => x.Documents).UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasIndex(x => x.AssetNumber).IsUnique();
        b.HasIndex(x => new { x.CompanyId, x.Category });
    }
}

internal sealed class AssetItemConfiguration : IEntityTypeConfiguration<AssetItem>
{
    public void Configure(EntityTypeBuilder<AssetItem> b)
    {
        b.ToTable("asset_items");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.ItemCode).HasMaxLength(50);
        b.Property(x => x.TypeCode).HasMaxLength(20);
        b.Property(x => x.TypeName).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(200);
        b.Property(x => x.PartNumber).HasMaxLength(100);
        b.Property(x => x.SerialNumber).HasMaxLength(100);
        b.Property(x => x.KeyDetail).HasMaxLength(200);
        b.Property(x => x.ControlNumber).HasMaxLength(100);
        b.Property(x => x.Remarks).HasMaxLength(500);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);

        b.HasIndex(x => x.ItemCode).IsUnique();
        b.HasIndex(x => new { x.CompanyId, x.Status });

        // Real serial numbers / keys are unique per company; "N/A" may repeat.
        b.HasIndex(x => new { x.CompanyId, x.SerialNumber }).IsUnique().HasFilter("\"SerialNumber\" <> 'N/A'");
        b.HasIndex(x => new { x.CompanyId, x.KeyDetail }).IsUnique().HasFilter("\"KeyDetail\" <> 'N/A'");
    }
}

internal sealed class AssetDocumentConfiguration : IEntityTypeConfiguration<AssetDocument>
{
    public void Configure(EntityTypeBuilder<AssetDocument> b)
    {
        b.ToTable("asset_documents");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.DocumentType).HasMaxLength(100);
        b.Property(x => x.FileName).HasMaxLength(255);
        b.Property(x => x.ContentType).HasMaxLength(150);
        b.Property(x => x.StorageKey).HasMaxLength(64);
    }
}
