namespace RzumeAPI.Models.Requests;


public class SendConfirmEmailProps
{
    public required  User User  { get; set; }
    public required string Token { get; set; }
    public required string MailPurpose { get; set; }
    public  bool IsSigin { get; set; } = false;
    public required string LinkPath { get; set; }
    public required string TemplatePath { get; set; }
    public required string TemplateName { get; set; }
    public required string Subject { get; set; }



}