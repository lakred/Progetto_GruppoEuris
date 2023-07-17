using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestTDD.UnitTestGateway.SimulationDb;

public class ParticipantsGatewayTest 
{
    private ConcordiaContext _context;
    private ExperimentsGateway _experimentsGateway;
    private ParticipantsGateway _participantsGateway;
    private ParticipantsGateway _gateway;
    private List<Experiment> _createdExperiments;
    private List<Participant> _createdParticipants;

    public ParticipantsGatewayTest()
    {
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ConcordiaContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;
        _context = new ConcordiaContext(options);
        _experimentsGateway = new ExperimentsGateway(_context);
        _participantsGateway = new ParticipantsGateway(_context);
        _gateway = new ParticipantsGateway(_context);
        _createdExperiments = new List<Experiment>();
        _createdParticipants = new List<Participant>();
    }

    private Experiment CreateExperiment()
    {
        var experiment = new Experiment(null, "E001", "Test Experiment", "Description", false, null, null, 1, 1);
        Experiment createdExperiment = _experimentsGateway.Insert(experiment);
        _createdExperiments.Add(createdExperiment);
        return createdExperiment;
    }

    private Participant CreateParticipant()
    {
        var experiment = CreateExperiment();
        var participant = new Participant(null, experiment.Id.Value, null);
        Participant createdParticipant = _participantsGateway.Insert(participant);
        _createdParticipants.Add(createdParticipant);
        return createdParticipant;
    }

    [Fact]
    public void GetAll_ReturnsAllExperimentParticipants()
    {
        var experiment = CreateExperiment();
        var participant = CreateParticipant();
        var experimentParticipant = new Participant(null, experiment.Id.Value, participant.Id.Value);
        _gateway.Insert(experimentParticipant);
        var result = _gateway.GetAll();
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetById_ReturnsCorrectParticipant()
    {
        var experiment = CreateExperiment();
        var participant = new Participant(null, experiment.Id.Value, null);
        var insertedParticipant = _gateway.Insert(participant);
        var result = _gateway.GetById(insertedParticipant.Id.Value);
        Assert.NotNull(result);
        Assert.Equal(insertedParticipant.Id.Value, result.Id.Value);
        _gateway.Delete(participant.Id.Value);
    }

    [Fact]
    public void Insert_InsertsNewParticipant()
    {
        var experiment = CreateExperiment();
        var participant = new Participant(null, experiment.Id.Value, null);
        var result = _gateway.Insert(participant);
        Assert.NotNull(result);
        _gateway.Delete(participant.Id.Value);
    }

    [Fact]
    public void Update_UpdatesExistingParticipant()
    {
        var experiment = CreateExperiment();
        var participant = new Participant(null, experiment.Id.Value, null);
        var insertedParticipant = _gateway.Insert(participant);
        insertedParticipant.ExperimentId = experiment.Id.Value;
        var updated = _gateway.Update(insertedParticipant);
        Assert.NotNull(updated);
        Assert.Equal(insertedParticipant.ExperimentId, updated.ExperimentId);
        _gateway.Delete(updated.Id.Value);
    }

    [Fact]
    public void Delete_DeletesExistingParticipant()
    {
        var experiment = CreateExperiment();
        var participant = new Participant(null, experiment.Id.Value, null);
        var insertedParticipant = _gateway.Insert(participant);
        _gateway.Delete(insertedParticipant.Id.Value);
        var result = _gateway.GetById(insertedParticipant.Id.Value);
        Assert.Null(result);
    }

    [Fact]
    public void GetByIdMulti_ReturnsCorrectParticipants()
    {
        var experiment = CreateExperiment();
        var participant1 = new Participant(null, experiment.Id.Value, null);
        var participant2 = new Participant(null, experiment.Id.Value, null);
        var insertedParticipant1 = _gateway.Insert(participant1);
        var insertedParticipant2 = _gateway.Insert(participant2);
        var result = _gateway.GetByIdMulti(new List<int> { insertedParticipant1.Id.Value, insertedParticipant2.Id.Value });
        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Id == insertedParticipant1.Id.Value);
        Assert.Contains(result, p => p.Id == insertedParticipant2.Id.Value);
        _gateway.Delete(participant1.Id.Value);
        _gateway.Delete(participant2.Id.Value);
    }

    [Fact]
    public void InsertMulti_InsertsNewParticipants()
    {
        var experiment = CreateExperiment();
        var participant1 = new Participant(null, experiment.Id.Value, null);
        var participant2 = new Participant(null, experiment.Id.Value, null);
        var result = _gateway.InsertMulti(new List<Participant> { participant1, participant2 });
        Assert.Equal(2, result.Count());
        foreach (var participant in result)
        {
            _gateway.Delete(participant.Id.Value);
        }
    }

    [Fact]
    public void UpdateMulti_UpdatesExistingParticipants()
    {
        var experiment = CreateExperiment();
        var participant1 = new Participant(null, experiment.Id.Value, null);
        var participant2 = new Participant(null, experiment.Id.Value, null);
        var insertedParticipant1 = _gateway.Insert(participant1);
        var insertedParticipant2 = _gateway.Insert(participant2);
        insertedParticipant1.ExperimentId = experiment.Id.Value;
        insertedParticipant2.ExperimentId = experiment.Id.Value;
        var updatedParticipants = _gateway.UpdateMulti(new List<Participant> { insertedParticipant1, insertedParticipant2 });
        Assert.NotNull(updatedParticipants);
        Assert.All(updatedParticipants, participant => Assert.Equal(experiment.Id.Value, participant.ExperimentId));
        foreach (var participant in updatedParticipants)
        {
            _gateway.Delete(participant.Id.Value);
        }
    }

    [Fact]
    public void DeleteMulti_DeletesExistingParticipants()
    {
        var experiment = CreateExperiment();
        var participant1 = new Participant(null, experiment.Id.Value, null);
        var participant2 = new Participant(null, experiment.Id.Value, null);
        var insertedParticipant1 = _gateway.Insert(participant1);
        var insertedParticipant2 = _gateway.Insert(participant2);
        _gateway.DeleteMulti(new List<int> { insertedParticipant1.Id.Value, insertedParticipant2.Id.Value });
        var result1 = _gateway.GetById(insertedParticipant1.Id.Value);
        var result2 = _gateway.GetById(insertedParticipant2.Id.Value);
        Assert.Null(result1);
        Assert.Null(result2);
    }
}