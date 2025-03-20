# Car Rental Web Application

A modern web application for car rental management built with ASP.NET Core MVC.

## Prerequisites

Before running this application, make sure you have the following installed:

1. **.NET SDK 9.0 or later**
   - Download from: https://dotnet.microsoft.com/download
   - Verify installation: `dotnet --version`

2. **SQLite** (included in .NET SDK)
   - No separate installation needed
   - Database file will be created automatically

3. **Visual Studio Code** (recommended) or Visual Studio 2022
   - Download VS Code: https://code.visualstudio.com/
   - Required Extensions:
     - C# Dev Kit
     - SQLite Viewer (optional)

## Getting Started

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/car-rental-web.git
   cd car-rental-web
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply Database Migrations**
   ```bash
   cd CarRentalWeb
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   - Open browser and navigate to: https://localhost:5117
   - If port 5117 is in use, check the console output for the correct URL

## Default Admin Account

Use these credentials to login as administrator:
- Email: admin@carrent.com
- Password: Admin@123

## Features

- **User Management**
  - User registration and authentication
  - Role-based authorization (Admin/User)

- **Car Management** (Admin Only)
  - Add, edit, and delete cars
  - Upload car images
  - Set daily rates and availability

- **Booking System**
  - View available cars
  - Make reservations
  - Cancel bookings
  - View booking history

- **Admin Features**
  - Manage car inventory
  - View all bookings
  - Monitor system activity

## Project Structure

```
CarRentalWeb/
├── Areas/
│   └── Identity/          # Authentication views
├── Controllers/           # MVC Controllers
├── Data/                  # Database context and migrations
├── Models/               # Domain models
├── Views/                # MVC Views
├── wwwroot/             # Static files (CSS, JS, images)
└── Program.cs           # Application entry point
```

## Development Tips

1. **Database Management**
   - View database: Use SQLite viewer in VS Code
   - Reset database:
     ```bash
     rm app.db
     dotnet ef database update
     ```

2. **Adding Sample Data**
   - Login as admin
   - Click "Add Sample Cars" button
   - Sample cars with images will be added

3. **Common Issues**
   - If database errors occur, try resetting the database
   - For image upload issues, ensure the wwwroot/images folder exists
   - Check file permissions if running on Linux/Mac

## Testing

1. **User Flow**
   - Register a new account
   - Browse available cars
   - Make a booking
   - View booking details
   - Cancel booking (if upcoming)

2. **Admin Flow**
   - Login as admin
   - Add/Edit cars
   - Upload car images
   - View all bookings

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
