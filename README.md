# .NET Practice
## Scenarios
* User
  * Register/login
    * By email and password
  * Get all products with
    * Pagination
    * Filter products
      * Name
      * Category
      * Price
    * Order
* Admin
  * Login
  * CRUD products
  * Get all products with
    * Pagination
    * Filter products
      * Name
      * Category
      * Price
    * Order
  * CRUD users
    * Set user roles
## Tech stacks
* Source code analysis
  * Using .NET Analyzer
* Back-end:
  * ASP.NET 6
  * Entity Framework Core
  * Dependency Injection
  * Automapper
  * Swashbuckle.AspNetCore
  * FluentValidation.AspNetCore
  * Serilog
  * Custom middleware to show request and response with timeline
  * Xunit && MOQ for Unit Testing
  * Database Seeding
* Database:
  * SQL Server
## Directory structure
```
.
├── src
│   ├── API
│   │   ├── Controllers
│   │   ├── Extensions
│   │   ├── Logs
│   │   ├── Middleware
│   │   └── Properties
│   ├── Application
│   │   ├── Categories
│   │   ├── Dto
│   │   │   ├── Auth
│   │   │   ├── Category
│   │   │   ├── Product
│   │   │   └── User
│   │   ├── Errors
│   │   ├── Helpers
│   │   ├── Interfaces
│   │   └── Services
│   ├── Domain
│   │   ├── Common
│   │   ├── Entities
│   │   ├── Exceptions
│   │   └── Specification
│   │       ├── Product
│   │       ├── Token
│   │       └── User
│   └── Infrastructure
│       ├── Data
│       │   ├── Configuration
│       │   ├── Migrations
│       │   └── SeedData
│       └── Repositories
└── tests
    ├── IntegrationTest
    │   └── Setup
    └── UnitTest
        ├── Controller
        ├── Repository
        │   └── Specification
        ├── Service
        │   └── Setup
        └── TestReport


```
## Run project
### Setup
#### Without Docker
* Install [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* Install [Visual Studio](https://visualstudio.microsoft.com/vs/) or [Jetbrains Rider](https://www.jetbrains.com/rider/)
#### With Docker
* Install [Docker](https://www.docker.com/)
### Run
#### Without Docker
1. Open project in Visual Studio or Jetbrains Rider
2. Change the connection string with your SQL Server setup
   * **_Step 1_**: Go to `src/API/appsettings.json`
   * **_Step 2_**: Change the value of `"Store": "Server=localhost;Database=Practice;User Id=sa;Password=Qwe12345@;"`
     * If you change want to change server port, replace `localhost` with `localhost,{port}`
       * Example: `localhost,1433`
     * Change `User Id=sa` to your server username
       * Example: `User Id=myserver`
     * Change `Password=Qwe12345@` to your server password
       * Example: `Password=Abcd999!`
     * > Or checkout other connection string setup [here](https://www.connectionstrings.com/sql-server/)
3. Run the `API` project
4. Open a browser and go to `https://localhost:7226/swagger/index.html` for checking API Documentation
#### With Docker
1. Check whether the port `1433` and port `8081` in your computer are free
   * If not, go to `docker-compose.yml` file
     * Change the port of `store-server` from `1433:1433` to `{free-port}:1433`
       * Example: `1434:1433`
     * Change the port of `store-app` from `8081:80` to `{free-port}:80`
       * Example: `8080:80`
2. In Terminal or Command line: `docker compose up`
3. Open browser and go to `http://localhost:8081/swagger/index.html` or `http://localhost:{your-port}/swagger/index.html`
