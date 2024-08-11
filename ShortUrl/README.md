# URL Shortener

A simple URL shortener application built with ASP.NET Core, Entity Framework Core, and PostgreSQL.

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Node.js](https://nodejs.org/) and [npm](https://www.npmjs.com/)
- [PostgreSQL](https://www.postgresql.org/)

## Installation

1. **Clone the repository:**

    ```sh
    git clone https://github.com/yourusername/shorturl.git
    cd shorturl
    ```

2. **Install npm dependencies:**

    ```sh
    npm install
    ```

3. **Set up the database:**

   Update the connection string in `appsettings.json`:

    ```
    "ConnectionStrings": {
        "DefaultConnection": "Host=localhost;Database=shorturl;Username=yourusername;Password=yourpassword"
    }
    ```

4. **Apply migrations:**

    ```sh
    dotnet ef database update
    ```

## Running the Application

1. **Run the application:**

    ```sh
    dotnet run
    ```

2. **Open your browser and navigate to your localhost**

## Deployment

1. **Publish the application:**

    ```sh
    dotnet publish -c Release
    ```

2. **Deploy the published files to your server.**

3. **Ensure your server has the necessary environment variables set for the connection string.**

4. **Run the application on your server.**

## License

This project is licensed under the MIT License.