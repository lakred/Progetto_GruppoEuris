namespace ConcordiaDBLibrary.Models.Extensions;

using System.Text.RegularExpressions;
using Abstract;

public static class TrelloEntityExtension
{
    public static bool IsCode(this TrelloEntity entity)
    {
        var regex = @"^[0-9a-zA-Z]{24}$";
        return entity.Code is null? false : Regex.Match(entity.Code, regex).Success;
    }
}