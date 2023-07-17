namespace ConcordiaDBLibrary.Data.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Abstract;
using Models.Classes;

public class StatesConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> entity)
    {
        entity.ToTable(nameof(ConcordiaContext.States));

        entity.HasKey(e => e.Id).HasName($"PK_{nameof(ConcordiaContext.States)}");

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
            .HasColumnName(nameof(State.Name))
            .HasMaxLength(100)
            .HasColumnType("varchar(100)")
            .IsRequired(true);
    }
}