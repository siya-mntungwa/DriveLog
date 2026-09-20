using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;

namespace DriveLog.Web.Services;

public class DriveLogApiService
{
    private readonly HttpClient _httpClient;

    public DriveLogApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DriveLogApi");
    }

    public async Task<List<DriverDto>> GetDriversAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<DriverDto>>("api/Drivers") ?? new List<DriverDto>();
    }

    public async Task<List<VehicleDto>> GetVehiclesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<VehicleDto>>("api/Vehicles") ?? new List<VehicleDto>();
    }

    public async Task<DriveLogDto?> StartDriveAsync(StartDriveRequest request)
    {
        var url = "api/DriveLogs/start";

        Console.WriteLine($"POST URL: {_httpClient.BaseAddress}{url}");

        Console.WriteLine($"UserId: {request.UserId}, VehicleId: {request.VehicleId}");
        var response = await _httpClient.PostAsJsonAsync(url, request);

        Console.WriteLine($"Response status: {response.StatusCode}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response body: {error}");

            return null;
        }

        return await response.Content.ReadFromJsonAsync<DriveLogDto>();
    }

    public async Task<List<DriveLogDto>> GetDriveLogsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<DriveLogDto>>("api/DriveLogs") ?? new List<DriveLogDto>();
    }

    public async Task<DriveLogDto?> GetActiveDriveAsync(int userId)
    {
        var driveLogs = await GetDriveLogsAsync();

        return driveLogs.FirstOrDefault(d =>
            d.UserId == userId && d.EndTime == null);
    }

    public async Task<DriveLogDto?> EndDriveAsync(int driveId, string endLocation)
    {
        var request = new
        {
            EndLocation = endLocation
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"api/DriveLogs/end/{driveId}",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<DriveLogDto>();
    }

    public async Task<DriverEligibilityDto?> GetDriverEligibilityAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<DriverEligibilityDto>(
            $"api/Drivers/{userId}/eligibility");
    }

    public async Task<List<DriverDocumentDto>> GetDriverDocumentsAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<DriverDocumentDto>>(
            $"api/DriverDocuments/{userId}") ?? new List<DriverDocumentDto>();
    }

    public async Task<bool> ApproveDocumentAsync(int documentId)
    {
        var response = await _httpClient.PutAsync(
            $"api/DriverDocuments/{documentId}/approve",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RejectDocumentAsync(int documentId)
    {
        var response = await _httpClient.PutAsync(
            $"api/DriverDocuments/{documentId}/reject",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<LoginResponse?> LoginAsync(string employeeId, string password)
    {
        var request = new LoginRequest
        {
            EmployeeId = employeeId,
            Password = password
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/Login",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoginResponse>();
    }

    public async Task<bool> UploadDriverDocumentAsync(
        int userId,
        IBrowserFile file,
        string documentType,
        DateTime issueDate,
        DateTime expiryDate)
    {
        using var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(
            file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(file.ContentType);

        content.Add(
            fileContent,
            "File",
            file.Name);

        content.Add(
            new StringContent(documentType),
            "DocumentType");

        content.Add(
            new StringContent(issueDate.ToString("yyyy-MM-dd")),
            "IssueDate");

        content.Add(
            new StringContent(expiryDate.ToString("yyyy-MM-dd")),
            "ExpiryDate");

        var response = await _httpClient.PostAsync(
            $"api/DriverDocuments/{userId}/upload",
            content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateVehicleAsync(CreateVehicleRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/Vehicles",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeactivateVehicleAsync(int vehicleId)
    {
        var response = await _httpClient.PutAsync(
            $"api/Vehicles/{vehicleId}/deactivate",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ReactivateVehicleAsync(int vehicleId)
    {
        var response = await _httpClient.PutAsync(
            $"api/Vehicles/{vehicleId}/reactivate",
            null);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateVehicleAsync(
        int vehicleId,
        UpdateVehicleRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync(
            $"api/Vehicles/{vehicleId}",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CreateUserAsync(CreateUserRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/Users",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>(
            "api/Users") ?? new List<UserDto>();
    }
}

public class DriverDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class VehicleDto
{
    public int Id { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Status { get; set; }
}

public class StartDriveRequest
{
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public string StartLocation { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
}

public class DriveLogDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string StartLocation { get; set; } = string.Empty;
    public string? EndLocation { get; set; }
    public string Purpose { get; set; } = string.Empty;
}

public class DriverEligibilityDto
{
    public int UserId { get; set; }
    public bool IsEligible { get; set; }
    public bool HasValidDriversLicense { get; set; }
    public bool HasValidPDP { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class DriverDocumentDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string DocumentStatus { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}

public class LoginRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public int UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class CreateVehicleRequest
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}

public class UpdateVehicleRequest
{
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}

public class CreateUserRequest
{
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Role { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
