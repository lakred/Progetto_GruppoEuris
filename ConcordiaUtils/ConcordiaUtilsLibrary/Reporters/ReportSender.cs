namespace ConcordiaUtilsLibrary.Reporters;

using System.Net.Mail;
using System.Net;

public class ReportSender
{
    private readonly string _fromEmail;
    private readonly string _fromPassword;
    private readonly string _toEmail;
    private readonly string _host;
    private readonly int _port;

    public ReportSender(string fromEmail, string fromPassword,
                  string toEmail, string host, int port)
    {
        _fromEmail = fromEmail;
        _fromPassword = fromPassword;
        _toEmail = toEmail;
        _host = host;
        _port = port;
    }

    public async Task SendReportAsync(string report)
    {
        try
        {
            using var smtpClient = new SmtpClient(_host, _port);
            using var mailMessage = new MailMessage(_fromEmail, _toEmail, "Report Task", "Allegato: Report Task");
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
            smtpClient.EnableSsl = true;
            var reportFilePath = report;
            mailMessage.Attachments.Add(new Attachment(reportFilePath));
            await smtpClient.SendMailAsync(mailMessage);
            Console.WriteLine("Email inviata con successo!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Errore durante l'invio dell'email: " + ex.Message);
        }
    }
}