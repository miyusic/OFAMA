namespace OFAMA.Service
{
    public class SendMailParams
    {
        public string? MailServer { get; set; }
        public int Port { get; set; }
        public string? User {  get; set; }
        public string? Password { get; set; }
        public string? SendAddress { get; set; }

    }
}
