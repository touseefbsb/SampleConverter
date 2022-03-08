using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shapr3D_Converter.Models.Migrations
{
    public partial class DBCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModelEntities",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OriginalPath = table.Column<string>(nullable: true),
                    FileSize = table.Column<ulong>(nullable: false),
                    ObjConverted = table.Column<bool>(nullable: false),
                    StlConverted = table.Column<bool>(nullable: false),
                    StepConverted = table.Column<bool>(nullable: false),
                    FileBytes = table.Column<byte[]>(nullable: true),
                    StlFileBytes = table.Column<byte[]>(nullable: true),
                    ObjFileBytes = table.Column<byte[]>(nullable: true),
                    StepFileBytes = table.Column<byte[]>(nullable: true),
                    ThumbnailBytes = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelEntities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModelEntities");
        }
    }
}
