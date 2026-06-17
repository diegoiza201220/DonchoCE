namespace EnvioCorreos.Models
{
    public class EmailMessage
    {
        public List<string> Destinatarios { get; set; } = [];
        public List<string> ConCopia { get; set; } = [];
        public List<string> ConCopiaOculta { get; set; } = [];
        public string Asunto { get; set; } = "";
        public string Cuerpo { get; set; } = "";
        public bool EsHtml { get; set; } = true;
        public List<EmailAdjunto> Adjuntos { get; set; } = [];
    }

    public class EmailAdjunto
    {
        public string NombreArchivo { get; set; } = "";
        public byte[] Contenido { get; set; } = [];
        public string TipoMime { get; set; } = "application/octet-stream";

        // Constructor conveniente para archivos en disco
        public static EmailAdjunto DesdeDisco(string rutaArchivo)
        {
            return new EmailAdjunto
            {
                NombreArchivo = Path.GetFileName(rutaArchivo),
                Contenido = File.ReadAllBytes(rutaArchivo),
                TipoMime = "application/octet-stream"
            };
        }

        // Constructor para PDFs (útil para el RIDE de facturas)
        public static EmailAdjunto DesdePdf(string nombreArchivo, byte[] contenido)
        {
            return new EmailAdjunto
            {
                NombreArchivo = nombreArchivo,
                Contenido = contenido,
                TipoMime = "application/pdf"
            };
        }

        // Constructor para XML (útil para facturas electrónicas)
        public static EmailAdjunto DesdeXml(string nombreArchivo, string xmlContenido)
        {
            return new EmailAdjunto
            {
                NombreArchivo = nombreArchivo,
                Contenido = System.Text.Encoding.UTF8.GetBytes(xmlContenido),
                TipoMime = "application/xml"
            };
        }
    }
}
