namespace Blog.JwtConfiguration;
public static class Configuration
{
    public static string JwtKey = "CASA-PAPEL-9320843204093$$$9999332093209dskjioasue32";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "curso_api_IlT_unwg";
    public static SmtpConfig Smtp = new SmtpConfig();

    public class SmtpConfig
    {
        public string Host { get; set; }
        public int Port { get; set; } = 465;
        public string UserName { get; set; }
        public string Password { get; set; }

    }

}
