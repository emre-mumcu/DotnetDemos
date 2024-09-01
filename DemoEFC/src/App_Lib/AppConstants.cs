namespace src.App_Lib;

public static class AppConstants
{
    // COMMON
    public const string App_Culture = "tr-TR";

    // SESSION VARIABLES       
    public const string Session_Cookie_Name = "app.cookie.session";
    public const int Session_IdleTimeout = 20;

    // SESSION KEYS
    public const string SessionKeyLogin = "LOGIN";
    public const string SessionKeyCaptcha = "CAPTCHA";
    public const string SessionKeyLoginUser = "LOGINUSER";
    public const string SessionKey_SelectedRole = "SELECTED_ROLE";
    public const string SessionKey_System_Message = "SYSTEM_MESSAGE";

    // AUTHENTICATION VARIABLES
    public const string Auth_Cookie_Name = "app.cookie.authentication";
    public const string Auth_Cookie_LoginPath = "/Login";
    public const string Auth_Cookie_LogoutPath = "/Logout";
    public const string Auth_Cookie_AccessDeniedPath = "/Forbidden";
    public const string Auth_Cookie_ClaimsIssuer = "app.cookie.issuer";
    public const string Auth_Cookie_ReturnUrlParameter = "AppReturn";
    public const int Auth_Cookie_ExpireTimeSpan = 20;
}