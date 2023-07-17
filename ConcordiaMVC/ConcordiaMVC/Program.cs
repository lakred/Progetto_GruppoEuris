using ConcordiaDBLibrary;
using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Abstract;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using Microsoft.EntityFrameworkCore;
using ConcordiaTrelloLibrary;
using ConcordiaUtilsLibrary;
using ConcordiaMVC;

// set the connection string
DBSettings.SetConnectionString("Server=localhost;Database=Concordia;Integrated Security=true;TrustServerCertificate=True;");

// set the board and the administrator
TrelloSettings.SetBoardCode("6475b8a930419d4c8db7b32b");
TrelloSettings.SetBoardURL("https://trello.com/b/B3sftp9b/squirtleconcordia");
TrelloSettings.SetKeyAD("38df19edb21f2fb0be38069d18a7521a");
TrelloSettings.SetTokenAD("ATTA22ee09a927859ecc9335eeb04c14b1bda2580c25378c887615e3c3a02cd324da6CE9AFB3");

// set the email to send the report and the eamil to receive the report 
UtilsSettings.SetFromEmail("concordiasquirtle@outlook.it");
UtilsSettings.SetFromPassword("HNpqxTfDDKU4");
UtilsSettings.SetToEmail("concordiasquirtle@outlook.it");
UtilsSettings.SetHost("smtp-mail.outlook.com");
UtilsSettings.SetPort(587);

// the app can begin...
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ConcordiaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(DBSettings.GetConnectionString())));

builder.Services.AddScoped<IEntityGateway<Experiment>, ExperimentsGateway>();
builder.Services.AddScoped<IEntityGateway<Participant>, ParticipantsGateway>();
builder.Services.AddScoped<IEntityGateway<Priority>, PrioritiesGateway>();
builder.Services.AddScoped<IEntityGateway<Remark>, RemarksGateway>();
builder.Services.AddScoped<IEntityGateway<Scientist>, ScientistsGateway>();
builder.Services.AddScoped<IEntityGateway<State>, StatesGateway>();

builder.Services.AddSingleton<SynchronizationBackgroundService>();
builder.Services.AddHostedService(provider => provider.GetService<SynchronizationBackgroundService>());

var app = builder.Build();

await app.MigrateAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. 
    // You may want to change this for production scenarios, 
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();