namespace DriveLog.Web.Services;

public class AuthService
{
    public LoginResponse? CurrentUser { get; private set; }

    public bool IsLoggedIn => CurrentUser != null;

    public bool IsAdmin => CurrentUser?.Role == "Admin";

    public bool IsDriver => CurrentUser?.Role == "Driver";

    public void Login(LoginResponse user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}