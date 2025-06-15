# ClassManager

Service Manager Class

## Project Overview
ClassManager is a service designed to manage classes efficiently. It includes an API service and a database service.

## Prerequisites
- Docker and Docker Compose installed on your system.
- .NET SDK 9.0 or later (required to build and run the project locally).
- A code editor like Visual Studio Code or Visual Studio (optional but recommended).
- Windows, macOS, or Linux operating system with support for Docker.

## Environment Setup
1. Install Docker and Docker Compose:
   - Follow the official [Docker installation guide](https://docs.docker.com/get-docker/).
2. Install the .NET SDK:
   - Download and install the .NET SDK 9.0 or later from the [.NET official website](https://dotnet.microsoft.com/download).
3. (Optional) Install Visual Studio Code:
   - Download and install Visual Studio Code from [here](https://code.visualstudio.com/).
   - Install the "C#" extension for better development experience.
4. Ensure your system meets the minimum requirements for running Docker and .NET applications.

## Project Structure
- `src/Api`: Contains the API service code.
- `src/Application`: Contains application logic.
- `src/Domain`: Contains domain models.
- `src/Infrastructure`: Contains infrastructure-related code.

## How to Build and Run

### Using Docker Compose
1. Ensure Docker is running on your system.
2. Navigate to the project root directory.
3. Run the following command to build and start the services:
   ```cmd
   docker-compose up --build
   ```
4. The API service will be available at:
   - HTTP: `http://localhost:8080`
   - HTTPS: `https://localhost:8081`
   - https://localhost:8081/swagger

### Stopping the Services
To stop the services, run:
```cmd
docker-compose down
```

### Local Development
1. Navigate to the `src/Api` directory.
2. Run the API project using the .NET CLI:
   ```cmd
   dotnet run
   ```
3. The API will be available at the same endpoints as above.

## Environment Variables
- `ASPNETCORE_URLS`: Configures the URLs for the API.
- `ASPNETCORE_ENVIRONMENT`: Sets the environment (e.g., Development).
- `ConnectionStrings__DefaultConnection`: Connection string for the database.
- `Kestrel__Certificates__Default__Password`: Password for the HTTPS certificate.
- `Kestrel__Certificates__Default__Path`: Path to the HTTPS certificate.

## Volumes
- `.certs`: Maps to `/https` in the container for HTTPS certificates.
- `sqldata`: Maps to `/var/opt/mssql` in the database container for persistent data storage.
