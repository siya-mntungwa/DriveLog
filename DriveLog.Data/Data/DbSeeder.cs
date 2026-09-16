using DriveLog.Data.Enums;
using DriveLog.Data.Models;

namespace DriveLog.Data.Data;

public static class DbSeeder
{
    public static void Seed(DriveLogDbContext context)
    {
        var admin = context.Users.FirstOrDefault(u => u.EmployeeId == "ADM001");
        var driver = context.Users.FirstOrDefault(u => u.EmployeeId == "EMP001");

        if (admin == null)
        {
            admin = new User
            {
                EmployeeId = "ADM001",
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@drivelog.com",
                PasswordHash = "DemoPassword",
                Role = UserRole.Admin,
                Status = UserStatus.Active
            };

            context.Users.Add(admin);
        }

        if (driver == null)
        {
            driver = new User
            {
                EmployeeId = "EMP001",
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@drivelog.com",
                PasswordHash = "DemoPassword",
                Role = UserRole.Driver,
                Status = UserStatus.Active
            };

            context.Users.Add(driver);
            context.SaveChanges();
        }

        if (!context.Vehicles.Any())
        {
            var vehicles = new List<Vehicle>
            {
                new Vehicle
                {
                    RegistrationNumber = "ABC 123 GP",
                    Make = "Toyota",
                    Model = "Hilux",
                    Year = 2024,
                    Status = VehicleStatus.Available
                },
                
                new Vehicle
                {
                    RegistrationNumber = "DEF 456 GP",
                    Make = "Ford",
                    Model = "Ranger",
                    Year = 2023,
                    Status = VehicleStatus.InUse
                },

                new Vehicle
                {
                    RegistrationNumber = "GHI 789 GP",
                    Make = "Isuzu",
                    Model = "D-Max",
                    Year = 2022,
                    Status = VehicleStatus.Inactive
                }
            };

            context.Vehicles.AddRange(vehicles);
        }

        if (!context.DriveDocuments.Any(d => d.UserId == driver.Id))
        {
            var documents = new List<DriverDocument>
            {
                new DriverDocument
                {
                    UserId = driver.Id,
                    DocumentType = DocumentType.License,
                    FileName = "john-drivers-license.pdf",
                    IssueDate = DateTime.UtcNow.AddYears(-2),
                    ExpiryDate = DateTime.UtcNow.AddYears(3),
                    DocumentStatus = DocumentStatus.Approved,
                    IsCurrent = true
                },

                new DriverDocument
                {
                    UserId = driver.Id,
                    DocumentType = DocumentType.PrDP,
                    FileName = "john-pdp.pdf",
                    IssueDate = DateTime.UtcNow.AddYears(-1),
                    ExpiryDate = DateTime.UtcNow.AddYears(2),
                    DocumentStatus = DocumentStatus.Approved,
                    IsCurrent = true
                }
            };

            context.DriveDocuments.AddRange(documents);
        }

        context.SaveChanges();
    }
}