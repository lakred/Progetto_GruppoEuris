using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary;

namespace TestTDD.UnitTestGateway.RealDb;

public class UnitTestexperimentsGateway : IDisposable
{
    private ConcordiaContext _context;
    private PrioritiesGateway _gatewayPriority;
    private StatesGateway _gatewayState;
    private ExperimentsGateway _gateway;
    private List<Priority> _createdPriorities;
    private List<State> _createdStates;

    public UnitTestexperimentsGateway()
    {
        DBSettings.SetConnectionString("Your_ConnectionString");
        string connectionString = DBSettings.GetConnectionString();
        _context = new ConcordiaContext();
        _gatewayPriority = new PrioritiesGateway(_context);
        _gatewayState = new StatesGateway(_context);
        _gateway = new ExperimentsGateway(_context);
        _createdPriorities = new List<Priority>();
        _createdStates = new List<State>();
    }

    public void Dispose()
    {
        foreach (var priority in _createdPriorities)
        {
            _gatewayPriority.Delete(priority.Id.Value);
        }
        foreach (var state in _createdStates)
        {
            _gatewayState.Delete(state.Id.Value);
        }
        _context.Dispose();
    }

    private Priority CreatePriority()
    {
        var priority = new Priority(null, "code4", "namePriority4", "color4");
        Priority resultPriority = _gatewayPriority.Insert(priority);
        _createdPriorities.Add(resultPriority);
        return resultPriority;
    }

    private State CreateState()
    {
        var state = new State(null, "S001", "Not begin");
        State resultState = _gatewayState.Insert(state);
        _createdStates.Add(resultState);
        return resultState;
    }

    [Fact]
    public void GetById_ReturnsExperiment()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiment = new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5), resultPriority.Id.Value, resultState.Id.Value);
        var insertedExperiment = _gateway.Insert(experiment);
        var result = _gateway.GetById((int)insertedExperiment.Id);
        Assert.NotNull(result);
        Assert.Equal((int)insertedExperiment.Id, result.Id);
        _gateway.Delete(insertedExperiment.Id.Value);
    }

    [Fact]
    public void GetAll_ReturnsAllExperiments()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiments = new List<Experiment>
        {
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value)
         };
        _gateway.InsertMulti(experiments);
        var result = _gateway.GetAll();
        Assert.NotEmpty(result);
        var ids = experiments.Select(e => e.Id.Value);
        _gateway.DeleteMulti(ids);
    }

    [Fact]
    public void GetByIdMulti_ReturnsExperiments()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiments = new List<Experiment>
        {
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value)
         };
        _gateway.InsertMulti(experiments);
        var ids = experiments.Select(e => e.Id.Value);
        var result = _gateway.GetByIdMulti(ids);
        Assert.NotEmpty(result);
        _gateway.DeleteMulti(ids);
    }

    [Fact]
    public void Insert_ReturnsInsertedExperiment()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiment = new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5), resultPriority.Id.Value, resultState.Id.Value);
        var insertedExperiment = _gateway.Insert(experiment);
        Assert.NotNull(insertedExperiment);
        Assert.NotNull(insertedExperiment.Id);
        _gateway.Delete(insertedExperiment.Id.Value);
    }

    [Fact]
    public void InsertMulti_ReturnsInsertedExperiments()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiments = new List<Experiment>
        {
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value)
        };
        var insertedExperiments = _gateway.InsertMulti(experiments);
        Assert.NotEmpty(insertedExperiments);
        Assert.All(insertedExperiments, r => Assert.NotNull(r.Id));
        var idsToDelete = insertedExperiments.Select(e => e.Id.Value);
        _gateway.DeleteMulti(idsToDelete);
    }

    [Fact]
    public void Update_ReturnsUpdatedExperiment()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiment = new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5), resultPriority.Id.Value, resultState.Id.Value);
        experiment = _gateway.Insert(experiment);
        experiment.Name = "UpdatedName";
        experiment.Description = "UpdatedDescription";
        var updatedExperiment = _gateway.Update(experiment);
        Assert.NotNull(updatedExperiment);
        Assert.Equal("UpdatedName", updatedExperiment.Name);
        Assert.Equal("UpdatedDescription", updatedExperiment.Description);
        _gateway.Delete(updatedExperiment.Id.Value);
    }

    [Fact]
    public void UpdateMulti_ReturnsUpdatedExperiments()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiments = new List<Experiment>
        {
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code2", "name2", "description2", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code3", "name3", "description3", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value)
        };
        experiments = (List<Experiment>)_gateway.InsertMulti(experiments);
        foreach (var experiment in experiments)
        {
            experiment.Name = "UpdatedName";
            experiment.Description = "UpdatedDescription";
        }
        var updatedExperiments = _gateway.UpdateMulti(experiments);
        Assert.NotEmpty(updatedExperiments);
        Assert.All(updatedExperiments, r => Assert.Equal("UpdatedName", r.Name));
        Assert.All(updatedExperiments, r => Assert.Equal("UpdatedDescription", r.Description));
        var idsToDelete = updatedExperiments.Select(e => e.Id.Value).ToList();
        var deletedExperiments = _gateway.DeleteMulti(idsToDelete);
        Assert.NotEmpty(deletedExperiments);
        Assert.All(deletedExperiments, r => idsToDelete.Contains(r.Id.Value));
        foreach (var id in idsToDelete)
        {
            var deletedExperimentInDb = _context.Experiments.Find(id);
            Assert.Null(deletedExperimentInDb);
        }
    }

    [Fact]
    public void Delete_RemovesExperiment()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiment = new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5), resultPriority.Id.Value, resultState.Id.Value);
        var insertedExperiment = _gateway.Insert(experiment);
        Assert.NotNull(insertedExperiment);
        Assert.NotNull(insertedExperiment.Id);
        var deletedExperiment = _gateway.Delete(insertedExperiment.Id.Value);
        Assert.NotNull(deletedExperiment);
        Assert.Equal(insertedExperiment.Id, deletedExperiment.Id);
        Assert.Null(_context.Experiments.Find(insertedExperiment.Id));
    }

    [Fact]
    public void DeleteMulti_RemovesMultipleExperiments()
    {
        var resultPriority = CreatePriority();
        var resultState = CreateState();
        var experiments = new List<Experiment>
        {
            new Experiment(null, "code1", "name1", "description1", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code2", "name2", "description2", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value),
            new Experiment(null, "code3", "name3", "description3", true, DateTimeOffset.Now, DateTimeOffset.Now.AddDays(5),
            resultPriority.Id.Value, resultState.Id.Value)
        };
        experiments = (List<Experiment>)_gateway.InsertMulti(experiments);
        var idsToDelete = experiments.Select(e => e.Id.Value).ToList();
        var deletedExperiments = _gateway.DeleteMulti(idsToDelete);
        Assert.NotEmpty(deletedExperiments);
        Assert.All(deletedExperiments, r => idsToDelete.Contains(r.Id.Value));
        foreach (var id in idsToDelete)
        {
            var deletedExperimentInDb = _context.Experiments.Find(id);
            Assert.Null(deletedExperimentInDb);
        }
    }
}