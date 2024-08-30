using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Users.Database.Migrations.UserDb
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.CreateTable(
                name: "ApiClient",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ClientName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<int>(type: "integer", nullable: true),
                    CreatedByClientId = table.Column<int>(type: "integer", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<int>(type: "integer", nullable: true),
                    UpdatedByClientId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(130)", maxLength: 130, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdentityId = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: true),
                    LastName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInvite",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InviteCode = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    InvitedByUserId = table.Column<int>(type: "integer", nullable: true),
                    InvitedByTenantId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StatusReason = table.Column<int>(type: "integer", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvite", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    RoleId = table.Column<int>(type: "integer", nullable: true),
                    TenantId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "users",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                schema: "users",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Role_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "users",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_User_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUser",
                schema: "users",
                columns: table => new
                {
                    TenantsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUser", x => new { x.TenantsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_TenantUser_Tenant_TenantsId",
                        column: x => x.TenantsId,
                        principalSchema: "users",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUser_User_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "users",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Name_UserId_RoleId",
                schema: "users",
                table: "Permissions",
                columns: new[] { "Name", "UserId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                schema: "users",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserId",
                schema: "users",
                table: "Permissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UsersId",
                schema: "users",
                table: "RoleUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Slug",
                schema: "users",
                table: "Tenant",
                column: "Slug",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantUser_UsersId",
                schema: "users",
                table: "TenantUser",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "users",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_IdentityId",
                schema: "users",
                table: "User",
                column: "IdentityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiClient",
                schema: "users");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "users");

            migrationBuilder.DropTable(
                name: "RoleUser",
                schema: "users");

            migrationBuilder.DropTable(
                name: "TenantUser",
                schema: "users");

            migrationBuilder.DropTable(
                name: "UserInvite",
                schema: "users");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "users");

            migrationBuilder.DropTable(
                name: "Tenant",
                schema: "users");

            migrationBuilder.DropTable(
                name: "User",
                schema: "users");
        }
    }
}
