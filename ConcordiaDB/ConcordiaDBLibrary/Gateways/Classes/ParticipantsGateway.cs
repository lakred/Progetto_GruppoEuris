namespace ConcordiaDBLibrary.Gateways.Classes;

using Data;
using Models.Abstract;
using Models.Classes;
using Gateways.Abstract;
using Microsoft.EntityFrameworkCore;

public class ParticipantsGateway : IEntityGateway<Participant>
{
    private readonly ConcordiaContext _context;

    public ParticipantsGateway(ConcordiaContext context)
    {
        _context = context;
    }

    public IEnumerable<Participant> GetAll()
    {
        return _context.Participants
           .Include(p => p.Experiment)
           .Include(p => p.Scientist)
           .ToList();
    }

    public Participant? GetById(int id)
    {
        return _context.Participants
            .Include(p => p.Experiment)
            .Include(p => p.Scientist)
            .SingleOrDefault(p => p.Id == id);
    }

    public IEnumerable<Participant>? GetByIdMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any())
            return null;
        var validIds = ids.OfType<int>().ToList();
        return _context.Participants
            .Where(p => validIds.Contains(p.Id.Value))
            .Include(p => p.Experiment)
            .Include(p => p.Scientist)
            .ToList();
    }

    public Participant Insert(Participant entity)
    {
        _context.Participants.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public IEnumerable<Participant>? InsertMulti(IEnumerable<Participant>? entities)
    {
        if(entities is null)
            return null;
        _context.Participants.AddRange(entities);
        _context.SaveChanges();
        return entities;
    }

    public Participant Update(Participant entity)
    {
        _context.Participants.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public IEnumerable<Participant>? UpdateMulti(IEnumerable<Participant>? entities)
    {
        if(entities is null)
            return null;
        _context.Participants.UpdateRange(entities);
        _context.SaveChanges();
        return entities;

    }

    public Participant Delete(int id)
    {
        var participant = _context.Participants.Find(id) ?? throw new ArgumentException("No participant found with the provided id.");
        _context.Participants.Remove(participant);
        _context.SaveChanges();
        return participant;
    }

    public IEnumerable<Participant>? DeleteMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any())
            return null;
        var validIds = ids.OfType<int>().ToList(); 
        var participants = _context.Participants.Where(p => p.Id.HasValue && validIds.Contains(p.Id.Value))
                                                .ToList();
        if (!participants.Any())
        {
            throw new ArgumentException("No participants found with the provided ids.");
        }
        _context.Participants.RemoveRange(participants);
        _context.SaveChanges();
        return participants;
    }
}