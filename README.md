# .NET-Office-Management-BackEnd [Work in progress, not finished yet!]
Office Mangement Backend using .NET 6 Web Api with JWT authentication &amp; MariaDB

Note: JWT token in not saved anywhere. This project only shows how we can use JWT token.
We can save it in database and also use refresh token to generated new access token after some time peroid.
These features may or may not be implemented in future.

Instruction:
1. dotnet --list-sdks
2. dotnet new webapi -f net6.0
3. dotnet build
4. dotnet run

Entity Framework Core:
1. dotnet add package Microsoft.EntityFrameworkCore.SqlServer
2. dotnet tool install --global dotnet-ef
3. dotnet add package Microsoft.EntityFrameworkCore.Design
4. dotnet ef migrations add <migration_name>
5. dotnet ef database update
6. dotnet ef migrations list
7. dotnet ef migrations remove
8. dotnet tool update --global dotnet-ef

MariaDB:
1. dotnet add package Pomelo.EntityFrameworkCore.MySql --version 7.0.0-silver.1

Database Scaffolding / Reverse Engineering:
1. dotnet ef dbcontext scaffold -o "<folder_name>" "Server=localhost;User=<user>;Password=<password>;Database=<db_name>" "Pomelo.EntityFrameworkCore.MySql"

Httprepl
1. dotnet tool install -g Microsoft.dotnet-httprepl
2. httprepl https://localhost:{PORT}
3. some commands, ls / cd / get <optional_parameter>/ post -c "{"key1": "string_value", "key2": boolean_value_true_false}" / delete / exit etc.

Misc.
1. dotnet tool update <package_name>
2. dotnet dev-certs https --trust
3. For certificate issue go to "chrome://flags/" or "edge://flags/", then enable "Allow invalid certificates for resources loaded from localhost."

How to register new user:
1. Open postman and enter below url,
    https://localhost:7000/api/user/register

2. In body tab enter below information,
{
    "username":"user_name",
    "password":"password",
    "super": false
}

3. Disable ssl verification in postman if needed.
4. Set first name (first name is mandatory to show it in a list) and access permission from phpmyadmin.


Create Angualr Project
dotnet new angular -o Office-Management-.NET-MVC-Angular-JWT

TODO:
fix appsettings.Developement.json not able to read
Specify CORS policy


