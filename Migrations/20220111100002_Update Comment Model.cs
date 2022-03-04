using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class UpdateCommentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Comment",
                newName: "Description");

            migrationBuilder.AddColumn<long>(
                name: "PostId",
                table: "Comment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_BlogPost_PostId",
                table: "Comment",
                column: "PostId",
                principalTable: "BlogPost",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_AspNetUsers_UserId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_BlogPost_PostId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_PostId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comment");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Comment",
                newName: "Content");
        }
    }
}
