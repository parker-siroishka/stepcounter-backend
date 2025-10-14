using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StepCounter.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCircularDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRouteProgress_RouteId",
                table: "UserRouteProgress");

            migrationBuilder.CreateIndex(
                name: "IX_UserRouteProgress_RouteId",
                table: "UserRouteProgress",
                column: "RouteId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserRouteProgress_RouteId",
                table: "UserRouteProgress");

            migrationBuilder.CreateIndex(
                name: "IX_UserRouteProgress_RouteId",
                table: "UserRouteProgress",
                column: "RouteId");
        }
    }
}
