using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class AddFieldForRestriction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRestricted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRestricted",
                table: "AspNetUsers");
        }
    }
}
