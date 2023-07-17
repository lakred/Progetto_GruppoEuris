namespace ConcordiaMVC;

using Microsoft.EntityFrameworkCore;
using ConcordiaDBLibrary.Data;

public static class Bootstrapper
{
    public static async Task MigrateAsync(this WebApplication app)
    {
        using var provider = app.Services.CreateScope();
        var context = provider.ServiceProvider.GetRequiredService<ConcordiaContext>();
        await context.Database.MigrateAsync();
    }
}