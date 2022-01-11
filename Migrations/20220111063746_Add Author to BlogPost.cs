using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class AddAuthortoBlogPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "BlogPost",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPost_AuthorId",
                table: "BlogPost",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPost_AspNetUsers_AuthorId",
                table: "BlogPost",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPost_AspNetUsers_AuthorId",
                table: "BlogPost");

            migrationBuilder.DropIndex(
                name: "IX_BlogPost_AuthorId",
                table: "BlogPost");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "BlogPost");
        }
    }
}
