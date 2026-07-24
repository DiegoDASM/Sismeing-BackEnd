using System;
using System.Collections.Generic;

namespace Sismeing.Domain.Models.Informes
{
    /// <summary>
    /// Modelo genérico para el informe de DATOS (instalación, mantenimiento, visita).
    /// El servicio arma las secciones agregando SOLO los campos que tienen valor,
    /// de modo que la plantilla nunca muestra filas vacías.
    /// </summary>
    public class InformeDatosModel
    {
        public string Titulo { get; set; } = "INFORME";        // Ej: "INFORME DE INSTALACIÓN"
        public string NumeroInforme { get; set; } = "";          // Ej: "GP 2127" o "#12"
        public string? Estado { get; set; }                      // Ej: "ACEPTADO"
        public string? EstadoPie { get; set; }                   // Texto adicional del pie (ej: "PRÓXIMO MANTENIMIENTO: ...")
        public string? Observaciones { get; set; }

        public List<InformeSeccion> Secciones { get; set; } = new();

        // Mediciones (inicial vs final) tomadas de la tabla medicion. Null si no hay.
        public InformeMediciones? Mediciones { get; set; }

        // Evidencia fotográfica embebida también en el informe de datos.
        public List<InformeFotoGrupo> Fotos { get; set; } = new();
        public bool TieneFotos
        {
            get
            {
                foreach (var g in Fotos)
                    if (g.Urls.Count > 0) return true;
                return false;
            }
        }

        // Firmas: todo informe se imprime SIEMPRE con espacio para la firma del
        // supervisor de obra y del encargado de la empresa cliente (el nombre es opcional).
        // La hoja de vida del equipo no lleva firmas (MostrarFirmas = false).
        public bool MostrarFirmas { get; set; } = true;
        public string? SupervisorNombre { get; set; }
        public string? EncargadoNombre { get; set; }

        // Código QR (data URI PNG) que enlaza a la hoja de vida del equipo.
        public string? QrDataUri { get; set; }
        public string? QrLeyenda { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;
    }

    /// <summary>Bloque de mediciones del informe: una fila por variable, con su valor inicial y final.</summary>
    public class InformeMediciones
    {
        public bool TieneFinal { get; set; }                     // muestra la columna "Final" solo si hay datos finales
        public List<InformeMedicionFila> Filas { get; set; } = new();

        /// <summary>Agrega la fila solo si hay al menos un valor (inicial o final).</summary>
        public void Add(string etiqueta, decimal? inicial, decimal? final)
        {
            if (inicial is null && final is null) return;
            Filas.Add(new InformeMedicionFila
            {
                Etiqueta = etiqueta,
                Inicial = inicial?.ToString("0.##"),
                Final = final?.ToString("0.##")
            });
            if (final is not null) TieneFinal = true;
        }
    }

    public class InformeMedicionFila
    {
        public string Etiqueta { get; set; } = "";
        public string? Inicial { get; set; }
        public string? Final { get; set; }
    }

    public class InformeSeccion
    {
        public string Titulo { get; set; } = "";
        public bool AnchoCompleto { get; set; }                  // true => la tarjeta ocupa todo el ancho
        public List<InformeCampo> Campos { get; set; } = new();

        /// <summary>Agrega el campo solo si el valor no es nulo/vacío. Devuelve la propia sección (encadenable).</summary>
        public InformeSeccion Add(string etiqueta, object? valor)
        {
            var texto = valor switch
            {
                null => null,
                DateTime d => d.ToString("yyyy-MM-dd HH:mm"),
                bool b => b ? "Sí" : "No",
                decimal dec => dec.ToString("0.##"),
                _ => valor.ToString()
            };

            if (!string.IsNullOrWhiteSpace(texto))
                Campos.Add(new InformeCampo { Etiqueta = etiqueta, Valor = texto! });

            return this;
        }
    }

    public class InformeCampo
    {
        public string Etiqueta { get; set; } = "";
        public string Valor { get; set; } = "";
    }

    /// <summary>
    /// Cabecera del formato oficial de SISMEING: ficha del equipo en dos columnas
    /// (EQUIPO / N° SERIE, MARCA / MODELO, ÁREA / CAPACIDAD, CÓDIGO / POTENCIA).
    /// La descripción del trabajo y el número de informe salen del propio modelo.
    ///
    /// Los campos se rellenan en el orden en que se agregan y la plantilla los
    /// reparte de dos en dos por fila, así que el orden importa.
    /// </summary>
    public class InformeCabecera
    {
        public List<InformeCampo> Campos { get; set; } = new();

        public bool TieneCampos => Campos.Count > 0;

        /// <summary>Agrega el campo solo si tiene valor. Devuelve la cabecera (encadenable).</summary>
        public InformeCabecera Add(string etiqueta, string? valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
                Campos.Add(new InformeCampo { Etiqueta = etiqueta, Valor = valor! });
            return this;
        }
    }

    /// <summary>
    /// Modelo genérico para el informe FOTOGRÁFICO (mismo para los tres módulos).
    /// Se llena con las fotos subidas (agrupadas por tipo) y las descripciones/observaciones del registro.
    /// </summary>
    public class InformeFotograficoModel
    {
        public string Titulo { get; set; } = "INFORME FOTOGRÁFICO";
        public string NumeroInforme { get; set; } = "";
        public string? Descripcion { get; set; }                 // Derivada del trabajo/tipo realizado
        public string? Observaciones { get; set; }

        // Ficha del equipo que encabeza la hoja (formato oficial).
        public InformeCabecera Cabecera { get; set; } = new();

        public List<InformeFotoGrupo> Grupos { get; set; } = new();

        // Formato oficial: filas foto inicial | descripción | foto final.
        // Si hay filas, la plantilla las usa en lugar de los grupos.
        public List<InformeFotoFila> Filas { get; set; } = new();

        // Firmas fijas (ver InformeDatosModel).
        public string? SupervisorNombre { get; set; }
        public string? EncargadoNombre { get; set; }

        public DateTime FechaGeneracion { get; set; } = DateTime.Now;

        public bool TieneFilas => Filas.Count > 0;

        public bool TieneFotos
        {
            get
            {
                if (TieneFilas) return true;
                foreach (var g in Grupos)
                    if (g.Urls.Count > 0) return true;
                return false;
            }
        }
    }

    public class InformeFotoGrupo
    {
        public string Titulo { get; set; } = "";                 // Ej: "Imagen Inicial" / "Imagen Final"
        public List<string> Urls { get; set; } = new();
    }

    /// <summary>
    /// Fila del informe fotográfico con el formato oficial de SISMEING:
    /// IMAGEN INICIAL | DESCRIPCIÓN (trabajo realizado) | IMAGEN FINAL.
    /// Las fotos se emparejan por orden de subida con el trabajo correspondiente.
    /// </summary>
    public class InformeFotoFila
    {
        public string? UrlInicial { get; set; }
        public string? Descripcion { get; set; }
        public string? UrlFinal { get; set; }
    }
}
