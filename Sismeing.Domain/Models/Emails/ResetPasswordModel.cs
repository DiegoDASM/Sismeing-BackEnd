namespace Sismeing.Domain.Models.Emails
{
    public class ResetPasswordModel
    {
        public string UserName { get; set; } = string.Empty;
        public string ResetUrl { get; set; } = string.Empty;
    }
}
