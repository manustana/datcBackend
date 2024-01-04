using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectDATC.Migrations
{
    /// <inheritdoc />
    public partial class TesttMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$vzKe6K/eHJEsBwJeTGn30Obv4c63t1H9N/ADkbdiZHGfZ4pfv.S0a");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$HPt3Ew9fcnDbp1JXhOZ.ruiZZJegp0j.sVhp2PWNd0LLUqbu/6keK");
        }
    }
}
