namespace ConcordiaDBLibrary.Gateways.Classes;

using Data;
using Models.Abstract;
using Models.Classes;
using Gateways.Abstract;
using Microsoft.EntityFrameworkCore;

public class ScientistsGateway : IEntityGateway<Scientist>
{
    private readonly ConcordiaContext _context;

    public ScientistsGateway(ConcordiaContext context)
    {
        _context = context;
    }

    public IEnumerable<Scientist> GetAll()
    {
        return _context.Scientists.AsNoTracking().ToList();
    }
       
    public Scientist? GetById(int id)
    {
        var scientis = _context.Scientists.SingleOrDefault(x => x.Id == id);
        return scientis;
    }
    public Scientist? GetByCode(string code)
    {
        return _context.Scientists.FirstOrDefault(x => x.Code == code);         
    }

    public IEnumerable<Scientist>? GetByIdMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any()) throw new Exception("No valid ids provided");       

        var scientists = new List<Scientist>();

        foreach (var id in ids)
        {
            var scientist = GetById(id);
            if (scientist != null)
            {
                scientists.Add(scientist);
            }
        }
        return scientists;
    }

    public Scientist Insert(Scientist entity)
    {
        var newScientist = _context.Scientists.Add(entity);
        _context.SaveChanges();
        return newScientist.Entity;
    }

    public IEnumerable<Scientist>? InsertMulti(IEnumerable<Scientist>? entities)
    {
        if (entities is null) throw new Exception("Null entities");          
        _context.Scientists.AddRange(entities);
        _context.SaveChanges();

        return entities;
    }


    public Scientist Update(Scientist entity)
    {
        if (entity.Id is null) throw new Exception("Null id");

        var intId = (int)entity.Id;
        var updateScientist = GetById(intId) ?? throw new Exception("Null entity");

        updateScientist.FullName = entity.FullName;
        updateScientist.Code = entity.Code;    
        _context.SaveChanges();
        return updateScientist;
    }

    public IEnumerable<Scientist>? UpdateMulti(IEnumerable<Scientist>? entities)
    {
        if (entities is null || !entities.Any()) throw new Exception("No entities provided");

        var scientists = new List<Scientist>();

        foreach (var entity in entities)
        {
            if (entity.Id is null) throw new Exception("Null id");
            var intId = (int)entity.Id;
            var updatedScientist = GetById(intId) ?? throw new Exception("Entity not found");

            updatedScientist.FullName = entity.FullName;
            updatedScientist.Code = entity.Code;
            scientists.Add(updatedScientist);
        }
        _context.SaveChanges();
        return scientists;
    }

    public Scientist Delete(int id)
    {
        var scientist = GetById(id) ?? throw new Exception("Insert valid id");

        var delScientist = _context.Scientists.Remove(scientist);
        _context.SaveChanges();
        return delScientist.Entity;
    }

    public IEnumerable<Scientist>? DeleteMulti(IEnumerable<int>? ids)
    {
        if(ids is null || !ids.Any()) throw new Exception("No valid ids provided");

        var deletedScientists = new List<Scientist>();

        foreach (var id in ids)
        {
            
            var scientist = GetById(id);
            if (scientist != null)
            {
                var delScientist = _context.Scientists.Remove(scientist);
                deletedScientists.Add(delScientist.Entity);
            }
        }
        _context.SaveChanges();
        return deletedScientists;
    }
}

