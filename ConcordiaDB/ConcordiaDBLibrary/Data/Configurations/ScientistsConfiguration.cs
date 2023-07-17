namespace ConcordiaDBLibrary.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Abstract;
using Models.Classes;

public class ScientistsConfiguration : IEntityTypeConfiguration<Scientist>
{
    public void Configure(EntityTypeBuilder<Scientist> entity)
    {
        entity.ToTable(nameof(ConcordiaContext.Scientists));

        entity.HasKey(e => e.Id).HasName($"PK_{nameof(ConcordiaContext.Scientists)}");

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

        entity.Property(e => e.FullName)
            .ValueGeneratedNever()
            .HasColumnName(nameof(Scientist.FullName))
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .IsRequired(true);
    }
}