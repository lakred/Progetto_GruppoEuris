namespace ConcordiaTrelloLibrary;

using ConcordiaTrelloLibrary.Gateways;

public static class TrelloSettings
{
    private static string KeyAD = string.Empty;
    private static string TokenAD = string.Empty;
    private static string BoardCode = string.Empty;
    private static string BoardURL = string.Empty;

    public static string GetBoardCode()
    {
        return BoardCode;
    }

    public static string GetBoardURL()
    {
        return BoardURL;
    }

    public static TrelloGateway GetBoardGateway()
    {
        return new TrelloGateway(KeyAD, TokenAD);
    }

    public static void SetKeyAD(string keyAD)
    {
        KeyAD = keyAD;
    }

    public static void SetTokenAD(string tokenAD)
    {
        TokenAD = tokenAD;
    }

    public static void SetBoardCode(string boardcode)
    {
        BoardCode = boardcode;
    }

    public static void SetBoardURL(string boardURL)
    {
        BoardURL = boardURL;
    }

    public static async Task<bool> IsBoardAccessibleAsync()
    {
        try
        {
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, BoardURL);
            var response = await httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch /* (Exception ex) */
        {
            return false;
        }
    }
}