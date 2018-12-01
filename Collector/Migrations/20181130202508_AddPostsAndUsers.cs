using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Collector.Migrations
{
    public partial class AddPostsAndUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    HideFromRobots = table.Column<bool>(nullable: false),
                    CommentKarma = table.Column<int>(nullable: false),
                    LinkKarma = table.Column<int>(nullable: false),
                    IsVerified = table.Column<bool>(nullable: false),
                    IsModerator = table.Column<bool>(nullable: false),
                    HasGold = table.Column<bool>(nullable: false),
                    HasSubscribed = table.Column<bool>(nullable: false),
                    HasVerifiedEmail = table.Column<bool>(nullable: true),
                    IsEmployee = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<int>(nullable: false),
                    SubredditId = table.Column<int>(nullable: false),
                    PostId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    ThumbnailUrl = table.Column<string>(nullable: true),
                    Nsfw = table.Column<bool>(nullable: false),
                    Upvotes = table.Column<int>(nullable: false),
                    Downvotes = table.Column<int>(nullable: false),
                    IsArchived = table.Column<bool>(nullable: true),
                    IsApproved = table.Column<bool>(nullable: true),
                    IsRemoved = table.Column<bool>(nullable: true),
                    ReportCount = table.Column<int>(nullable: true),
                    Score = table.Column<int>(nullable: false),
                    Kind = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_Subreddits_SubredditId",
                        column: x => x.SubredditId,
                        principalTable: "Subreddits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SubredditId",
                table: "Posts",
                column: "SubredditId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
