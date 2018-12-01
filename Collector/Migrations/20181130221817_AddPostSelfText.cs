using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class AddPostSelfText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SelfText",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SelfText",
                table: "Posts");
        }
    }
}
