namespace ConcordiaDBLibrary;

public static class DBSettings
{
    private static string ConnectionString = string.Empty;

    public static string GetConnectionString()
    {
        return ConnectionString;
    }

    public static void SetConnectionString(string connectionstring)
    {
        ConnectionString = connectionstring;
    }
}