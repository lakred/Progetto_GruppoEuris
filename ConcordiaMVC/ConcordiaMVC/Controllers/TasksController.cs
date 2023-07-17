namespace ConcordiaMVC.Controllers;

using Microsoft.AspNetCore.Mvc;
using ConcordiaDBLibrary.Models.Abstract;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary.Models.Extensions;
using ConcordiaDBLibrary.Gateways.Abstract;
using Models;

public class TasksController : Controller
{
    private readonly IEntityGateway<Experiment> _experimentsGway;
    private readonly IEntityGateway<Remark> _remarksGway;
    private readonly IEntityGateway<Priority> _priorityGway;
    private readonly IEntityGateway<Scientist> _scientistsGway;
    private readonly IEntityGateway<State> _statesGway;
    private readonly IEntityGateway<Participant> _participantsGway;

    private readonly ILogger<TasksController> _logger;

    public TasksController(IEntityGateway<Experiment> experimentsGway,
                           IEntityGateway<Remark> remarksGway,
                           IEntityGateway<Priority> priorityGway,
                           IEntityGateway<Scientist> scientistsGway,
                           IEntityGateway<State> statesGway,
                           IEntityGateway<Participant> participantsGway,
                           ILogger<TasksController> logger)
    {
        _experimentsGway = experimentsGway;
        _participantsGway = participantsGway;
        _remarksGway = remarksGway;
        _priorityGway = priorityGway;
        _scientistsGway = scientistsGway;
        _statesGway = statesGway;
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation($"{nameof(TasksController)}.{nameof(TasksController.Index)} was called.");
        // Experiments and Remarks
        var experiments = _experimentsGway.GetAll().ToList();
        var remarks = _remarksGway.GetAll().ToList();
        // Experiments Remarked and Experiments Not Remarked
        var expsInRemarks = (IEnumerable<Experiment>?)remarks.Select(r => r.Experiment).Distinct().ToList() ?? new List<Experiment>();
        var expsRemarked = experiments.Intersect(expsInRemarks, new EntityEqualityComparer<Experiment>());
        var expsNotRemarked = experiments.Except(expsInRemarks, new EntityEqualityComparer<Experiment>());
        // Tasks With Last Comment
        var tasksRemarked = new List<Task>();
        if (expsRemarked is not null)
        {
            foreach (var ex in expsRemarked)
            {
                tasksRemarked.Add(new Task(ex, remarks.Where(r => r.ExperimentId == ex.Id).OrderBy(r => r.Date).Last()));
            }
        }
        // Tasks Without Comments (Last Comment = null)
        var tasksNotRemarked = new List<Task>();
        if (expsNotRemarked is not null)
        {
            foreach (var ex in expsNotRemarked)
            {
                tasksRemarked.Add(new Task(ex, null));
            }
        }
        // Tasks With Last Comment and Tasks Without Comments
        var tasks = tasksRemarked.Concat(tasksNotRemarked);
        return View(new TasksList(tasks.OrderBy(t => t.Experiment.DueDate).OrderBy(t => t.Experiment.Ordering()).ToList()));
    }

    public IActionResult Detail(int id)
    {
        _logger.LogInformation($"{nameof(TasksController)}.{nameof(TasksController.Detail)} was called.");
        // States and Scientists
        var states = _statesGway.GetAll().ToList();
        var scientists = _scientistsGway.GetAll().ToList();
        // Experiment
        var experiment = _experimentsGway.GetById(id);
        if (experiment is null)
        {
            return View(new TaskBig(experiment, scientists, states));
        }
        // Scientists in Experiemnt
        IEnumerable<Scientist>? scientistsInExperiment = new List<Scientist>();
        var participants = _participantsGway.GetAll().Where(p => p.ExperimentId == id).ToList();
        if (participants is not null)
        {
            var assignees = participants.Select(p => p.Scientist).ToList();
            if (assignees is not null && !assignees.Any(s => s == null))
            {
                scientistsInExperiment = (IEnumerable<Scientist>?)assignees.OrderBy(s => s!.FullName).ToList();
            }
        }
        // Remarks in Experiemnt
        IEnumerable<Remark>? remarksInExperiment = new List<Remark>();
        var remarks = _remarksGway.GetAll().Where(r => r.ExperimentId == id).ToList();
        if (remarks is not null)
        {
            remarksInExperiment = remarks.OrderBy(r => r.Date).ToList();
        }
        return View(new TaskBig(experiment, scientistsInExperiment, remarksInExperiment, scientists, states));
    }

    public IActionResult Update(TaskBig taskBig)
    {
        _logger.LogInformation($"{nameof(TasksController)}.{nameof(TasksController.Update)} was called.");
        if (taskBig.Experiment is not null && taskBig.SelectedState > 0)
        {
            _experimentsGway.Update(new Experiment(taskBig.Experiment.Id, taskBig.Experiment.Code, taskBig.Experiment.Name,
                                                   taskBig.Experiment.Description, false, taskBig.Experiment.StartDate,
                                                   taskBig.Experiment.DueDate, taskBig.Experiment.PriorityId, taskBig.SelectedState));
        }
        return RedirectToAction("Index", "Tasks");
    }

    public IActionResult Insert(TaskBig taskBig)
    {
        _logger.LogInformation($"{nameof(TasksController)}.{nameof(TasksController.Insert)} was called.");
        if (taskBig.Experiment is not null && taskBig.Comment is not null)
        {
            var comment = taskBig.Comment.Trim();
            if (comment != string.Empty && taskBig.SelectedAuthor > 0)
            {
                _remarksGway.Insert(new Remark(null, null, taskBig.Comment, DateTimeOffset.Now,
                                               (int)taskBig.Experiment.Id!, taskBig.SelectedAuthor));
            }
        }
        return RedirectToAction("Index", "Tasks");
    }
}