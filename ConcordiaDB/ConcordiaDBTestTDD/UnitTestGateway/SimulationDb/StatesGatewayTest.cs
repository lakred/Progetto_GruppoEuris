using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestTDD.UnitTestGateway.SimulationDb;

public class StatesGatewayTest
{
    private ConcordiaContext _context;
    private StatesGateway _gateway;
    public StatesGatewayTest()
    {
        var serviceProvider = new ServiceCollection()
                 .AddEntityFrameworkInMemoryDatabase()
                 .BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ConcordiaContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;
        _context = new ConcordiaContext(options);
        _gateway = new StatesGateway(_context);
    }

    [Fact]
    public void GetAll_Valid()
    {
        var state1 = new State(1, "ST001", "In progress");
        var state2 = new State(2, "ST002", "Completed");
        var state3 = new State(3, "ST003", "Not begin");
        _gateway.Insert(state1);
        _gateway.Insert(state2);
        _gateway.Insert(state3);
        var result = _gateway.GetAll();

        Assert.Equal(3, result.Count());
    }

    [Fact]
    public void GetById_Valid()
    {
        var stateId = 1;
        var expectedState = new State(stateId, "ST001", "In progress");
        _gateway.Insert(expectedState);
        var result = _gateway.GetById(stateId);

        Assert.Equal(expectedState.Id, result.Id);
        Assert.Equal(expectedState.Code, result.Code);
        Assert.Equal(expectedState.Name, result.Name);
    }

    [Fact]
    public void GetByIdMulti_Valid()
    {
        var ids = new List<int> { 1, 2, 3 };
        var state1 = new State(1, "ST001", "In progress");
        var state2 = new State(2, "ST002", "Completed");
        var state3 = new State(3, "ST003", "Not begin");
        _gateway.Insert(state1);
        _gateway.Insert(state2);
        _gateway.Insert(state3);
        var result = _gateway.GetByIdMulti(ids);
        var expectedIds = new List<int>
        { state1.Id.Value, state2.Id.Value, state3.Id.Value };
        var expectedName = new List<string> { state1.Name, state2.Name, state3.Name };

        Assert.Equal(expectedIds, result.Select(s => s.Id.Value).ToList());
        Assert.Equal(expectedName, result.Select(s => s.Name).ToList());
    }

    [Fact]
    public void Insert_Valid()
    {
        var expectedState = new State(1, "S001", "Not begin");
        var result = _gateway.Insert(expectedState);

        Assert.Equal(expectedState, result);
    }

    [Fact]
    public void InsertMulti_Valid()
    {
        var expectedStates = new List<State>
        {
            new State(1, "ST001", "Pending"),
            new State(2, "ST002", "In Progress"),
            new State(3, "ST003", "Completed"),
            new State(4, "ST004", "Cancelled"),
            new State(5, "ST005", "On Hold")
        };
        var result = _gateway.InsertMulti(expectedStates);

        Assert.NotNull(result);
        Assert.Equal(expectedStates.Count(), result.Count());
        for (int i = 0; i < expectedStates.Count(); i++)
        {
            var expectedState = expectedStates[i];
            var actualState = result.ElementAt(i);

            Assert.Equal(expectedState.Id, actualState.Id);
            Assert.Equal(expectedState.Name, actualState.Name);
            Assert.Equal(expectedState.Code, actualState.Code);
        }
    }

    [Fact]
    public void Update_Valid()
    {
        var existingState = new State(1, "ST001", "In progress");
        _gateway.Insert(existingState);
        var newState = new State(1, "ST002", "Completed");
        var result = _gateway.Update(newState);

        Assert.Equal(newState.Id, result.Id);
        Assert.Equal(newState.Code, result.Code);
        Assert.Equal(newState.Name, result.Name);

        var updatedStateFromDB = _gateway.GetById(newState.Id.Value);

        Assert.NotNull(updatedStateFromDB);
        Assert.Equal(newState.Name, updatedStateFromDB.Name);
        Assert.Equal(newState.Id, updatedStateFromDB.Id);
        Assert.Equal(newState.Code, updatedStateFromDB.Code);
    }

    [Fact]
    public void UpdateMulti_Valid()
    {
        var statesToUpdate = new List<State>
            {
                new State(1, "ST001", "Pending"),
                new State(2, "ST002", "In Progress"),
                new State(3, "ST003", "In progress"),
                new State(4, "ST004", "Cancelled"),
                new State(5, "ST005", "On Hold")
            };

        foreach (var state in statesToUpdate)
        {
            _gateway.Insert(state);
        }

        var newStates = new List<State>
            {
                new State(1, "ST001", "In progress"),
                new State(2, "ST002", "Completed"),
                new State(3, "ST003", "Completed"),
                new State(4, "ST004", "In progress"),
                new State(5, "ST005", "Completed")
            };

        var result = _gateway.UpdateMulti(newStates);

        Assert.NotNull(result);
        foreach (var updatedState in result)
        {
            var originalState = statesToUpdate.FirstOrDefault(s => s.Id == updatedState.Id);
            Assert.NotNull(originalState);
            Assert.Equal(originalState.Name, updatedState.Name);
            Assert.Equal(originalState.Code, updatedState.Code);
        }

        foreach (var updatedState in result)
        {
            var stateFromDB = _gateway.GetById(updatedState.Id.Value);
            Assert.NotNull(stateFromDB);
            Assert.Equal(updatedState.Name, stateFromDB.Name);
            Assert.Equal(updatedState.Code, stateFromDB.Code);
            Assert.Equal(updatedState.Id, stateFromDB.Id);
        }
    }

    [Fact]
    public void Delete_Valid()
    {
        var stateToDelete = new State(1, "ST001", "Completed");
        _gateway.Insert(stateToDelete);
        var result = _gateway.Delete(stateToDelete.Id.Value);

        Assert.NotNull(result);
        Assert.Equal(stateToDelete.Id, result.Id);
        Assert.Equal(stateToDelete.Name, result.Name);
        Assert.Equal(stateToDelete.Code, result.Code);

        var stateFromDB = _gateway.GetById(stateToDelete.Id.Value);
        Assert.Null(stateFromDB);
    }

    [Fact]
    public void DeleteMulti_Valid()
    {
        var ids = new List<int> { 1, 2, 3 };
        var state1 = new State(1, "ST001", "In progress");
        var state2 = new State(2, "ST002", "Completed");
        var state3 = new State(3, "ST003", "Not begin");
        _gateway.Insert(state1);
        _gateway.Insert(state2);
        _gateway.Insert(state3);
        var result = _gateway.DeleteMulti(ids);
        var expected = new List<State> { state1, state2, state3 };

        Assert.Equal(expected, result);

        foreach (var deletedState in expected)
        {
            var stateFromDB = _gateway.GetById(deletedState.Id.Value);
            Assert.Null(stateFromDB);
        }
    }
}
