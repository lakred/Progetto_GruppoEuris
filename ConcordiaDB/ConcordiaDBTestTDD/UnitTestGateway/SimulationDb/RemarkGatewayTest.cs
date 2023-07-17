using ConcordiaDBLibrary.Data;
using ConcordiaDBLibrary.Gateways.Classes;
using ConcordiaDBLibrary.Models.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestTDD.UnitTestGateway.SimulationDb;

public class RemarkGatewayTest
{
    private ConcordiaContext _context;
    private RemarksGateway _gateway;
    public RemarkGatewayTest()
    {
        var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();
        var options = new DbContextOptionsBuilder<ConcordiaContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceProvider)
            .Options;
        _context = new ConcordiaContext(options);
        _gateway = new RemarksGateway(_context);
    }

    [Fact]
    public void GetAll_Valid()
    {
        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        _gateway.Insert(new Remark(1, "R001", "First remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value));
        _gateway.Insert(new Remark(2, "R002", "Second remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value));
        _gateway.Insert(new Remark(3, "R003", "Third remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value));
        _gateway.Insert(new Remark(4, "R004", "Fourth remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value));
        _gateway.Insert(new Remark(5, "R005", "Fifth remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value));


        var result = _gateway.GetAll();

        Assert.Equal(5, result.Count());
    }

    [Fact]
    public void GetById_Valid()
    {
        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        var remarkId = 3;
        var expectedRemark = new Remark(remarkId, "R003", "Third remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        _gateway.Insert(expectedRemark);
        var result = _gateway.GetById(remarkId);

        Assert.Equal(expectedRemark.Id, result.Id);
        Assert.Equal(expectedRemark.Code, result.Code);
        Assert.Equal(expectedRemark.Text, result.Text);
        Assert.Equal(expectedRemark.Date, result.Date);
        Assert.Equal(expectedRemark.ScientistId, result.ScientistId);
        Assert.Equal(expectedRemark.ExperimentId, result.ExperimentId);
    }

    [Fact]
    public void GetByIdMulti_Valid()
    {
        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        var ids = new List<int> { 1, 2, 3 };
        var expectedRemark1 = new Remark(1, "R001", "First remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var expectedRemark2 = new Remark(2, "R002", "Second remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var expectedRemark3 = new Remark(3, "R003", "Third remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        _gateway.Insert(expectedRemark1);
        _gateway.Insert(expectedRemark2);
        _gateway.Insert(expectedRemark3);
        var result = _gateway.GetByIdMulti(ids);
        var expectedIds = new List<int> { expectedRemark1.Id.Value, expectedRemark2.Id.Value, expectedRemark3.Id.Value };
        var expectedCodes = new List<string> { expectedRemark1.Code, expectedRemark2.Code, expectedRemark3.Code };

        Assert.Equal(expectedIds, result.Select(r => r.Id.Value).ToList());
        Assert.Equal(expectedCodes, result.Select(r => r.Code).ToList());
    }

    [Fact]
    public void Insert_Valid()
    {
        var expectedRemark = new Remark(1, "R001", "First remark", DateTimeOffset.Now, 1, 1);
        var result = _gateway.Insert(expectedRemark);

        Assert.Equal(expectedRemark, result);
    }

    [Fact]
    public void InsertMulti_Valid()
    {
        var expectedRemarks = new List<Remark>
        {
            new Remark(1, "R001", "First remark", DateTimeOffset.Now, 1, 1),
            new Remark(2, "R002", "Second remark", DateTimeOffset.Now, 2, 2),
            new Remark(3, "R003", "Third remark", DateTimeOffset.Now, 3, 3),
            new Remark(4, "R004", "Fourth remark", DateTimeOffset.Now, 4, 4),
            new Remark(5, "R005", "Fifth remark", DateTimeOffset.Now, 5, 5)
        };
        var result = _gateway.InsertMulti(expectedRemarks);

        Assert.NotNull(result);
        Assert.Equal(expectedRemarks.Count(), result.Count());

        for (int i = 0; i < expectedRemarks.Count(); i++)
        {
            var expectedRemark = expectedRemarks[i];
            var actualRemark = result.ElementAt(i);

            Assert.Equal(expectedRemark.Id, actualRemark.Id);
            Assert.Equal(expectedRemark.Code, actualRemark.Code);
            Assert.Equal(expectedRemark.Text, actualRemark.Text);
            Assert.Equal(expectedRemark.Date, actualRemark.Date);
        }
    }

    [Fact]
    public void Update_Valid()
    {
        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        var existingRemark = new Remark(10, "R001", "Old remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        _gateway.Insert(existingRemark);

        var updatedRemark = new Remark(10, "R001", "Updated remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var result = _gateway.Update(updatedRemark);

        Assert.NotNull(result);
        Assert.Equal(updatedRemark.Id, result.Id);
        Assert.Equal(updatedRemark.Code, result.Code);
        Assert.Equal(updatedRemark.Text, result.Text);
        Assert.Equal(updatedRemark.Date, result.Date);
        //Assert.Equal(updatedRemark.Scientist, result.Scientist);
        //Assert.Equal(updatedRemark.Experiment, result.Experiment);
        //Assert.Equal(updatedRemark.ExperimentId, result.ExperimentId);
        //Assert.Equal(updatedRemark.ScientistId, result.ScientistId);

        var updatedRemarkFromDatabase = _gateway.GetById(updatedRemark.Id.Value);

        Assert.NotNull(updatedRemarkFromDatabase);
        Assert.Equal(updatedRemark.Id, updatedRemarkFromDatabase.Id);
        Assert.Equal(updatedRemark.Code, updatedRemarkFromDatabase.Code);
        Assert.Equal(updatedRemark.Text, updatedRemarkFromDatabase.Text);
        Assert.Equal(updatedRemark.Date, updatedRemarkFromDatabase.Date);
        //Assert.Equal(updatedRemark.Scientist, updatedRemarkFromDatabase.Scientist);
        //Assert.Equal(updatedRemark.Experiment, updatedRemarkFromDatabase.Experiment);
        //Assert.Equal(updatedRemark.ExperimentId, updatedRemarkFromDatabase.ExperimentId);
        //Assert.Equal(updatedRemark.ScientistId, updatedRemarkFromDatabase.ScientistId);
    }

    [Fact]
    public void UpdateMulti_Valid()
    {
        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        var remarksToUpdate = new List<Remark>
            {
                new Remark(1, "R001", "Old remark 1", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value),
                new Remark(2, "R002", "Old remark 2", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value),
                new Remark(3, "R003", "Old remark 3", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value)
            };

        _gateway.InsertMulti(remarksToUpdate);

        var newRemarks = new List<Remark>
        {
            new Remark(1, "R001", "Updated remark 1", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value),
            new Remark(2, "R002", "Updated remark 2", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value),
            new Remark(3, "R003", "Updated remark 3", DateTimeOffset.Now,experiment.Id.Value, scientist.Id.Value)

        };

        var updatedRemarks = _gateway.UpdateMulti(newRemarks);

        Assert.NotNull(updatedRemarks);
        Assert.Equal(remarksToUpdate.Count(), updatedRemarks.Count());

        foreach (var updatedRemark in updatedRemarks)
        {
            var originalRemark = remarksToUpdate.FirstOrDefault(r => r.Id == updatedRemark.Id);
            Assert.NotNull(originalRemark);
            Assert.Equal(originalRemark.Code, updatedRemark.Code);
            Assert.Equal(originalRemark.Text, updatedRemark.Text);
            Assert.Equal(originalRemark.Date, updatedRemark.Date);
            Assert.Equal(originalRemark.Scientist, updatedRemark.Scientist);
            Assert.Equal(originalRemark.Experiment, updatedRemark.Experiment);
            Assert.Equal(originalRemark.ExperimentId, updatedRemark.ExperimentId);
            Assert.Equal(originalRemark.ScientistId, updatedRemark.ScientistId);
        }
        foreach (var updatedRemark in updatedRemarks)
        {
            var remarkFromDatabase = _gateway.GetById(updatedRemark.Id.Value);
            Assert.NotNull(remarkFromDatabase);
            Assert.Equal(updatedRemark.Code, remarkFromDatabase.Code);
            Assert.Equal(updatedRemark.Text, remarkFromDatabase.Text);
            Assert.Equal(updatedRemark.Date, remarkFromDatabase.Date);
            Assert.Equal(updatedRemark.Scientist, remarkFromDatabase.Scientist);
            Assert.Equal(updatedRemark.Experiment, remarkFromDatabase.Experiment);
            Assert.Equal(updatedRemark.ExperimentId, remarkFromDatabase.ExperimentId);
            Assert.Equal(updatedRemark.ScientistId, remarkFromDatabase.ScientistId);
        }
    }

    [Fact]
    public void Delete_Valid()
    {
        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        var remarkToDelete = new Remark(1, "R001", "Remark to delete", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        _gateway.Insert(remarkToDelete);
        var deletedRemark = _gateway.Delete(remarkToDelete.Id.Value);

        Assert.NotNull(deletedRemark);
        Assert.Equal(remarkToDelete.Id, deletedRemark.Id);
        Assert.Equal(remarkToDelete.Code, deletedRemark.Code);
        Assert.Equal(remarkToDelete.Text, deletedRemark.Text);
        Assert.Equal(remarkToDelete.Date, deletedRemark.Date);
        Assert.Equal(remarkToDelete.ExperimentId, deletedRemark.ExperimentId);
        Assert.Equal(remarkToDelete.ScientistId, deletedRemark.ScientistId);

        var remarkFromDatabase = _gateway.GetById(remarkToDelete.Id.Value);
        Assert.Null(remarkFromDatabase);
    }

    [Fact]
    public void DeleteMulti_Valid()
    {

        var experiment = new Experiment(1, "E001", "Test experiment", "Description", true, DateTimeOffset.Now, null, 1, 1);
        var scientist = new Scientist(1, "S001", "John Doe");

        _context.Experiments.Add(experiment);
        _context.Scientists.Add(scientist);
        _context.SaveChanges();

        var ids = new List<int> { 1, 2, 3 };
        var expectedRemark1 = new Remark(1, "R001", "First remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var expectedRemark2 = new Remark(2, "R002", "Second remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var expectedRemark3 = new Remark(3, "R003", "Third remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var expectedRemark4 = new Remark(4, "R004", "Fourth remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        var expectedRemark5 = new Remark(5, "R005", "Fifth remark", DateTimeOffset.Now, experiment.Id.Value, scientist.Id.Value);
        _gateway.Insert(expectedRemark1);
        _gateway.Insert(expectedRemark2);
        _gateway.Insert(expectedRemark3);
        _gateway.Insert(expectedRemark4);
        _gateway.Insert(expectedRemark5);
        var result = _gateway.DeleteMulti(ids);
        var expected = new List<Remark> { expectedRemark1, expectedRemark2, expectedRemark3 };

        Assert.Equal(expected, result);
        foreach (var deletedRemark in expected)
        {
            var remarkFromDatabase = _gateway.GetById(deletedRemark.Id.Value);
            Assert.Null(remarkFromDatabase);
        }
        var notDeletedRemark = _gateway.GetById(expectedRemark4.Id.Value);
        Assert.NotNull(notDeletedRemark);
    }
}




