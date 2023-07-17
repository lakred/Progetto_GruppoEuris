namespace ConcordiaDBLibrary.Gateways.Classes;

using Data;
using Models.Abstract;
using Models.Classes;
using Gateways.Abstract;
using Microsoft.EntityFrameworkCore;

public class StatesGateway : IEntityGateway<State>
{
    private readonly ConcordiaContext _context;

    public StatesGateway(ConcordiaContext context)
    {
        _context = context;
    }

    public IEnumerable<State> GetAll()
    {
        return _context.States.AsNoTracking().ToList();        
    }

    public State? GetById(int id)
    {
        return _context.States.SingleOrDefault(x => x.Id == id); 
    }

    public State? GetByCode(string code)
    {
        return _context.States.FirstOrDefault(x => x.Code == code);         
    }

    public IEnumerable<State>? GetByIdMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any()) throw new Exception("No valid ids provided");

        var states = new List<State>();

        foreach (var id in ids)
        {
            var state = GetById(id);
            if (state != null)
            {
                states.Add(state);
            }
        }
        return states;
    }

    public State Insert(State entity)
    {       
        var newState = _context.States.Add(entity);
        _context.SaveChanges();
        return newState.Entity;
    }

    public IEnumerable<State>? InsertMulti(IEnumerable<State>? entities)
    {
        if (entities is null) throw new Exception("Null entities");

        _context.States.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public State Update(State entity)
    {
        if (entity.Id is null) throw new Exception("Null id");

        var intId = (int)entity.Id;
        var stateUpdated = GetById(intId) ?? throw new Exception("Null entity");

        stateUpdated.Name = entity.Name;
        stateUpdated.Code = entity.Code;
        _context.SaveChanges();
        return stateUpdated;
    }

    public IEnumerable<State>? UpdateMulti(IEnumerable<State>? entities)
    {
        if (entities is null || !entities.Any()) throw new Exception("No entities provided");

        var states = new List<State>();

        foreach (var entity in entities)
        {
            if (entity.Id is null) throw new Exception("Null id");
            var intId = (int)entity.Id;
            var updateState = GetById(intId) ?? throw new Exception("Entity not found");

            updateState.Name = entity.Name;
            updateState.Code = entity.Code;

            states.Add(updateState);
        }
        _context.SaveChanges();
        return states;
    }

    public State Delete(int id)
    {
        var state = GetById(id) ?? throw new Exception("Insert valid id");

        var delState = _context.States.Remove(state);
        _context.SaveChanges();
        return delState.Entity;
    }

    public IEnumerable<State>? DeleteMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any()) throw new Exception("No valid ids provided");

        var states = new List<State>();

        foreach(var id in ids)
        {
            var state = GetById(id);
            if(state != null)
            {
                var delState = _context.States.Remove(state);
                states.Add(delState.Entity);

            }
        }
            _context.SaveChanges();
            return states;        
    }
}