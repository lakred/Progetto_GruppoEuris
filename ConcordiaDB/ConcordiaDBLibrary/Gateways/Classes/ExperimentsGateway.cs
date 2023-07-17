namespace ConcordiaDBLibrary.Gateways.Classes;

using Data;
using Models.Abstract;
using Models.Classes;
using Gateways.Abstract;
using Microsoft.EntityFrameworkCore;

public class ExperimentsGateway : IEntityGateway<Experiment>
{
    private readonly ConcordiaContext _context;

    public ExperimentsGateway(ConcordiaContext context)
    {
        _context = context;
    }

    public IEnumerable<Experiment> GetAll()
    {
        return _context.Experiments
                   .Include(e => e.Priority)
                   .Include(e => e.State)
                   .Include(e => e.Participants)
                   .Include(e => e.Remarks)
                   .ToList();
    }

    public Experiment? GetById(int id)
    {
        var experiment = _context.Experiments
        .Include(e => e.Priority)
        .Include(e => e.State)
        .Include(e => e.Participants)
        .Include(e => e.Remarks)
        .FirstOrDefault(e => e.Id == id);
        return experiment;
    }

    public IEnumerable<Experiment>? GetByIdMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any())
        {
            return Enumerable.Empty<Experiment>();
        }
        var experiments = _context.Experiments
        .Include(e => e.Priority)
        .Include(e => e.State)
        .Include(e => e.Participants)
        .Include(e => e.Remarks)
        .Where(e => ids.Contains(e.Id.Value))
        .ToList();
        return experiments;
    }

    public Experiment Insert(Experiment entity)
    {
        _context.Experiments.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public IEnumerable<Experiment>? InsertMulti(IEnumerable<Experiment>? entities)
    {
        if (entities is null)
        {
            return null;
        }
        try
        {
            _context.Experiments.AddRange(entities);
            _context.SaveChanges();
            return entities;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while inserting multiple experiments: {e.Message}");
            return null;
        }
    }

    public Experiment Update(Experiment entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
            return entity;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while updating the experiment: {e.Message}");
            return null;
        }
    }

    public IEnumerable<Experiment>? UpdateMulti(IEnumerable<Experiment>? entities)
    {
        if (entities is null)
        {
            throw new ArgumentNullException(nameof(entities));
        }
        try
        {
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            _context.SaveChanges();
            return entities;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while updating experiments: {e.Message}");
            return null;
        }
    }

    public Experiment Delete(int id)
    {
        var experiment = _context.Experiments.Find(id);
        if (experiment is null)
        {
            throw new ArgumentNullException("The experiment with the specified id does not exist.");
        }
        try
        {
            _context.Experiments.Remove(experiment);
            _context.SaveChanges();
            return experiment;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while deleting the experiment: {e.Message}");
            return null;
        }
    }

    public IEnumerable<Experiment>? DeleteMulti(IEnumerable<int>? ids)
    {
        if (ids is null)
        {
            throw new ArgumentNullException(nameof(ids));
        }
        try
        {
            var experiments = _context.Experiments.Where(e => ids.Contains(e.Id.Value)).ToList();
            if (experiments.Count == 0)
            {
                throw new ArgumentNullException("No experiments with the specified ids exist.");
            }
            _context.Experiments.RemoveRange(experiments);
            _context.SaveChanges();
            return experiments;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred while deleting multiple experiments: {e.Message}");
            return null;
        }
    }
}