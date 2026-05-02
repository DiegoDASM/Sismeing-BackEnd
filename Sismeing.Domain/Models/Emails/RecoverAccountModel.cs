namespace Sismeing.Domain.Models.Emails
{
    public class RecoverAccountModel
    {
        public string Username { get; set; } = string.Empty;
        public string LoginUrl { get; set; } = string.Empty;
    }
}
