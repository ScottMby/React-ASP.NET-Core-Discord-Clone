using Microsoft.EntityFrameworkCore.Migrations;
using NpgsqlTypes;

#nullable disable

namespace Discord_Clone.Server.Migrations
{
    /// <inheritdoc />
    public partial class UserSearchIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<NpgsqlTsVector>(
                name: "UserSearchVector",
                table: "AspNetUsers",
                type: "tsvector",
                nullable: false)
                .Annotation("Npgsql:TsVectorConfig", "english")
                .Annotation("Npgsql:TsVectorProperties", new[] { "UserName", "DisplayName" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_UserSearchVector",
                table: "AspNetUsers",
                column: "UserSearchVector")
                .Annotation("Npgsql:IndexMethod", "GIN");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_UserSearchVector",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserSearchVector",
                table: "AspNetUsers");
        }
    }
}
