using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication25.Migrations
{
    public partial class init000 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilesInfos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataVolume = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IpInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAddress = table.Column<byte[]>(type: "varbinary(16)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MainTable",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    _IPinfoId = table.Column<long>(type: "bigint", nullable: true),
                    FilesInfoId = table.Column<long>(type: "bigint", nullable: true),
                    DateTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestResult = table.Column<int>(type: "int", nullable: false),
                    DataVolume = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainTable_FilesInfos_FilesInfoId",
                        column: x => x.FilesInfoId,
                        principalTable: "FilesInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MainTable_IpInfo__IPinfoId",
                        column: x => x._IPinfoId,
                        principalTable: "IpInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainTable__IPinfoId",
                table: "MainTable",
                column: "_IPinfoId");

            migrationBuilder.CreateIndex(
                name: "IX_MainTable_FilesInfoId",
                table: "MainTable",
                column: "FilesInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainTable");

            migrationBuilder.DropTable(
                name: "FilesInfos");

            migrationBuilder.DropTable(
                name: "IpInfo");
        }
    }
}
