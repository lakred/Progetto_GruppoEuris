namespace ConcordiaDBLibrary.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Abstract;
using Models.Classes;

public class PrioritiesConfiguration : IEntityTypeConfiguration<Priority>
{
    public void Configure(EntityTypeBuilder<Priority> entity)
    {
        entity.ToTable(nameof(ConcordiaContext.Priorities));

        entity.HasKey(e => e.Id).HasName($"PK_{nameof(ConcordiaContext.Priorities)}");

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
            .HasColumnName(nameof(Priority.Name))
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .IsRequired(true);

        entity.Property(e => e.Color)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Priority.Color))
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .IsRequired(true);
    }
}