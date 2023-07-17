namespace ConcordiaDBLibrary.Gateways.Classes;

using Data;
using Models.Abstract;
using Models.Classes;
using Gateways.Abstract;
using Microsoft.EntityFrameworkCore;

public class PrioritiesGateway : IEntityGateway<Priority>
{
    private readonly ConcordiaContext _context;

    public PrioritiesGateway(ConcordiaContext context)
    {
        _context = context;
    }

    public IEnumerable<Priority> GetAll()
    {
        return _context.Priorities.Include(p => p.Experiments).ToList();
    }

    public Priority? GetById(int id)
    {
        return _context.Priorities.Include(p => p.Experiments).FirstOrDefault(p => p.Id == id);
    }

    public Priority? GetByCode(string code)
    {
        return _context.Priorities.FirstOrDefault(x => x.Code == code);
    }

    public IEnumerable<Priority>? GetByIdMulti(IEnumerable<int>? ids)
    {
        if (ids is null)
            return null;
        var nullableIds = ids.Select(id => (int?)id);
        return _context.Priorities.Where(p => nullableIds
                                  .Contains(p.Id))
                                  .Include(p => p.Experiments)
                                  .ToList();
    }

    public Priority Insert(Priority entity)
    {
        _context.Priorities.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public IEnumerable<Priority>? InsertMulti(IEnumerable<Priority>? entities)
    {
        if (entities is null)
            return null;
        _context.Priorities.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public Priority Update(Priority entity)
    {
        var priorityToUpdate = _context.Priorities.Include(p => p.Experiments)
                                                  .SingleOrDefault(p => p.Id == entity.Id);
        if (priorityToUpdate is not null)
        {
            priorityToUpdate.Code = entity.Code;
            priorityToUpdate.Name = entity.Name;
            priorityToUpdate.Color = entity.Color;
            _context.Priorities.Update(priorityToUpdate);
            _context.SaveChanges();
        }
        return priorityToUpdate;
    }

    public IEnumerable<Priority>? UpdateMulti(IEnumerable<Priority>? entities)
    {
        if (entities is null)
            return null;
        foreach (var entity in entities)
        {
            var priorityToUpdate = _context.Priorities.Include(p => p.Experiments)
                                                      .SingleOrDefault(p => p.Id == entity.Id);
            if (priorityToUpdate is not null)
            {
                priorityToUpdate.Code = entity.Code;
                priorityToUpdate.Name = entity.Name;
                priorityToUpdate.Color = entity.Color;
                _context.Priorities.Update(priorityToUpdate);
            }
        }
        _context.SaveChanges();
        return entities;
    }

    public Priority Delete(int id)
    {
        var priority = _context.Priorities.Find(id);
        if (priority is not null)
        {
            _context.Priorities.Remove(priority);
            _context.SaveChanges();
        }
        return priority;
    }

    public IEnumerable<Priority>? DeleteMulti(IEnumerable<int>? ids)
    {
        if (ids is null)
            return null;
        var nullableIds = ids.Select(id => (int?)id);
        var priorities = _context.Priorities.Where(p => nullableIds
                                            .Contains(p.Id))
                                            .ToList();
        if (priorities.Any())
        {
            _context.Priorities.RemoveRange(priorities);
            _context.SaveChanges();
        }
        return priorities;
    }
}