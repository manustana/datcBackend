using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectDATC.Migrations
{
    /// <inheritdoc />
    public partial class FinalMigrationPerhapsMaybe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                            name: "Status",
                            table: "Reports",
                            type: "nvarchar(450)",
                            nullable: true,
                            defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
