namespace ConcordiaDBLibrary.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Abstract;
using Models.Classes;

public class ParticipantsConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> entity)
    {
        entity.ToTable(nameof(ConcordiaContext.Participants));

        entity.HasKey(e => e.Id).HasName($"PK_{nameof(ConcordiaContext.Participants)}");

        entity.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName(nameof(Entity.Id))
            .HasColumnType("int")
            .IsRequired(true);

        entity.Property(e => e.ExperimentId)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Participant.ExperimentId))
            .HasColumnType("int")
            .IsRequired(true);

        entity.Property(e => e.ScientistId)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Participant.ScientistId))
            .HasColumnType("int")
            .IsRequired(false);

        entity.HasOne(d => d.Experiment).WithMany(p => p.Participants)
            .HasForeignKey(d => d.ExperimentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName($"FK_{nameof(ConcordiaContext.Participants)}_{nameof(ConcordiaContext.Experiments)}");

        entity.HasOne(d => d.Scientist).WithMany(p => p.Participants)
            .HasForeignKey(d => d.ScientistId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName($"FK_{nameof(ConcordiaContext.Participants)}_{nameof(ConcordiaContext.Scientists)}");
    }
}

