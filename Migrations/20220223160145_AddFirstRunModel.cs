using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class AddFirstRunModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Heading",
                table: "Home",
                newName: "SubHeading");

            migrationBuilder.RenameColumn(
                name: "HeaderImageLink",
                table: "Home",
                newName: "HeaderImage");

            migrationBuilder.CreateTable(
                name: "FirstRun",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FirstRun", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FirstRun");

            migrationBuilder.RenameColumn(
                name: "SubHeading",
                table: "Home",
                newName: "Heading");

            migrationBuilder.RenameColumn(
                name: "HeaderImage",
                table: "Home",
                newName: "HeaderImageLink");
        }
    }
}
