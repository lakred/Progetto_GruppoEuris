namespace ConcordiaMVC.Controllers;

using Microsoft.AspNetCore.Mvc;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary.Models.Extensions;
using ConcordiaDBLibrary.Gateways.Abstract;
using Models;


public class UsersController : Controller
{
    private readonly IEntityGateway<Experiment> _experimentsGway;
    private readonly IEntityGateway<Scientist> _scientistsGway;
    private readonly IEntityGateway<Participant> _participantsGway;

    private readonly ILogger<UsersController> _logger;

    public UsersController(IEntityGateway<Experiment> experimentsGway,
                           IEntityGateway<Scientist> scientistsGway,
                           IEntityGateway<Participant> participantsGway,
                           ILogger<UsersController> logger)
    {
        _experimentsGway = experimentsGway;
        _scientistsGway = scientistsGway;
        _participantsGway = participantsGway;
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation($"{nameof(UsersController)}.{nameof(UsersController.Index)} was called.");
        var scientists = _scientistsGway.GetAll().ToList();
        if (scientists is null)
        {
            return View(new UsersList());
        }
        return View(new UsersList(scientists));
    }

    public IActionResult DetailSingle(int id)
    {
        _logger.LogInformation($"{nameof(UsersController)}.{nameof(UsersController.DetailSingle)} was called.");
        var scientist = _scientistsGway.GetById(id);
        if (scientist is null)
        {
            return View(new UserSingleList(scientist));
        }
        var partecipants = _participantsGway.GetAll().Where(p => p.ScientistId == scientist.Id).ToList();
        if (partecipants is null)
        {
            return View(new UserSingleList(scientist));
        }
        var experimentIds = partecipants.Select(p => p.ExperimentId);
        if (experimentIds is null)
        {
            return View(new UserSingleList(scientist));
        }
        var experimentsByScientist = _experimentsGway.GetByIdMulti(experimentIds.ToList());
        if (experimentsByScientist is null)
        {
            return View(new UserSingleList(scientist));
        }
        var experimentByScientistWait = experimentsByScientist.Where(e => !(e.State!.Name.Equals("Finish", StringComparison.OrdinalIgnoreCase))).ToList();
        if (experimentByScientistWait is null)
        {
            return View(new UserSingleList(scientist));
        }
        return View(new UserSingleList(scientist, experimentByScientistWait.OrderBy(e => e.DueDate).OrderBy(e => e.Ordering()).ToList()));
    }

    public IActionResult DetailMulti(int id)
    {
        _logger.LogInformation($"{nameof(UsersController)}.{nameof(UsersController.DetailMulti)} was called.");
        var scientist = _scientistsGway.GetById(id);
        if (scientist is null)
        {
            return View(new UserMultiList(scientist));
        }
        var partecipants = _participantsGway.GetAll().Where(p => p.ScientistId == scientist.Id).ToList();
        if (partecipants is null)
        {
            return View(new UserMultiList(scientist));
        }
        var experimentIds = partecipants.Select(p => p.ExperimentId).ToList();
        if (experimentIds is null)
        {
            return View(new UserMultiList(scientist));
        }
        var experimentsByScientist = _experimentsGway.GetByIdMulti(experimentIds.ToList());
        if (experimentsByScientist is null)
        {
            return View(new UserMultiList(scientist));
        }
        var experimentsByScientistList = experimentsByScientist.ToList();
        var expsByScInS = experimentsByScientistList.Where(e => e.State!.Name.Equals("Start", StringComparison.OrdinalIgnoreCase)).ToList() ?? new List<Experiment>();
        var expsByScInW = experimentsByScientistList.Where(e => e.State!.Name.Equals("Working", StringComparison.OrdinalIgnoreCase)).ToList() ?? new List<Experiment>();
        var expsByScInF = experimentsByScientistList.Where(e => e.State!.Name.Equals("Finish", StringComparison.OrdinalIgnoreCase)).ToList() ?? new List<Experiment>();
        return View(new UserMultiList(scientist,
                                      expsByScInS.OrderBy(e => e.DueDate).OrderBy(e => e.OrderingByPriority()).ToList(),
                                      expsByScInW.OrderBy(e => e.DueDate).OrderBy(e => e.OrderingByPriority()).ToList(),
                                      expsByScInF.OrderBy(e => e.DueDate).OrderBy(e => e.OrderingByPriority()).ToList()));

    }

    public IActionResult DetailTask(int id)
    {
        _logger.LogInformation($"{nameof(UsersController)}.{nameof(UsersController.DetailTask)} was called.");
        return RedirectToAction("Detail", "Tasks", new { id });
    }
}