# SkyBooker Platform

SkyBooker is a modern, scalable, and fully-featured flight booking platform. It provides a seamless end-to-end experience for users to search for flights, select seats, manage passengers, and finalize bookings. For administrators, it includes a comprehensive dashboard to manage flights, bookings, users, and seating configurations.

The platform is built using a **React** frontend and a robust **.NET Microservices** backend, utilizing an API Gateway pattern for efficient routing and a containerized architecture deployed on **Render**.

---

## ✨ Key Features

### For Users
* **User Authentication:** Secure registration and login using JWT.
* **Flight Discovery:** Browse and search for available flights and popular destinations.
* **Multi-step Booking Flow:** Streamlined process for entering passenger details, selecting seats, and confirming bookings.
* **Seat Selection:** Interactive UI to choose available seats for specific flights.
* **Booking Management:** View past and upcoming flight bookings.

### For Administrators
* **Role-Based Access Control:** Secure admin panel restricted to authorized personnel.
* **Flight Management:** Add, update, or remove flight schedules and details.
* **Seat Configuration:** Manage seating arrangements, pricing, and availability.
* **Booking Control:** Full CRUD capabilities to manage customer bookings, edit passenger details, or perform cancellations.
* **User Management:** Dashboard to view and manage registered platform users.

---

## 🏗️Architecture & Tech Stack

SkyBooker is built on a microservices architecture to ensure high availability, scalability, and maintainability.

### Frontend
* **Framework:** React (Vite)
* **Styling:** CSS/TailwindCSS
* **State Management & Routing:** React Router, Context API
* **HTTP Client:** Axios (configured to route through API Gateway)

### Backend (Microservices)
* **Framework:** .NET 8 (C#)
* **API Gateway:** Ocelot / YARP (Yet Another Reverse Proxy)
* **Database:** PostgreSQL (via Render Managed Database / Local Docker)
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Tokens)
* **Code Quality Analysis:** SonarQube integration

#### Services Breakdown:
1. **ApiGateway:** The single entry point for all frontend requests, routing traffic to the appropriate microservice.
2. **AuthService:** Handles user registration, login, JWT generation, and role validation.
3. **FlightService:** Manages flight data, schedules, and destinations.
4. **SeatService:** Handles seat layouts, pricing, and real-time availability.
5. **PassengerService:** Manages passenger information associated with bookings.
6. **BookingService:** Orchestrates the booking process, tying together flights, passengers, and seats.
7. **NotificationService:** Handles user alerts, booking confirmations, and travel updates.

---

##  Getting Started (Local Development)

### Prerequisites
* [Node.js](https://nodejs.org/) (v18+)
* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local PostgreSQL and SonarQube)

### 1. Database Setup
You can either use a remote PostgreSQL instance (like Render) or run it locally using Docker:
```bash
docker run --name skybooker-db -e POSTGRES_PASSWORD=yourpassword -p 5432:5432 -d postgres
