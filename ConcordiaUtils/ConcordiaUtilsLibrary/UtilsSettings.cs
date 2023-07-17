namespace ConcordiaUtilsLibrary;

using Reporters;

public static class UtilsSettings
{
    private static string FromEmail = string.Empty;
    private static string FromPassword = string.Empty;
    private static string ToEmail = string.Empty;
    private static string Host = string.Empty;
    private static int Port = 587;

    public static ReportSender GetReportSender()
    {
        return new ReportSender(FromEmail, FromPassword, ToEmail, Host, Port);
    }

    public static void SetFromEmail(string email)
    {
        FromEmail = email;
    }

    public static void SetFromPassword(string password)
    {
        FromPassword = password;
    }

    public static void SetToEmail(string email)
    {
        ToEmail = email;
    }

    public static void SetHost(string host)
    {
        Host = host;
    }

    public static void SetPort(int port)
    {
        Port = port;
    }
}