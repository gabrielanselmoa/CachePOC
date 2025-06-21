CachePOC: .NET Minimal API with Caching Strategies (PoC)
========================================================

This project is a Proof-of-Concept (PoC) demonstrating a simple .NET 8 Minimal API application integrated with **SQLite** for data persistence and exploring different **caching strategies**, including **in-memory caching** and **Redis**. The primary goal is to showcase the implementation and benefits of caching in a streamlined architectural setup.

* * * * *

🎯 Project Overview
-------------------

CachePOC is designed with a focus on simplicity and clarity, specifically tailored for a PoC. It exposes basic CRUD operations for a `Product` entity through Minimal API endpoints. The core highlight is the implementation of caching layers to improve performance and reduce database load.

* * * * *

🚀 Technologies Used
--------------------

-   **[.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)**: The core framework for the application.
-   **[ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis?view=aspnetcore-8.0)**: For building lightweight and fast HTTP APIs with minimal boilerplate.
-   **[SQLite](https://www.sqlite.org/index.html)**: A lightweight, file-based relational database, ideal for PoCs and development environments.
-   **[Entity Framework Core (EF Core)](https://learn.microsoft.com/en-us/ef/core/)**: ORM for interacting with the SQLite database.
-   **[Microsoft.Extensions.Caching.Memory](https://www.google.com/search?q=https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.memory)**: .NET's built-in in-memory caching mechanism.
-   **[StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)**: A high-performance .NET client for Redis, used for distributed caching.

* * * * *

🏗️ Architecture
----------------

This project is structured to demonstrate a clear separation of concerns, even within a single-project PoC using Minimal APIs. The architecture aims for simplicity while adhering to principles of clean architecture.

-   **📁 Domain Layer**: Contains the core business entities (`Product`) and DTOs (`ProductRequest`, `ProductResponse`).

-   **💡 Application Layer**: Houses the `IRepository` interface and its concrete `Repository` implementation, responsible for data access logic and incorporating caching. This layer acts as the application's logical core, interacting with the infrastructure.

-   **🛠️ | 🌐 Infrastructure/Presentation (Minimal API)**: The `Program.cs` file serves as both the infrastructure setup (configuring DI, database, caching) and the presentation layer, defining the Minimal API endpoints directly. This avoids a separate "Controllers" folder, aligning with the Minimal API philosophy.


<!-- end list -->

```
+-------------------+
|   Presentation    | <---- (Minimal APIs / Program.cs) - Defines Endpoints
+---------+---------+
          | Consumes (Calls Services from)
          v
+-------------------+
|    Application    | <---- (DTOs, Business Logic, Service Implementations)
| (Business Logic)  |       e.g., `ProductService`
+---------+---------+
          | Uses Interfaces from / Implements
          v
+-------------------+    +----------------------+
|      Domain       | <---| Infrastructure     | <---- (EF Core, SQLite, Redis, Memory Cache)
| (Entities, Interfaces)  | (Implementations for Data & Cache)
+-------------------+    +----------------------+

```

* * * * *

⚙️ Setup and Running the Project
--------------------------------

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed.
-   [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional, but recommended for easily running Redis).

### Steps

1.  **Clone the repository:**

    Bash

    ```
    git clone [Your Repository URL]
    cd CachePOC

    ```

2.  **Restore dependencies:**

    Bash

    ```
    dotnet restore

    ```

3.  **Set up SQLite Database & Seed Data:** This project uses EF Core Migrations to create the SQLite database and seed initial data.

    -   **Ensure your `AppDbContext` includes `modelBuilder.Entity<Product>().HasData(...)` for seeding.** This is the recommended EF Core approach for basic data seeding via migrations.
    -   **Run EF Core Migrations:**

        Bash

        ```
        dotnet ef database update

        ```

        This command will create the `mydatabase.db` file in your project's `bin/Debug/net8.0` (or `bin/Release/net8.0`) directory and populate it with the initial product data.
4.  **Run Redis (Optional, for Distributed Caching):** If you want to test Redis caching, you need a Redis instance running. The easiest way is using Docker:

    Bash

    ```
    docker run --name my-redis -p 6379:6379 -d redis/redis-stack-server:latest

    ```

    Ensure your `appsettings.json` points to the correct Redis connection string (e.g., `localhost:6379`).

5.  **Run the application:**

    Bash

    ```
    dotnet run

    ```

    The application will typically run on `http://localhost:5000` or `https://localhost:7000`.

* * * * *

🔗 API Endpoints
----------------

The API exposes the following endpoints for the `Product` entity:

-   **`GET /products`**: Retrieves all products. This endpoint will demonstrate caching.
-   **`GET /products/{id}`**: Retrieves a single product by ID. This endpoint will demonstrate caching for individual items.
-   **`POST /products`**: Creates a new product.
-   **`PUT /products/{id}`**: Updates an existing product.
-   **`DELETE /products/{id}`**: Deletes a product.

* * * * *

⚡ Caching in Action (PoC Focus)
-------------------------------

The `GetProducts` and `GetProduct` endpoints are configured to utilize caching:

-   **First Request**: Data is fetched from the SQLite database.
-   **Subsequent Requests**: Data is served from the configured cache (in-memory or Redis), significantly improving response times and reducing database load.

You can observe the caching behavior by:

1.  Making an initial `GET /products` request.
2.  Making subsequent `GET /products` requests and noting the faster response times.
3.  Observing the console logs (using `ILogger`) which will indicate whether data was fetched from the database or the cache.

* * * * *

💡 Extending the PoC
--------------------

This PoC provides a solid foundation. Here are some ideas for further exploration:

-   **Implement Cache Invalidation:** Add logic to explicitly remove or update cache entries when data is created, updated, or deleted.
-   **Explore Cache Eviction Policies:** Experiment with different `MemoryCacheEntryOptions` or Redis expiry settings to control cache lifespan.
-   **Error Handling:** Enhance error handling and logging for production readiness.
-   **More Complex Queries:** Apply caching to more complex database queries and aggregations.