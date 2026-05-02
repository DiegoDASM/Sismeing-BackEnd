namespace Sismeing.Domain.Models.Emails
{
    public class MaintenanceReminderModel
    {
        public string EmpresaCliente { get; set; } = string.Empty;
        public string TipoSistema { get; set; } = string.Empty;
        public List<EquipoInfo> Equipos { get; set; } = new();
    }

    public class EquipoInfo
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime ProximaFecha { get; set; }
    }
}
