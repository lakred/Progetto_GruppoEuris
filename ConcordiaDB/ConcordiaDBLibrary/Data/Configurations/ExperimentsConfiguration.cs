namespace ConcordiaDBLibrary.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Abstract;
using Models.Classes;

public class ExperimentsConfiguration : IEntityTypeConfiguration<Experiment>
{
    public void Configure(EntityTypeBuilder<Experiment> entity)
    {
        entity.ToTable(nameof(ConcordiaContext.Experiments));

        entity.HasKey(e => e.Id).HasName($"PK_{nameof(ConcordiaContext.Experiments)}");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName(nameof(Entity.Id))
            .HasColumnType("int")
            .IsRequired(true);

        entity.Property(e => e.Code)
            .ValueGeneratedNever()
            .HasColumnName(nameof(TrelloEntity.Code))
            .HasMaxLength(24)
            .HasColumnType("varchar(24)")
            .IsRequired(false);

        entity.Property(e => e.Name)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Experiment.Name))
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .IsRequired(true);

        entity.Property(e => e.Description)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Experiment.Description))
            .HasMaxLength(int.MaxValue)
            .HasColumnType("varchar(MAX)")
            .IsRequired(true);

        entity.Property(e => e.StartDate)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Experiment.StartDate))
            .HasColumnType("datetimeoffset")
            .IsRequired(false);

        entity.Property(e => e.DueDate)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Experiment.DueDate))
            .HasColumnType("datetimeoffset")
            .IsRequired(false);

        entity.Property(e => e.PriorityId)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Experiment.PriorityId))
            .HasColumnType("int")
            .IsRequired(true);

        entity.Property(e => e.StateId)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Experiment.StateId))
            .HasColumnType("int")
            .IsRequired(true);

        entity.HasOne(d => d.Priority).WithMany(p => p.Experiments)
            .HasForeignKey(d => d.PriorityId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName($"FK_{nameof(ConcordiaContext.Experiments)}_{nameof(ConcordiaContext.Priorities)}");

        entity.HasOne(d => d.State).WithMany(p => p.Experiments)
            .HasForeignKey(d => d.StateId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName($"FK_{nameof(ConcordiaContext.Experiments)}_{nameof(ConcordiaContext.States)}");
    }
}