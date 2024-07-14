using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wema_test_service.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateOfResidence = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Lga = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VerificationStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", maxLength: 100, nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Otps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsBody = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SentDate = table.Column<DateTimeOffset>(type: "datetimeoffset", maxLength: 100, nullable: true),
                    VerifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", maxLength: 100, nullable: false),
                    ModifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otps", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CreatedDate",
                table: "Customers",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Id",
                table: "Customers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true,
                filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Otps_CreatedDate",
                table: "Otps",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Otps_Id",
                table: "Otps",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Otps");
        }
    }
}
