# Cards

Cards is a framework and webserver to create and host card games

## Development Instructions

These instructions will help you to set up your local development environment to contribute.

### Prerequisites

Cards is using [ASP.NET](https://dotnet.microsoft.com/en-us/apps/aspnet), so you need to have
the [.NET 6 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/) installed.

As database, Cards is using [MariaDB](https://mariadb.org/), make sure to have a working installation. 
### Setup secrets

Cards is expecting following config variables, which can be set by environment variables or a `secrets.json` file.
<details>
<summary>Click me!</summary>

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;user=dbuser;password=password;database=cards"
  },
  "EmailSender": {
    "Host": "example.com",
    "Port": 587,
    "EnableSSL": true,
    "Username": "username",
    "Password": "password"
  }
}
```

</details>

### Get it up and running

You can restore all necessary packages with `dotnet restore` and run the application with `dotnet run`.

To load the database tables, import [this](src/cards/db.sql) file or use the `dotnet-ef` tool. Install it
with `dotnet tool install --global dotnet-ef` and run `dotnet ef database update` to apply the latest migration.

## Add a new game

1. Create a new game in `Data/Game/Implementations`
2. Your game has to implement the `IGameService`
3. Add your game to the `Data/Game/GameEnum.cs` enum
4. Add your game in `Data/Lobby.cs` to the switch statement in `StartGame()`
5. Add a static title and description function to your game and add these to `Data/Game/IGameService.cs` static methods

Looking at existing games might help, there are already algorithms for shuffling, card implementations etc.
