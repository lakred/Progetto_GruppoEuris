using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestTDD.UnitTestGateway.SimulationDb;

public class ScientistGatewayTest
{
    private ConcordiaContext _context;
    private ScientistsGateway _gateway;
    public ScientistGatewayTest()
    {
        var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ConcordiaContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;
        _context = new ConcordiaContext(options);
        _gateway = new ScientistsGateway(_context);
    }

    [Fact]
    public void GetAll_Valid()
    {
        var scientist1 = new Scientist(1, "SC001", "John Doe");
        var scientist2 = new Scientist(2, "SC002", "Jane Smith");
        var scientist3 = new Scientist(3, "SC003", "Michael Johnson");
        var scientist4 = new Scientist(4, "SC003", "Frank Johnson");
        _gateway.Insert(scientist1);
        _gateway.Insert(scientist2);
        _gateway.Insert(scientist3);
        _gateway.Insert(scientist4);
        var result = _gateway.GetAll();

        Assert.Equal(4, result.Count());
    }

    [Fact]
    public void GetById_Valid()
    {
        var scientistId = 1;
        var expectedScientist = new Scientist(scientistId, "SC001", "John Doe");
        _gateway.Insert(expectedScientist);
        var result = _gateway.GetById(scientistId);

        Assert.Equal(expectedScientist.Id, result.Id);
        Assert.Equal(expectedScientist.Code, result.Code);
        Assert.Equal(expectedScientist.FullName, result.FullName);
    }

    [Fact]
    public void GetByIDMulti_Valid()
    {
        var ids = new List<int> { 1, 2, 3, };
        var expectedScientist1 = new Scientist(1, "SC001", "John Doe");
        var expectedScientist2 = new Scientist(2, "SC002", "Jane Smith");
        var expectedScientist3 = new Scientist(3, "SC003", "Michael Johnson");
        _gateway.Insert(expectedScientist1);
        _gateway.Insert(expectedScientist2);
        _gateway.Insert(expectedScientist3);
        var result = _gateway.GetByIdMulti(ids);
        var expectedIds = new List<int> { expectedScientist1.Id.Value, expectedScientist2.Id.Value, expectedScientist3.Id.Value };
        var expectedFullName = new List<string> { expectedScientist1.FullName, expectedScientist2.FullName, expectedScientist3.FullName };
        var expectedCode = new List<string> { expectedScientist1.Code, expectedScientist2.Code, expectedScientist3.Code };

        Assert.Equal(expectedIds, result.Select(r => r.Id.Value).ToList());
        Assert.Equal(expectedFullName, result.Select(r => r.FullName).ToList());
        Assert.Equal(expectedCode, result.Select(r => r.Code).ToList());
    }

    [Fact]
    public void Insert_Valid()
    {
        var expectedScientist = new Scientist(1, "SC001", "John Doe");
        var result = _gateway.Insert(expectedScientist);

        Assert.Equal(expectedScientist, result);
    }

    [Fact]
    public void InsertMulti_Valid()
    {
        var expectedScientists = new List<Scientist>
        {
            new Scientist(1, "SC001", "John Doe"),
            new Scientist(2, "SC002", "Jane Smith"),
            new Scientist(3, "SC003", "Michael Johnson"),
            new Scientist(4, "SC004", "Emily Davis"),
            new Scientist(5, "SC005", "Robert Wilson")
        };
        var result = _gateway.InsertMulti(expectedScientists);

        Assert.NotNull(result);
        Assert.Equal(expectedScientists.Count, result.Count());

        for (int i = 0; i < expectedScientists.Count; i++)
        {
            var expectedScientist = expectedScientists[i];
            var actualRemark = result.ElementAt(i);

            Assert.Equal(expectedScientist.Id, actualRemark.Id);
            Assert.Equal(expectedScientist.FullName, actualRemark.FullName);
            Assert.Equal(expectedScientist.Code, actualRemark.Code);
        }
    }

    [Fact]
    public void Update_Valid()
    {
        var existingScientist = new Scientist(1, "SC001", "John Doe");
        _gateway.Insert(existingScientist);
        var updatedScientist = new Scientist(1, "SC001", "Will Doe");
        var result = _gateway.Update(updatedScientist);

        Assert.NotNull(result);
        Assert.Equal(updatedScientist.Id, result.Id);
        Assert.Equal(updatedScientist.FullName, result.FullName);
        Assert.Equal(updatedScientist.Code, result.Code);

        var updatedScientitFromDB = _gateway.GetById(updatedScientist.Id.Value);

        Assert.NotNull(updatedScientitFromDB);
        Assert.Equal(updatedScientitFromDB.FullName, updatedScientitFromDB.FullName);
    }

    [Fact]
    public void UpdateMulti_Valid()
    {
        var scientistsToUpdate = new List<Scientist>
        {
            new Scientist(1, "SC001", "John Doe"),
            new Scientist(2, "SC002", "Jane Smith"),
            new Scientist(3, "SC003", "Michael Johnson"),
            new Scientist(4, "SC004", "Emily Davis"),
            new Scientist(5, "SC005", "Robert Wilson")
        };
        foreach (var scientist in scientistsToUpdate)
        {
            _gateway.Insert(scientist);
        }
        var newScientists = new List<Scientist>
        {
             new Scientist(1, "SC001", "Will Doe"),
            new Scientist(2, "SC002", "Jane Pots"),
            new Scientist(3, "SC003", "Frank Johnson"),
            new Scientist(4, "SC004", "Saven Davis"),
            new Scientist(5, "SC005", "Robert Boris")
        };
        var updatedScientists = _gateway.UpdateMulti(newScientists);

        Assert.NotNull(updatedScientists);
        Assert.Equal(scientistsToUpdate.Count(), updatedScientists.Count());
        foreach (var updatedScientist in updatedScientists)
        {
            var originalScientist = scientistsToUpdate.FirstOrDefault(s => s.Id == updatedScientist.Id);
            Assert.NotNull(originalScientist);
            Assert.Equal(originalScientist.Code, updatedScientist.Code);
            Assert.Equal(originalScientist.FullName, updatedScientist.FullName);
        }
        foreach (var updatedScientist in updatedScientists)
        {
            var scientistFromDB = _gateway.GetById(updatedScientist.Id.Value);
            Assert.NotNull(scientistFromDB);
            Assert.Equal(updatedScientist.Code, scientistFromDB.Code);
            Assert.Equal(updatedScientist.FullName, scientistFromDB.FullName);
        }
    }

    [Fact]
    public void Delete_Valid()
    {
        var scientistToDelete = new Scientist(1, "SC001", "Frank Green");
        _gateway.Insert(scientistToDelete);
        var deletedScientist = _gateway.Delete(scientistToDelete.Id.Value);

        Assert.NotNull(deletedScientist);
        Assert.Equal(scientistToDelete.Id, deletedScientist.Id);
        Assert.Equal(scientistToDelete.FullName, deletedScientist.FullName);
        Assert.Equal(scientistToDelete.Code, deletedScientist.Code);

        var scientistFromDb = _gateway.GetById(scientistToDelete.Id.Value);
        Assert.Null(scientistFromDb);
    }

    [Fact]
    public void DeleteMulti_Void()
    {
        var ids = new List<int> { 1, 2, 3 };
        var scientist1 = new Scientist(1, "SC001", "John Doe");
        var scientist2 = new Scientist(2, "SC002", "Jane Smith");
        var scientist3 = new Scientist(3, "SC003", "Michael Johnson");
        var scientist4 = new Scientist(4, "SC004", "Frank Johnson");
        var scientist5 = new Scientist(5, "SC005", "Jenny White");
        _gateway.Insert(scientist1);
        _gateway.Insert(scientist2);
        _gateway.Insert(scientist3);
        _gateway.Insert(scientist4);
        _gateway.Insert(scientist5);
        var result = _gateway.DeleteMulti(ids);
        var expected = new List<Scientist> { scientist1, scientist2, scientist3 };

        Assert.Equal(expected, result);
        foreach (var deletedscientist in expected)
        {
            var scientistFromDb = _gateway.GetById(deletedscientist.Id.Value);
            Assert.Null(scientistFromDb);
        }

        var notDeletedScientist = _gateway.GetById(scientist4.Id.Value);
        Assert.NotNull(notDeletedScientist);
        var notDeletedScientist2 = _gateway.GetById(scientist5.Id.Value);
        Assert.NotNull(notDeletedScientist2);
    }
}
