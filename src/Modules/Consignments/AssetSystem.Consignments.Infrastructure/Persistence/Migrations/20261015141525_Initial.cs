using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetSystem.Consignments.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "consignments");

            migrationBuilder.CreateTable(
                name: "consignments",
                schema: "consignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ConsigneeId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsigneeNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    ConsigneeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DepartmentName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IssuedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CancellationReason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ReceivingCopyFileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ReceivingCopyContentType = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ReceivingCopySizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    ReceivingCopyStorageKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consignments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "sequences",
                schema: "consignments",
                columns: table => new
                {
                    Key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sequences", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "consignment_lines",
                schema: "consignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TypeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SerialNumber = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AssetNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ConsignmentId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_consignment_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_consignment_lines_consignments_ConsignmentId",
                        column: x => x.ConsignmentId,
                        principalSchema: "consignments",
                        principalTable: "consignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_consignment_lines_ConsignmentId",
                schema: "consignments",
                table: "consignment_lines",
                column: "ConsignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_consignment_lines_ItemId",
                schema: "consignments",
                table: "consignment_lines",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_consignments_CompanyId_ConsigneeId",
                schema: "consignments",
                table: "consignments",
                columns: new[] { "CompanyId", "ConsigneeId" });

            migrationBuilder.CreateIndex(
                name: "IX_consignments_CompanyId_Number",
                schema: "consignments",
                table: "consignments",
                columns: new[] { "CompanyId", "Number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "consignment_lines",
                schema: "consignments");

            migrationBuilder.DropTable(
                name: "sequences",
                schema: "consignments");

            migrationBuilder.DropTable(
                name: "consignments",
                schema: "consignments");
        }
    }
}
