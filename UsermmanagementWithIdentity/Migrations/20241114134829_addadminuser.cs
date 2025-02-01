using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UsermmanagementWithIdentity.Migrations
{
    /// <inheritdoc />
    public partial class addadminuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "6bf7ca0a-0a34-4e46-9cd9-218b3939b72d", "5af36c8b-9ad8-4452-9796-d53b574a5960" },
                schema: "Security"

                );
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
