using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class AddHeadImageToSubreddits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeaderImageUrl",
                table: "Subreddits",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeaderImageUrl",
                table: "Subreddits");
        }
    }
}
