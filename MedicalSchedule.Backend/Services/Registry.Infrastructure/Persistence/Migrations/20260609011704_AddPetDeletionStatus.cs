using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registry.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPetDeletionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeletionRejectionReason",
                table: "Pets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletionStatus",
                table: "Pets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletionRejectionReason",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "DeletionStatus",
                table: "Pets");
        }
    }
}
