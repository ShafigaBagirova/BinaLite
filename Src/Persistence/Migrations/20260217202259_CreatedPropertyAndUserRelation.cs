using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreatedPropertyAndUserRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "PropertyAd",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "PropertyAd",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PropertyAd",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAd_UserId",
                table: "PropertyAd",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyAd_AspNetUsers_UserId",
                table: "PropertyAd",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyAd_AspNetUsers_UserId",
                table: "PropertyAd");

            migrationBuilder.DropIndex(
                name: "IX_PropertyAd_UserId",
                table: "PropertyAd");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "PropertyAd");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PropertyAd");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PropertyAd");
        }
    }
}
