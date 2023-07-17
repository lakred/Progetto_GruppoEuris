namespace ConcordiaMVC.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SynchronizationBackgroundService _synchronizationBackgroundService;

    public HomeController(ILogger<HomeController> logger, SynchronizationBackgroundService synchronizationBackgroundService)
    {
        _logger = logger;
        _synchronizationBackgroundService = synchronizationBackgroundService;
    }

    public IActionResult Synchronization()
    {
        return View();
    }

    public JsonResult IsSynchronizing()
    {
        var syncService = HttpContext.RequestServices.GetService<SynchronizationBackgroundService>();
        return Json(new { isSynchronizing = syncService.IsSynchronizing });
    }

    public IActionResult About()
    {
        _logger.LogInformation($"{nameof(HomeController)}.{nameof(HomeController.About)} was called.");
        return View();
    }

    public IActionResult Index()
    {
        _logger.LogInformation($"{nameof(HomeController)}.{nameof(HomeController.Index)} was called.");
        return View();
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation($"{nameof(HomeController)}.{nameof(HomeController.Privacy)} was called.");
        return View();
    }

    public IActionResult GoToTasks()
    {
        _logger.LogInformation($"{nameof(HomeController)}.{nameof(HomeController.GoToTasks)} was called.");
        _logger.LogInformation($"Redirect to {nameof(TasksController)}.{nameof(TasksController.Index)}");
        return RedirectToAction("Index", "Tasks");
    }

    public IActionResult GoToUsers()
    {
        _logger.LogInformation($"{nameof(HomeController)}.{nameof(HomeController.GoToUsers)} was called.");
        _logger.LogInformation($"Redirect to {nameof(UsersController)}.{nameof(UsersController.Index)}");
        return RedirectToAction("Index", "Users");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var model = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
        return View("Error", model);
    }
}