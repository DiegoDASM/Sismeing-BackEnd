namespace Sismeing.Domain.Models.Emails
{
    public class WelcomeModel
    {
        public string UserName { get; set; } = string.Empty;
        public string SetPasswordUrl { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
