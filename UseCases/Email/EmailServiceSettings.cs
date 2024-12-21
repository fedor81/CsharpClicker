namespace CSharpClicker.Web.UseCases.Email
{
    public class EmailServiceSettings
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpLogin { get; set; }
        public string SmtpPassword { get; set; }
    }
}
