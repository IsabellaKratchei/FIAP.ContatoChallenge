using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIAP.ContatoChallenge.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDDDsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DDDs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DDDs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DDD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Regiao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DDDs", x => x.Id);
                });
        }
    }
}
