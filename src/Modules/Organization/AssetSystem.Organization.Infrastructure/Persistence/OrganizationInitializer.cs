using AssetSystem.BuildingBlocks.Infrastructure.Persistence;
using AssetSystem.Organization.Domain.Branches;
using AssetSystem.Organization.Domain.Companies;
using AssetSystem.Organization.Domain.Departments;
using AssetSystem.Organization.Domain.ItemTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AssetSystem.Organization.Infrastructure.Persistence;

internal sealed class OrganizationInitializer(IConfiguration configuration) : MigratingInitializer<OrganizationDbContext>
{
    protected override async Task SeedAsync(OrganizationDbContext db, IServiceProvider services, CancellationToken ct)
    {
        if (!await db.ItemTypes.AnyAsync(ct))
        {
            db.ItemTypes.AddRange(DefaultItemTypes.Select(t => ItemType.Create(t.Code, t.Name)));
            await db.SaveChangesAsync(ct);
        }

        // Demo organisation so a fresh install is usable immediately. Turn off in real deployments.
        if (configuration.GetValue<bool>("Seeding:SampleData") && !await db.Companies.AnyAsync(ct))
        {
            var company = Company.Create("09", "Civic Merchandising Inc.", "77 Mindanao Ave. Bagong Pagasa", "CVC");
            db.Companies.Add(company);
            db.Branches.AddRange(
                Branch.Create(company.Id, "Head Office", "Mindanao Ave.", null),
                Branch.Create(company.Id, "Davao Branch", "Davao City", "DVO"),
                Branch.Create(company.Id, "Cebu Branch", "Cebu City", "CEB"));
            db.Departments.AddRange(
                Department.Create(company.Id, "81", "Admin Department"),
                Department.Create(company.Id, "86", "Computer System Department"),
                Department.Create(company.Id, "80", "Finance Department"));
            await db.SaveChangesAsync(ct);
        }
    }

    private static readonly (string Code, string Name)[] DefaultItemTypes =
    [
        ("M", "Monitor"), ("D", "Desktop"), ("CAS", "Casing"), ("PRO", "Processor"),
        ("RAM", "Memory-Desktop"), ("LAPRAM", "Memory-Laptop"), ("FW", "Firewall Appliance"),
        ("HDD", "Hard Disk Drive"), ("MB", "Motherboard"), ("PS", "Power Supply"),
        ("ODD", "Optical Disk Drive"), ("OTH", "Other / Cable"), ("UPSBATT", "UPS Battery"),
        ("FDD", "Floppy Disk Drive"), ("MSE", "Mouse"), ("KYB", "Keyboard"), ("VGC", "Graphics Card"),
        ("UPS", "UPS"), ("LCO", "Laptop Unit"), ("LAPADPT", "Laptop Adapter"), ("OS", "Operating System"),
        ("OFC", "Office App System"), ("EHDD", "External Hard Drive"), ("UFD", "USB Flash Drive"),
        ("EODD", "External Optical Disk Drive"), ("MFP", "Multi-Function Printer & Scanner"),
        ("P", "Printer"), ("SCR", "Scanner"), ("ROUTR", "Router"), ("SWTCH", "Switch"),
        ("LPT", "LPT Cable"), ("KVM", "KVM Switch"), ("LAPBAT", "Laptop Battery")
    ];
}
