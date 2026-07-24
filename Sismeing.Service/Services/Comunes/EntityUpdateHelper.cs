using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Sismeing.Service.Services.Comunes
{
    public static class EntityUpdateHelper
    {
        // SetValues copia TODOS los escalares del item recibido del cliente,
        // incluidos los campos de auditoría de registro que el cliente no envía
        // (llegan como default: FechaRegistro = 0001-01-01 Kind=Unspecified, que
        // además Npgsql rechaza en columnas timestamptz). Este método revierte
        // esos campos para que conserven su valor original.
        public static void PreservarCamposRegistro(EntityEntry entry)
        {
            string[] camposProtegidos =
            {
                "FechaRegistro", "UsuarioRegistro", "IpRegistro",
                "UsuarioEliminacion", "FechaEliminacion", "IpEliminacion",
            };

            foreach (var prop in entry.Properties)
            {
                if (camposProtegidos.Contains(prop.Metadata.Name))
                    prop.IsModified = false;
            }
        }

        // Campos que el servidor genera solo una vez (ej. NumeroInforme) y que el
        // formulario ya no envía: si el cliente manda null/vacío, se conserva el
        // valor original en lugar de borrarlo.
        public static void PreservarSiVacio(EntityEntry entry, params string[] campos)
        {
            foreach (var prop in entry.Properties)
            {
                if (!campos.Contains(prop.Metadata.Name)) continue;
                if (prop.CurrentValue is null || (prop.CurrentValue is string s && string.IsNullOrWhiteSpace(s)))
                {
                    prop.CurrentValue = prop.OriginalValue;
                    prop.IsModified = false;
                }
            }
        }

        // Las fechas que llegan del cliente en JSON (ej. "2026-06-12") se
        // deserializan con Kind=Unspecified y Npgsql no las acepta en columnas
        // timestamp with time zone. Se marcan como UTC.
        public static DateTime? AsegurarUtc(DateTime? fecha)
        {
            if (fecha == null) return null;
            return AsegurarUtc(fecha.Value);
        }

        public static DateTime AsegurarUtc(DateTime fecha)
        {
            return fecha.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(fecha, DateTimeKind.Utc)
                : fecha.ToUniversalTime();
        }
    }
}
