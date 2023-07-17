namespace ConcordiaDBLibrary.Gateways.Classes;

using Data;
using Models.Abstract;
using Models.Classes;
using Gateways.Abstract;
using Microsoft.EntityFrameworkCore;

public class RemarksGateway : IEntityGateway<Remark>
{
    private readonly ConcordiaContext _context;

    public RemarksGateway(ConcordiaContext context)
    {
        _context = context;
    }

    public IEnumerable<Remark> GetAll()
    {
        return _context.Remarks
            .Include(x => x.Experiment)
            .Include(x => x.Scientist)
            .AsNoTracking()
            .ToList();
    }

    public Remark? GetById(int id)
    {
        return _context.Remarks
            .Include(x => x.Experiment)
            .Include(x => x.Scientist)
            .SingleOrDefault(x => x.Id == id);
    }

    public IEnumerable<Remark>? GetByIdMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any()) throw new Exception("No valid ids provided");

        var remarks = new List<Remark>();

        foreach (var id in ids)
        {
            var scientist = GetById(id);
            if (scientist != null)
            {
                remarks.Add(scientist);
            }
        }
        return remarks;
    }

    public Remark Insert(Remark entity)
    {
         var newRemark = _context.Remarks.Add(entity);
        _context.SaveChanges();

        return newRemark.Entity;
    }

    public IEnumerable<Remark> InsertMulti(IEnumerable<Remark> entities)
    {
        if (entities is null) throw new Exception("No entities");
       
       
        _context.Remarks.AddRange(entities);
        _context.SaveChanges();
        
        return entities;
    }

    public Remark Update(Remark entity)
    {
        if (entity.Id is null)
            throw new ArgumentNullException(nameof(entity.Id), "Null Id");

        var intId = (int)entity.Id;
        var existingRemark = GetById(intId) ?? throw new Exception("Null Entity");

        existingRemark.Code = entity.Code;
        existingRemark.Text = entity.Text;
        existingRemark.Date = entity.Date;
        existingRemark.ExperimentId = entity.ExperimentId;
        existingRemark.ScientistId = entity.ScientistId;
        
        _context.SaveChanges();

        return existingRemark;
    }

    public IEnumerable<Remark> UpdateMulti(IEnumerable<Remark> entities)
    {
        if (entities is null || !entities.Any())
            throw new Exception("No entities provided");

        var updatedRemarks = new List<Remark>();

        foreach (var entity in entities)
        {
            if (entity.Id is null)
                throw new Exception("Null Id");

            var intId = (int)entity.Id;
            var existingRemark = GetById(intId) ?? throw new Exception("Entity not found");

            existingRemark.Code = entity.Code;
            existingRemark.Text = entity.Text;
            existingRemark.Date = entity.Date;
            existingRemark.ExperimentId = entity.ExperimentId;
            existingRemark.ScientistId = entity.ScientistId;
            
            updatedRemarks.Add(existingRemark);
        }
        _context.SaveChanges();
        return updatedRemarks;
    }

    public Remark Delete(int id)
    {
        var remark = GetById(id) ?? throw new Exception("Insert valid id");
        var delRemark = _context.Remarks.Remove(remark);
        _context.SaveChanges();
        return delRemark.Entity;
    }

    public IEnumerable<Remark>? DeleteMulti(IEnumerable<int>? ids)
    {
        if (ids is null || !ids.Any()) throw new Exception("No valid ids provided");

        var deletedRemarks = new List<Remark>();

        foreach (var id in ids)
        {            
            var remark = GetById(id);
            if (remark != null)
            {
                var delRemark = _context.Remarks.Remove(remark);
                deletedRemarks.Add(delRemark.Entity);
            }
        }
        _context.SaveChanges();
        return deletedRemarks;
    }
}