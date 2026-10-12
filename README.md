# Vehicle Information App

A full-stack web application built with **ASP.NET Core Web API**, **Angular**, and **Docker**. The application retrieves vehicle information using the NHTSA Vehicle Product Information Catalog (vPIC) API.

## Technologies Used

- **Backend:** ASP.NET Core Web API (.NET 8)
- **Frontend:** Angular
- **External API:** NHTSA vPIC API
- **Containerization:** Docker and Docker Compose
- **Web Server / Reverse Proxy:** Nginx
- **Deployment:** AWS EC2

## Features

- Retrieve vehicle makes from the NHTSA API.
- Search for vehicle information by make and model year.
- Retrieve available vehicle types and models based on the selected criteria.
- Run the frontend and backend together using Docker Compose.
- Access backend API endpoints through the Nginx reverse proxy.

## Prerequisites

Choose one of the following approaches to run the application.

### Option 1: Run with Docker (Recommended)

Install:

- Docker Desktop
- Git

### Option 2: Run without Docker

Install:

- .NET 8 SDK
- Node.js and npm
- Angular CLI, if required by your local Angular setup

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Hamadjafar/vehicle-information-app.git
cd vehicle-information-app
```

### 2. Start the Application with Docker

From the project root directory, run:

```bash
docker compose up --build -d
```

Docker Compose builds and starts both the backend and frontend containers.

### 3. Open the Application

Open the following URL in your browser:

```text
http://localhost
```

The frontend is served by Nginx on port `80`. Requests to `/api/` are forwarded to the backend container.

### 4. Stop the Application

To stop the containers, run:

```bash
docker compose down
```

To start the application again after it has been built, run:

```bash
docker compose up -d
```

## Running the Backend Separately

Open a terminal and navigate to the backend project:

```bash
cd VehicleApp.Api
dotnet restore
dotnet run
```

Use the URL displayed by the .NET application in the terminal.

## Running the Frontend Separately

Open another terminal and navigate to the frontend project:

```bash
cd vehicle-app
npm install
npm start
```

Open the local URL displayed by Angular in the terminal, commonly `http://localhost:4200`.

**Note:** The standalone frontend setup may require an appropriate API URL or proxy configuration depending on the backend URL and local environment. The Docker Compose setup is the recommended way to run the complete application with the included Nginx reverse proxy.

## External API

Vehicle information is retrieved from the NHTSA vPIC API:

https://vpic.nhtsa.dot.gov/api/

The availability and returned data depend on the external API.

## Deployment

The application has been deployed to AWS EC2.

- **Application URL:** http://51.20.3.223

## Project Structure

```text
vehicle-information-app/
├── VehicleApp.Api/
│   └── Dockerfile
├── vehicle-app/
│   ├── Dockerfile
│   └── nginx.conf
├── docker-compose.yml
└── README.md
```

## License

This project was created as a full-stack application assignment.