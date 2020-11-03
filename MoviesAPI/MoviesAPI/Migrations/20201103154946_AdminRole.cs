using Microsoft.EntityFrameworkCore.Migrations;

namespace MoviesAPI.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                Insert into AspNetRoles (Id, [name], [NormalizedName])
                values ('f7f8fd7d-6091-4746-8968-8cd2995bee64', 'Admin', 'Admin')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"delete AspNetRoles
                where id = 'f7f8fd7d-6091-4746-8968-8cd2995bee64'");
        }
    }
}
