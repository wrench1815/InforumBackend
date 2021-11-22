using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class RemoveRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_Category_CategoryId",
                table: "BlogPost");

            migrationBuilder.DropIndex(
                name: "IX_BlogPost_CategoryId",
                table: "BlogPost");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "BlogPost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoryId",
                table: "BlogPost",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_CategoryId",
                table: "BlogPost",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_Category_CategoryId",
                table: "BlogPost",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
