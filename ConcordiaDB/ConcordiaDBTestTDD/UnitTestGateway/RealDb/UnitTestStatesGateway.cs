using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary;

namespace TestTDD.UnitTestGateway.RealDb;

public class UnitTestStatesGateway
{
    private readonly ConcordiaContext _context;
    private readonly StatesGateway _gateway;
    public UnitTestStatesGateway()
    {
        DBSettings.SetConnectionString("Your_ConnectionString");
        string connectionString = DBSettings.GetConnectionString();
        _context = new ConcordiaContext();
        _gateway = new StatesGateway(_context);
    }

    private State createState()
    {
        var state = new State(null, "T001", "NameToDelete");        
        return _gateway.Insert(state);
    }

    [Fact]
    public void GetAll_Valid()
    {
        var numOfStates = _gateway.GetAll().Count();
        var state = createState();
        var result = _gateway.GetAll();
        Assert.Equal(numOfStates+1, result.Count() );
        _gateway.Delete(state.Id.Value);
    }

    [Fact]
    public void GetById_Valid()
    {
        var state = createState();
        var result = _gateway.GetById(state.Id.Value);
        Assert.Equal(state, result);
        Assert.NotNull(result);
        _gateway.Delete(state.Id.Value);
    }

    [Fact]
    public void Insert_Valid()
    {
        var state = new State(null, "T001", "NameToDelete");
        var result = _gateway.Insert(state);
        var expected = _gateway.GetById(result.Id.Value);
        Assert.Equal(expected, result);
        Assert.NotNull(result);
        _gateway.Delete(expected.Id.Value);
    }

    [Fact]
    public void InsertMulti_Valid()
    {
        var numOfStates = _gateway.GetAll().Count();
        var state1 = new State(null, "TestToDelete", "T001");
        var state2 = new State(null, "TestToDelete2", "T002");
        var statesToInsert = new List<State> { state1,state2 };
        var result = _gateway.InsertMulti(statesToInsert);
        var newNumOfState = _gateway.GetAll().Count();
        Assert.Equal(2, result.Count());
        Assert.Equal(newNumOfState - 2, numOfStates);
        foreach( var state in result )
        {
            _gateway.Delete(state.Id.Value);
        }
    }

    [Fact]
    public void Update_Valid() 
    {
        var state = createState();
        var updatedState = new State(state.Id.Value, "T002","NameToUpdate");
        var result = _gateway.Update(updatedState);
        var expectedName = "NameToUpdate";
        var expectedCode = "T002";
        Assert.Equal(expectedCode, result.Code);
        Assert.Equal(expectedName, result.Name);
        _gateway.Delete(result.Id.Value);
    }

    [Fact]
    public void UpdateMulti_Valid() 
    {
        var state1 = createState();
        var state2 = createState();
        var updatedState1 = new State(state1.Id.Value, "T002", "NameToUpdate");
        var updatedState2 = new State(state2.Id.Value, "T003", "NameToUpdate2");
        var statesToUpdate = new List<State>
        { updatedState1, updatedState2 };
        var result = _gateway.UpdateMulti(statesToUpdate);
        var resultCode1 = result.Where(x => x.Id == updatedState1.Id.Value).ToList().Select(x => x.Code).First();
        var resultCode2 = result.Where(x => x.Id == updatedState2.Id.Value).ToList().Select(x => x.Code).First();
        var resultName1 = result.Where(x => x.Id == updatedState1.Id.Value).ToList().Select(x => x.Name).First();
        var resultName2 = result.Where(x => x.Id == updatedState2.Id.Value).ToList().Select(x => x.Name).First();
        var expectedCode1 = "T002";
        var expectedCode2 = "T003";
        var expectedName1 = "NameToUpdate";
        var expectedName2 = "NameToUpdate2";
        Assert.Equal(expectedCode1, resultCode1);
        Assert.Equal(expectedCode2, resultCode2);
        Assert.Equal(expectedName1, resultName1); 
        Assert.Equal(expectedName2, resultName2);
        foreach (var state in result)
        {
            _gateway.Delete(state.Id.Value);
        }
    }

    [Fact]
    public void Delete_Valid()
    {
        var numOfStates = _gateway.GetAll().Count();
        var state = createState();
        var result = _gateway.Delete(state.Id.Value);
        var updatedNumOfStates = _gateway.GetAll().Count();
        Assert.NotNull(result);
        Assert.Equal(numOfStates, updatedNumOfStates);
    }

    [Fact]
    public void DeleteMulti_Valid()
    {
        var numOfStates = _gateway.GetAll().Count();
        var state = createState();
        var state2 = createState();
        var listOfIdStates = new List<int> { state.Id.Value, state2.Id.Value };
        var result = _gateway.DeleteMulti(listOfIdStates);
        var updatedNumOfStates = _gateway.GetAll().Count();
        Assert.NotNull(result);
        Assert.Equal(numOfStates, updatedNumOfStates);
    }
}