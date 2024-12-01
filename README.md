# Room Reservation System

A .NET Core Web API for managing room reservations across different buildings. This system allows users to book rooms, manage reservations, and handle user authentication.

## Features

- User Authentication with JWT
- Role-based Authorization
- Room Management
- Building Management
- Reservation System
- Oracle Database Integration

## Prerequisites

- .NET 7.0 or higher
- Oracle Database
- Visual Studio 2022 or VS Code

## Installation

1. Clone the repository
```bash
git clone https://github.com/yourusername/room-reservation-system.git
```

2. Create environment file
```bash
cp .env.example .env
```
Then update the `.env` file with your actual configuration values.

3. Install dependencies
```bash
dotnet restore
```

4. Run database migrations
```bash
# Execute the SQL script from new_sqript.txt in your Oracle database
```

5. Start the application
```bash
dotnet run
```

## API Endpoints

### Authentication
- POST `/api/auth/login`
  - Login with username and password
  - Returns JWT token
- POST `/api/auth/register`
  - Register new user

### Users
- GET `/api/users`
  - Get all users (Admin only)
- GET `/api/users/{id}`
  - Get user by ID
- PUT `/api/users/{id}`
  - Update user information

### Buildings
- GET `/api/buildings`
  - Get all buildings
- POST `/api/buildings`
  - Add new building (Admin only)
- PUT `/api/buildings/{id}`
  - Update building information
- DELETE `/api/buildings/{id}`
  - Delete building

### Rooms
- GET `/api/rooms`
  - Get all rooms
- GET `/api/rooms/{id}`
  - Get room by ID
- POST `/api/rooms`
  - Add new room
- PUT `/api/rooms/{id}`
  - Update room information
- DELETE `/api/rooms/{id}`
  - Delete room

### Reservations
- GET `/api/reservations`
  - Get all reservations
- POST `/api/reservations`
  - Create new reservation
- PUT `/api/reservations/{id}`
  - Update reservation
- DELETE `/api/reservations/{id}`
  - Cancel reservation

## Request/Response Examples

### Login Request
```json
POST /api/auth/login
{
    "username": "user@example.com",
    "password": "password123"
}
```

### Login Response
```json
{
    "token": "eyJhbGciOiJIUzI1NiIs...",
    "userId": 1,
    "username": "user@example.com",
    "role": "User"
}
```

### Create Reservation Request
```json
POST /api/reservations
{
    "roomId": 1,
    "startTime": "2024-02-20T09:00:00",
    "endTime": "2024-02-20T10:00:00",
    "description": "Team Meeting"
}
```

## Environment Variables

The following environment variables need to be set in the `.env` file:

```
JWT_SECRET_KEY=your_jwt_secret_key_here
DB_CONNECTION_STRING=Data Source=your_server;User Id=your_username;Password=your_password;
```

## Authentication

The API uses JWT (JSON Web Tokens) for authentication. Include the token in the Authorization header:
```
Authorization: Bearer your_jwt_token_here
```

## Error Handling

The API returns standard HTTP status codes:

- 200: Success
- 400: Bad Request
- 401: Unauthorized
- 403: Forbidden
- 404: Not Found
- 500: Internal Server Error

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details
