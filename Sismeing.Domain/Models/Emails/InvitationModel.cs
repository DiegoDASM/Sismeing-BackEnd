namespace Sismeing.Domain.Models.Emails
{
    // Correo de invitación para que un encargado complete su registro.
    public class InvitationModel
    {
        public string Email { get; set; } = string.Empty;
        public string EmpresaNombre { get; set; } = string.Empty;
        public string CompletarRegistroUrl { get; set; } = string.Empty;
    }
}
