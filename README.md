# SqlServer-DbService
A lightweight C# data access service for SQL Server using ADO.NET. Supports synchronous and asynchronous queries, CRUD operations, stored procedures, parameter mapping, and DataTable-to-Dictionary conversion. Designed for ASP.NET Core applications with dependency injection and efficient database access.
# SqlServer-DbService

A lightweight C# data access service for Microsoft SQL Server, built with ADO.NET and `Microsoft.Data.SqlClient`.

## Features

- Synchronous and asynchronous database operations.
- Query execution with `DataTable` results.
- Non-query execution for INSERT, UPDATE, and DELETE operations.
- Scalar query execution.
- Stored procedure execution.
- Stored procedure parameter discovery.
- Parameter mapping from dictionaries.
- Conversion of `DataTable` results into lists of dictionaries.
- Dependency injection support through `IConfiguration`.

## Requirements

- .NET SDK compatible with the project target framework.
- Microsoft SQL Server.
- The `Microsoft.Data.SqlClient` NuGet package.
- A configured SQL Server connection string.

## Configuration

Configure the connection string through your application's configuration system. Do not commit production credentials or secrets to source control.

## Usage

Register `DbService` in your application's dependency injection container and inject `IDbServices` into the components that require database access.

## Design

This project uses ADO.NET directly and does not require Entity Framework Core. It is intended for applications that need explicit control over SQL commands, parameters, and stored procedures.

## Limitations

- Stored procedure parameter discovery requires a connection to SQL Server.
- The `ReadTable` helper assumes a stored procedure contract that includes an `Operation` parameter accepting `Read`.
- Database integration tests require a configured SQL Server instance.

## License

See the `LICENSE` file for licensing terms.
