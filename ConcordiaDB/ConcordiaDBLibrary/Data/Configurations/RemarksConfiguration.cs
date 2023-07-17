namespace ConcordiaDBLibrary.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Abstract;
using Models.Classes;

public class RemarksConfiguration : IEntityTypeConfiguration<Remark>
{
    public void Configure(EntityTypeBuilder<Remark> entity)
    {
        entity.ToTable(nameof(ConcordiaContext.Remarks));

        entity.HasKey(e => e.Id).HasName($"PK_{nameof(ConcordiaContext.Remarks)}");

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

        entity.Property(e => e.Text)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Remark.Text))
            .HasMaxLength(int.MaxValue)
            .HasColumnType("varchar(MAX)")
            .IsRequired(true);

        entity.Property(e => e.Date)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Remark.Date))
            .HasColumnType("datetimeoffset")
            .IsRequired(false);

        entity.Property(e => e.ExperimentId)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Remark.ExperimentId))
            .HasColumnType("int")
            .IsRequired(true);

        entity.Property(e => e.ScientistId)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Remark.ScientistId))
            .HasColumnType("int")
            .IsRequired(true);

        entity.HasOne(d => d.Experiment).WithMany(p => p.Remarks)
            .HasForeignKey(d => d.ExperimentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName($"FK_{nameof(ConcordiaContext.Remarks)}_{nameof(ConcordiaContext.Experiments)}");

        entity.HasOne(d => d.Scientist).WithMany(p => p.Remarks)
            .HasForeignKey(d => d.ScientistId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName($"FK_{nameof(ConcordiaContext.Remarks)}_{nameof(ConcordiaContext.Scientists)}");
    }
}