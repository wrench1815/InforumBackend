using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InforumBackend.Migrations
{
    public partial class AddVoteFieldOnForumQuery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Vote",
                table: "ForumQuery",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vote",
                table: "ForumQuery");
        }
    }
}
