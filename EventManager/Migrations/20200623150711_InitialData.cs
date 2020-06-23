using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EventManager.Migrations
{
    public partial class InitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountType", "BirthDate", "City", "Country", "CreatedDate", "Email", "FirstName", "FriendsAmount", "LastName", "Password", "PhoneNumber", "SecondName" },
                values: new object[] { new Guid("bd3d0e41-e0a1-4d91-8c58-5e39f01f3d0f"), "admin", new DateTime(1997, 8, 12, 7, 22, 16, 0, DateTimeKind.Unspecified), null, null, new DateTime(2020, 6, 23, 15, 7, 10, 531, DateTimeKind.Utc).AddTicks(6792), "some@gmail.com", "Alexander", 0, "Maglev", "Qazwsxedc1!", "+79999999999", null });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountType", "BirthDate", "City", "Country", "CreatedDate", "Email", "FirstName", "FriendsAmount", "LastName", "Password", "PhoneNumber", "SecondName" },
                values: new object[] { new Guid("2dc36a8a-c0fd-44fd-9781-42a66cd45237"), "admin", new DateTime(1999, 8, 12, 12, 22, 16, 0, DateTimeKind.Unspecified), null, null, new DateTime(2020, 6, 23, 15, 7, 10, 531, DateTimeKind.Utc).AddTicks(8872), "some@mail.com", "Ivan", 0, "Maglev", "Qazwsxedc1!", "+79999999998", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("2dc36a8a-c0fd-44fd-9781-42a66cd45237"));

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: new Guid("bd3d0e41-e0a1-4d91-8c58-5e39f01f3d0f"));
        }
    }
}
