namespace RzumeAPI.Models.Utilities;

public static class UserStatMsg
{
    public const string UserExistsMsg = "User already exists";
    public const string EmailNotConfirmedMsg = "Kindly validate mail";

    public const string EmailValidated = "User mail already validated";

    public const string NotFound = "User not found";
    public const string Found = "User found";

    public const string InvalidDetails = "Username or password is incorrect";
    public const string LockedOut = "Too many login attempts, retry after 5 minutes";
    public const string AccountActivated = "Account activated";

}