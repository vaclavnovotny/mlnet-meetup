using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MLNET.RealWorld.Migrations
{
    public partial class AddedKeyAnnotation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrainedModels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelData = table.Column<byte[]>(nullable: true),
                    Accuracy = table.Column<double>(nullable: false),
                    TrainedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainedModels", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainedModels");
        }
    }
}
