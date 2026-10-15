using AssetSystem.Consignments.Domain.Consignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetSystem.Consignments.Infrastructure.Persistence;

internal sealed class ConsignmentConfiguration : IEntityTypeConfiguration<Consignment>
{
    public void Configure(EntityTypeBuilder<Consignment> b)
    {
        b.ToTable("consignments");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Number).HasMaxLength(30);
        b.Property(x => x.ConsigneeNumber).HasMaxLength(30);
        b.Property(x => x.ConsigneeName).HasMaxLength(200);
        b.Property(x => x.DepartmentName).HasMaxLength(200);
        b.Property(x => x.Remarks).HasMaxLength(500);
        b.Property(x => x.CancellationReason).HasMaxLength(500);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(x => x.CreatedBy).HasMaxLength(100);
        b.Property(x => x.UpdatedBy).HasMaxLength(100);

        b.OwnsOne(x => x.ReceivingCopy, r =>
        {
            r.Property(x => x.FileName).HasColumnName("ReceivingCopyFileName").HasMaxLength(255);
            r.Property(x => x.ContentType).HasColumnName("ReceivingCopyContentType").HasMaxLength(150);
            r.Property(x => x.SizeBytes).HasColumnName("ReceivingCopySizeBytes");
            r.Property(x => x.StorageKey).HasColumnName("ReceivingCopyStorageKey").HasMaxLength(64);
        });

        b.HasMany(x => x.Lines).WithOne().HasForeignKey("ConsignmentId").OnDelete(DeleteBehavior.Cascade);
        b.Navigation(x => x.Lines).UsePropertyAccessMode(PropertyAccessMode.Field);

        b.HasIndex(x => new { x.CompanyId, x.Number }).IsUnique();
        b.HasIndex(x => new { x.CompanyId, x.ConsigneeId });
    }
}

internal sealed class ConsignmentLineConfiguration : IEntityTypeConfiguration<ConsignmentLine>
{
    public void Configure(EntityTypeBuilder<ConsignmentLine> b)
    {
        b.ToTable("consignment_lines");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.ItemCode).HasMaxLength(50);
        b.Property(x => x.TypeName).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(200);
        b.Property(x => x.SerialNumber).HasMaxLength(100);
        b.Property(x => x.AssetNumber).HasMaxLength(30);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        b.HasIndex(x => x.ItemId);
    }
}
