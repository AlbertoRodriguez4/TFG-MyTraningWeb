using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aa2_alberto_rodriguez_penalva.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges20260501 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyPart",
                table: "exercises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "exercises",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<List<string>>(
                name: "InstructionSteps",
                table: "exercises",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "SecondaryMuscles",
                table: "exercises",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Target",
                table: "exercises",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyPart",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "InstructionSteps",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "SecondaryMuscles",
                table: "exercises");

            migrationBuilder.DropColumn(
                name: "Target",
                table: "exercises");
        }
    }
}
