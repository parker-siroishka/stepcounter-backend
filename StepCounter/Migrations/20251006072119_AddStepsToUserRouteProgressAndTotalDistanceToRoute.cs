using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StepCounter.Migrations
{
    /// <inheritdoc />
    public partial class AddStepsToUserRouteProgressAndTotalDistanceToRoute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Steps",
                table: "UserRouteProgress",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalDistance",
                table: "Routes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Steps",
                table: "UserRouteProgress");

            migrationBuilder.DropColumn(
                name: "TotalDistance",
                table: "Routes");
        }
    }
}
