# InforumBackend

Backend for Inforum

## Tools

### Must have dotnet version 6.x, Visual Studio or VSCode, MSSQL Server 2019 (optional for Windows), Git, [Powershell(Windows only)](https://github.com/PowerShell/PowerShell/releases), Terminal like Zsh or Bash if Linux

## Needed Environment Variables / Config Variables for Project to run

- **"JwtValidIssuer"**: for JWT Issuer
- **"JwtValidAudience"**: for JWT Audience
- **"JwtSecret"**: for JWT Secret. Must be a random generated string atleast 10 characters long
- **"ConnectionStrings:InforumBackendContext"**: for DB Connection String
- **"AllowedHosts"**: for allowed hosts

Example Configurations; Inflated(typically `secrets.json`)

```json
{
  "AllowedHosts": "*",
  "ConnectionStrings:InforumBackendContext": "Server=localhost;Database=InforumDB;User=testUser;Password=Test@123;",
  "JwtValidIssuer": "Inforum",
  "JwtValidAudience": "Inforum",
  "JwtSecret": "AVeryRealSecretKey"
}
```

Example Configurations; Non-Inflated(typically `appsettings.json`)

```json
{
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "InforumBackendContext": "Server=localhost;Database=InforumDB;User=testUser;Password=Test@123;"
  },
  "JwtValidIssuer": "Inforum",
  "JwtValidAudience": "Inforum",
  "JwtSecret": "AVeryRealSecretKey"
}
```

Example DB Connection String

```json
{
  "ConnectionStrings:InforumBackendContext": "Server=host;Database=DBName;User=user;Password=Test@123;"
}
```

## Important Commands

- `dotnet restore`: to restore(install) all needed packages used in Application
- `dotnet build`: to build the Application
- `dotnet run`: to run the application
- `dotnet watch`: to run the application with watcher, useful during development
- `dotnet ef migrations add '<MigrationMessage>'`: to add Migrations
- `dotnet ef database update`: to update Database as per latest migrations

## Heroku related Information

### Note: Heroku does not officially support dotnet deployment

- Buildpack used: https://github.com/jincod/dotnetcore-buildpack
- Environment Variables: same as above along with `ASPNETCORE_ENVIRONMENT` and `HEROKUISH` as per docs of buildpack. First being the nature of Environment, `Production` for deployment and later being for a hackish way of Herokuish Support, set it to `true`
