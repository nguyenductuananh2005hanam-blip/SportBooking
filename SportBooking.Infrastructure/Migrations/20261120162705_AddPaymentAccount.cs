using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportBooking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VenuePaymentAccountId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VenuePaymentAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VenueId = table.Column<int>(type: "int", nullable: false),
                    BankBin = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    QrImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VenuePaymentAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VenuePaymentAccounts_Venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "Venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_VenuePaymentAccountId",
                table: "Payments",
                column: "VenuePaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VenuePaymentAccounts_VenueId_AccountNumber",
                table: "VenuePaymentAccounts",
                columns: new[] { "VenueId", "AccountNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_VenuePaymentAccounts_DefaultPerVenue",
                table: "VenuePaymentAccounts",
                column: "VenueId",
                unique: true,
                filter: "[IsDefault] = 1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_VenuePaymentAccounts_VenuePaymentAccountId",
                table: "Payments",
                column: "VenuePaymentAccountId",
                principalTable: "VenuePaymentAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_VenuePaymentAccounts_VenuePaymentAccountId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "VenuePaymentAccounts");

            migrationBuilder.DropIndex(
                name: "IX_Payments_VenuePaymentAccountId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "VenuePaymentAccountId",
                table: "Payments");
        }
    }
}
