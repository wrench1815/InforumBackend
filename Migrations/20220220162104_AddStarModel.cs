using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class AddStarModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Star",
                table: "BlogPost",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Star",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogPostId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Star", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Star");

            migrationBuilder.DropColumn(
                name: "Star",
                table: "BlogPost");
        }
    }
}
