using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary;

namespace TestTDD.UnitTestGateway.RealDb;

public class UnitTestRemarkGateway : IDisposable
{
    private readonly ConcordiaContext _context;
    private readonly RemarksGateway _gateway;
    private readonly ScientistsGateway _gatewayScientist;
    private readonly ExperimentsGateway _gatewayExperiment;
    private readonly StatesGateway _gatewayState;
    private readonly PrioritiesGateway _gatewayPriority;
    private readonly List<Experiment> _createdExperiments;
    private readonly List<Scientist> _createdScientists;
    private readonly List<State> _createdStates;
    private readonly List<Priority> _createdPriorities;

    public UnitTestRemarkGateway()
    {
        DBSettings.SetConnectionString("Your_ConnectionString");
        string connectionString = DBSettings.GetConnectionString();
        _context = new ConcordiaContext();
        _gateway = new RemarksGateway(_context);
        _gatewayScientist = new ScientistsGateway(_context);
        _gatewayExperiment = new ExperimentsGateway(_context);
        _gatewayPriority = new PrioritiesGateway(_context);
        _gatewayState = new StatesGateway(_context);
        _createdExperiments = new List<Experiment>();
        _createdScientists = new List<Scientist>();
        _createdStates = new List<State>();
        _createdPriorities = new List<Priority>();        
    }

    public void Dispose()
    {
        foreach ( var scientist in _createdScientists )
        {
            _gatewayScientist.Delete(scientist.Id.Value);
        }
        foreach ( var experiment in _createdExperiments )
        {
            _gatewayExperiment.Delete(experiment.Id.Value);
        }
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

    private Scientist createScientist()
    {
        var scientist = new Scientist(null, "S001", "John Doe");
        var resultScientist = _gatewayScientist.Insert(scientist);
        _createdScientists.Add(resultScientist);
        return resultScientist;
    }

    private Priority createPriority()
    {
        var priority = new Priority(null, "P001", "High", "Red");
        var createdPriority = _gatewayPriority.Insert(priority);
        _createdPriorities.Add(createdPriority);
        return createdPriority;
    }

    private State createState()
    {
        var state = new State(null, "S001", "Initial");
        State createdState = _gatewayState.Insert(state);
        _createdStates.Add(createdState);
        return createdState;
    }

    private Experiment createExperiment()
    {
        var priority = createPriority();
        var state = createState();
        var experiment = new Experiment(null, "E001", "Test Experiment", "Description", false, null, null, priority.Id.Value, state.Id.Value);
        Experiment createdExperiment = _gatewayExperiment.Insert(experiment);
        _createdExperiments.Add(createdExperiment);
        return createdExperiment;
    }

    [Fact]
    public void GetAll_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var initialCount = _gateway.GetAll().Count();
        var remark1 = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var remark2 = new Remark(null, "R002", "Second remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        _gateway.Insert(remark1);
        _gateway.Insert(remark2);
        var result = _gateway.GetAll().ToList();

        Assert.Equal(initialCount + 2, result.Count());

        _gateway.Delete(remark1.Id.Value);
        _gateway.Delete(remark2.Id.Value);
    }

    [Fact]
    public void GetById_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();            
        var remark = new Remark (null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var expectedRemark = _gateway.Insert(remark);
        var result = _gateway.GetById(remark.Id.Value);

        Assert.NotNull(result);
        Assert.Equal(expectedRemark, result);
        _gateway.Delete(expectedRemark.Id.Value);

    }

    [Fact]
    public void Insert_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var remark = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var result = _gateway.Insert(remark);
        var expectedRemark = _gateway.GetById(remark.Id.Value);

        Assert.Equal(expectedRemark, result);
        _gateway.Delete(remark.Id.Value);
    }

    [Fact]
    public void InsertMulti_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var remark1 = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var remark2 = new Remark(null, "R002", "Second remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var insertedRemarks = new List<Remark> { remark1, remark2 };
        var result = _gateway.InsertMulti(insertedRemarks);

        Assert.Equal(2, result.Count());

        foreach( var remark in result)
        {
            _gateway.Delete(remark.Id.Value);
        }

    }

    [Fact]
    public void Update_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var remark = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var insertedRemark = _gateway.Insert(remark);
        var newRemark = new Remark(insertedRemark.Id.Value, "R001", "Updated remark", DateTimeOffset.Now, insertedRemark.ExperimentId, insertedRemark.ScientistId);
        var expectedText = "Updated remark";
        var result = _gateway.Update(newRemark);

        Assert.NotNull(result);
        Assert.Equal(expectedText, result.Text);
                   
        _gateway.Delete(result.Id.Value);            

    }

    [Fact]
    public void UpdateMulti_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var remark = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var remark2 = new Remark(null, "R002", "Second remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var insertedRemark = _gateway.Insert(remark); 
        var insertedRemark2 = _gateway.Insert(remark2);
        var updatedRemark = new Remark(insertedRemark.Id.Value, "R001", "Updated first remark", DateTimeOffset.Now, insertedRemark.ExperimentId, insertedRemark.ScientistId);
        var updatedRemark2 = new Remark(insertedRemark2.Id.Value, "R002", "Updated second remark", DateTimeOffset.Now, insertedRemark2.ExperimentId, insertedRemark2.ScientistId);
        var listOfInsertedRemarks = new List<Remark> { updatedRemark, updatedRemark2 };

        var result = _gateway.UpdateMulti(listOfInsertedRemarks);
        var resultText = result.Where( x => x.Id == insertedRemark.Id.Value ).ToList().Select(x => x.Text).First();
        var resultText2 = result.Where(x => x.Id == insertedRemark2.Id.Value).ToList().Select(x => x.Text).First();
        var expectedText = "Updated first remark";
        var expectedText2 = "Updated second remark";

        Assert.NotNull(result);
        Assert.Equal(expectedText, resultText);
        Assert.Equal(expectedText2, resultText2);

        foreach( var remarkToDelete in result)
        {
            _gateway.Delete(remarkToDelete.Id.Value);
        }
    }

    [Fact]
    public void Delete_Valid()
    {
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var remark = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var numOfRemarks = _gateway.GetAll().Count();
        var insertRemark = _gateway.Insert(remark);
        var result = _gateway.Delete(insertRemark.Id.Value);
        var updatedNumOfRemarks = _gateway.GetAll().Count();

        Assert.NotNull(result);
        Assert.Equal(numOfRemarks, updatedNumOfRemarks);
    }

    [Fact]
    public void DeleteMulti_Valid() 
    {
        var numOfRemarks = _gateway.GetAll().Count();
        var resultScientist = createScientist();
        var resultExperiment = createExperiment();
        var remark = new Remark(null, "R001", "First remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var remark2 = new Remark(null, "R002", "Second remark", DateTimeOffset.Now, resultExperiment.Id.Value, resultScientist.Id.Value);
        var insertedRemark = _gateway.Insert(remark);
        var insertedRemark2 = _gateway.Insert(remark2);
        var listOfInsertedRemarks = new List<int> { insertedRemark.Id.Value, insertedRemark2.Id.Value };

        var result = _gateway.DeleteMulti(listOfInsertedRemarks);
        var updatedNumOfRemarks = _gateway.GetAll().Count();

        Assert.NotNull(result);
        Assert.Equal(numOfRemarks, updatedNumOfRemarks);

    }
}