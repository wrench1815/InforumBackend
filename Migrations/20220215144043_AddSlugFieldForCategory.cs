using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class AddSlugFieldForCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Name",
                table: "Category",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_Name",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
