namespace EnvioCorreos.Configuration
{
    public class EmailOptions
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string Usuario { get; set; } = "";
        public string Password { get; set; } = "";
        public string NombreRemitente { get; set; } = "";
        public bool UsarSsl { get; set; } = true;
    }
}
