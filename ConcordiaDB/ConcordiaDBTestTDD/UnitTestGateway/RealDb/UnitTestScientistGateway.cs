using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using ConcordiaDBLibrary;

namespace TestTDD.UnitTestGateway.RealDb;

public class UnitTestScientistGateway 
{
    private readonly ConcordiaContext _context;    
    private readonly ScientistsGateway _gateway;   
    
    public UnitTestScientistGateway()
    {
        DBSettings.SetConnectionString("Your_ConnectionString");
        string connectionString = DBSettings.GetConnectionString();
        _context = new ConcordiaContext();
        _gateway = new ScientistsGateway(_context);
    }

    private Scientist createScientist()
    {
        var scientist = new Scientist(null, "S001", "John Doe");
        var resultScientist = _gateway.Insert(scientist);
        return resultScientist;
    }

    [Fact]
    public void GetAll_Valid()
    {
        var numOfScientists = _gateway.GetAll().Count();
        var newScientist = createScientist();        
        var result = _gateway.GetAll();
        Assert.Equal(numOfScientists + 1, result.Count());
        _gateway.Delete(newScientist.Id.Value);
    }

    [Fact]
    public void GetById_Valid()
    {
        var newScientist = createScientist();
        var result = _gateway.GetById(newScientist.Id.Value);
        Assert.Equal(newScientist, result);
        Assert.NotNull(result);
        _gateway.Delete(newScientist.Id.Value);
    }

    [Fact]
    public void Insert_Valid()
    {
        var scientist = new Scientist(null, "S001", "John Doe");
        var result = _gateway.Insert(scientist);
        var expectedScientist = _gateway.GetById(result.Id.Value);
        Assert.Equal(expectedScientist, result);
        _gateway.Delete(result.Id.Value);
    }

    [Fact]
    public void Insert_Multi()
    {
        var scientist = new Scientist(null, "S001", "John Doe");
        var scientist2 = new Scientist(null, "S002", "Frank Doe");
        var scientistsToInsert = new List<Scientist> { scientist,scientist2 };
        var result = _gateway.InsertMulti(scientistsToInsert);
        Assert.Equal(2, result.Count());
        foreach(var scientistToDelete in result)
        {
            _gateway.Delete(scientistToDelete.Id.Value);
        }
    }

    [Fact]
    public void Update_Valid()
    {
        var scientist = createScientist();
        var updatedScientist = new Scientist(scientist.Id.Value, "S003", "Frank Doe");
        var result = _gateway.Update(updatedScientist);
        var expectedCode = "S003";
        var expectedName =  "Frank Doe";
        Assert.Equal(expectedCode, result.Code);
        Assert.Equal(expectedName, result.FullName);
        _gateway.Delete(result.Id.Value);
    }

    [Fact]
    public void UpdateMulti_Valid()
    {
        var scientist1 = new Scientist(null, "S001", "John Doe");
        var scientist2 = new Scientist(null, "S002", "Frank Doe");
        var insertScientist1 = _gateway.Insert(scientist1);
        var insertScientist2 = _gateway.Insert(scientist2);
        var updatedScientist1 = new Scientist(insertScientist1.Id.Value, "S003", "Frank Doe");
        var updatedScientist2 = new Scientist(insertScientist2.Id.Value, "S004", "Jane Green");
        var updatedList = new List<Scientist> { updatedScientist1,updatedScientist2 };
        var result = _gateway.UpdateMulti(updatedList);
        var resultCode1 = result.Where(x => x.Id == updatedScientist1.Id.Value).ToList().Select(x => x.Code).First();
        var resultCode2 = result.Where(x => x.Id == updatedScientist2.Id.Value).ToList().Select(x => x.Code).First();
        var resultName1 = result.Where(x => x.Id == updatedScientist1.Id.Value).ToList().Select(x => x.FullName).First();
        var resultName2 = result.Where(x => x.Id == updatedScientist2.Id.Value).ToList().Select(x => x.FullName).First();
        var expectedCode1 = "S003";
        var expectedCode2 = "S004";
        var expectedName1 = "Frank Doe";
        var expectedName2 = "Jane Green";
        Assert.Equal(expectedCode1, resultCode1);
        Assert.Equal(expectedCode2, resultCode2);
        Assert.Equal(expectedName1, resultName1);
        Assert.Equal(expectedName2, resultName2);
        foreach( var scientist in result)
        {
            _gateway.Delete(scientist.Id.Value);
        }
    }

    [Fact]
    public void Delete_Valid()
    {
        var numOfScientist = _gateway.GetAll().Count();
        var scientist = createScientist();
        var result = _gateway.Delete(scientist.Id.Value);
        var updatedNumOfScientist = _gateway.GetAll().Count();
        Assert.NotNull(result);
        Assert.Equal(numOfScientist, updatedNumOfScientist);
    }

    [Fact]
    public void DeleteMulti_Valid() 
    {
        var numOfScientist = _gateway.GetAll().Count();
        var scientist = createScientist(); 
        var scientist2 = createScientist();
        var listOfInsertedId = new List<int> { scientist.Id.Value, scientist2.Id.Value };
        var result = _gateway.DeleteMulti(listOfInsertedId);
        var updatedNumOfScientist = _gateway.GetAll().Count();
        Assert.NotNull(result);
        Assert.Equal(numOfScientist, updatedNumOfScientist);        
    }
}