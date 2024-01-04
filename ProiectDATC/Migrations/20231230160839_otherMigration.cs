using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectDATC.Migrations
{
    /// <inheritdoc />
    public partial class otherMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$GqcktwOoRrJkEY5E9qaKH.Djm9ykXA3V3fipN62rBA3EDeeQ.WFA.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$MNIrIePVrJjnJZNRQKX7BOH4CBAdaQ74y/yvHwuipVPUh3UL.BPei");
        }
    }
}
