# Room Reservation System

Room Reservation System is a university .NET project created for the course **Programování v C# II** at **VŠB-TUO**.

The application allows users to register, log in, browse available rooms, create and manage reservations, view reservation history, and analyze room usage statistics.  
The project demonstrates a multi-layered .NET application using **ASP.NET Core MVC**, **Dapper**, **SQLite**, and a **WPF desktop client**.

## Technologies

- C#
- .NET 9
- ASP.NET Core MVC
- ASP.NET Core Controllers
- WPF
- Dapper
- SQLite
- Razor Views
- Session-based authentication
- Custom API token middleware

## Main Features

- User registration and login
- Room listing and filtering
- Room reservation creation
- Reservation validation
- Reservation collision detection
- Reservation editing and cancellation
- Reservation history
- Room usage statistics
- WPF desktop client connected to API endpoints
- SQLite database initialization with demo data

## Architecture

The solution is divided into several projects:

```text
RoomReservationSystem/
├── RoomReservation.Web
├── RoomReservation.Desktop
├── RoomReservation.Data
├── RoomReservation.Domain
├── RoomReservation.Common
└── RoomReservation.API
```

### Project Responsibilities

| Project | Responsibility |
|---|---|
| `RoomReservation.Web` | ASP.NET Core MVC web application, controllers, views, authentication and API endpoints |
| `RoomReservation.Desktop` | WPF desktop client connected to the web/API layer |
| `RoomReservation.Data` | Database access, repositories and database initialization |
| `RoomReservation.Domain` | Domain models and business entities |
| `RoomReservation.Common` | Shared DTOs and common classes |
| `RoomReservation.API` | Separate API project structure prepared for future extension |

## Business Rules

The application validates several reservation rules:

- Reservations can only be created for future time.
- End time must be later than start time.
- Number of people cannot exceed room capacity.
- Reservation duration cannot exceed the room limit.
- Overlapping reservations for the same room are not allowed.

## Database

The project uses **SQLite** as a local database and **Dapper** for data access.

The database is initialized automatically on application startup.  
Local database files are not intended to be committed to GitHub.

## API Access

The project uses a simple custom API token middleware for demonstration purposes.

API requests require the following header:

```http
X-Api-Token: YOUR_LOCAL_DEVELOPMENT_TOKEN
```

For local development, configure the token in `appsettings.Development.json`, user secrets, or environment variables.

> Note: The API token middleware is a simplified academic implementation and is not intended to replace production-grade authentication such as JWT or OAuth.

## How to Run

### Prerequisites

- .NET SDK
- Visual Studio or JetBrains Rider
- SQLite-compatible environment

### Run the Web Application

1. Clone the repository:

```bash
git clone https://github.com/moritomori/RoomReservationSystem.git
```

2. Open the solution in Visual Studio or Rider.

3. Restore NuGet packages.

4. Run the web application:

```bash
dotnet run --project RoomReservation.Web
```

5. The SQLite database will be initialized automatically on first run.

## Suggested Screenshots

For portfolio presentation, the following screenshots are recommended:

- Home page
- Login / registration page
- Room list
- Reservation creation form
- Reservation history
- Room usage statistics
- WPF desktop client
- Database tables in SQLite browser

## Portfolio Note

This is an academic project prepared for portfolio purposes.

The main goal of the project is to demonstrate practical experience with:

- C# and .NET
- ASP.NET Core MVC
- Dapper and SQL
- SQLite database access
- layered application structure
- WPF desktop development
- business logic validation
- API communication

## Possible Improvements

- Replace simplified API token authentication with JWT
- Add automated tests
- Add Docker support
- Improve error handling and logging
- Add Entity Framework Core alternative implementation
- Improve UI styling
- Add CI workflow with GitHub Actions
- Move API endpoints into the separate `RoomReservation.API` project
