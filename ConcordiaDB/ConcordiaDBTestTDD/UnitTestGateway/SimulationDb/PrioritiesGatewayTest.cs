using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestTDD.UnitTestGateway.SimulationDb;

public class PrioritiesGatewayTest
{
    private ConcordiaContext _context;
    private PrioritiesGateway _prioritiesGateway;
    private List<Priority> _createdPriorities;

    public PrioritiesGatewayTest()
    {
        var serviceProvider = new ServiceCollection()
		        .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ConcordiaContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;
        _context = new ConcordiaContext(options);
        _prioritiesGateway = new PrioritiesGateway(_context);
        _createdPriorities = new List<Priority>();
    }

    private Priority TestSetupSingle()
    {
        var priority = new Priority(null, "code4", "namePriority4", "color4");
        _prioritiesGateway.Insert(priority);
        _createdPriorities.Add(priority);
        return priority;
    }

    private List<Priority> TestSetupMulti()
    {
        var priorities = new List<Priority>
        {
            new Priority(null, "code4", "namePriority5", "color4"),
            new Priority(null, "code5", "namePriority6", "color5"),
            new Priority(null, "code6", "namePriority7", "color6")
        };
        _prioritiesGateway.InsertMulti(priorities);
        _createdPriorities.AddRange(priorities);
        return priorities;
    }

    [Fact]
    public void GetAll_ReturnsAllPriorities()
    {
        var priority = TestSetupSingle();
        var result = _prioritiesGateway.GetAll();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public void GetById_ReturnsPriority()
    {
        var priority = TestSetupSingle();
        var result = _prioritiesGateway.GetById((int)priority.Id);
        Assert.NotNull(result);
        Assert.Equal(priority.Id, result.Id);
    }

    [Fact]
    public void GetByIdMulti_ReturnsPriorities()
    {
        var priorities = TestSetupMulti();
        var ids = priorities.Select(p => p.Id.Value).ToList();
        var result = _prioritiesGateway.GetByIdMulti(ids);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(ids.Count, result.Count());
    }

    [Fact]
    public void Insert_ReturnsInsertedPriority()
    {
        var priority = new Priority(null, "code4", "namePriority4", "color4");
        Priority result = _prioritiesGateway.Insert(priority);
        _createdPriorities.Add(result);
        Assert.NotNull(result);
        Assert.Equal(priority.Code, result.Code);
        Assert.Equal(priority.Name, result.Name);
        Assert.Equal(priority.Color, result.Color);
    }

    [Fact]
    public void InsertMulti_ReturnsInsertedPriorities()
    {
        var priorities = new List<Priority>
    {
        new Priority(null, "code4", "namePriority5", "color4"),
        new Priority(null, "code5", "namePriority6", "color5"),
        new Priority(null, "code6", "namePriority7", "color6")
    };
        var result = _prioritiesGateway.InsertMulti(priorities).ToList();
        _createdPriorities.AddRange(result);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(priorities.Count, result.Count());
    }

    [Fact]
    public void Update_ReturnsUpdatedPriority()
    {
        var priority = TestSetupSingle();
        var result = _prioritiesGateway.Update(priority);
        Assert.NotNull(result);
        Assert.Equal(priority.Id, result.Id);
        Assert.Equal(priority.Code, result.Code);
        Assert.Equal(priority.Name, result.Name);
        Assert.Equal(priority.Color, result.Color);
    }

    [Fact]
    public void UpdateMulti_ReturnsUpdatedPriorities()
    {
        var priorities = TestSetupMulti();
        var result = _prioritiesGateway.UpdateMulti(priorities);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(priorities.Count, result.Count());
    }

    [Fact]
    public void Delete_ReturnsDeletedPriority()
    {
        var priority = TestSetupSingle();
        var deletedPriority = _prioritiesGateway.Delete((int)priority.Id);
        Assert.NotNull(deletedPriority);
        Assert.Equal(priority.Id, deletedPriority.Id);
        Assert.Equal(priority.Code, deletedPriority.Code);
        Assert.Equal(priority.Name, deletedPriority.Name);
        Assert.Equal(priority.Color, deletedPriority.Color);
        Assert.Null(_context.Priorities.Find(priority.Id));
        _createdPriorities.Remove(priority);
    }

    [Fact]
    public void DeleteMulti_ReturnsDeletedPriorities()
    {
        var priorities = TestSetupMulti();
        var ids = priorities.Select(p => p.Id.Value).ToList();
        var deletedPriorities = _prioritiesGateway.DeleteMulti(ids);
        Assert.NotNull(deletedPriorities);
        Assert.NotEmpty(deletedPriorities);
        Assert.Equal(ids.Count, deletedPriorities.Count());
        foreach (var deletedPriority in deletedPriorities)
        {
            Assert.True(ids.Contains((int)deletedPriority.Id));
        }
        foreach (var id in ids)
        {
            var priority = _context.Priorities.Find(id);
            Assert.Null(priority);
        }
        _createdPriorities.RemoveAll(p => ids.Contains(p.Id.Value));
    }
}