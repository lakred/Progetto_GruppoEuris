namespace ConcordiaDBLibrary.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ConcordiaDBLibrary;
using Models.Classes;
using Configurations;

public class ConcordiaContext : DbContext
{
    public DbSet<Experiment> Experiments { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<Priority> Priorities { get; set; } = null!;
    public DbSet<Remark> Remarks { get; set; } = null!;
    public DbSet<Scientist> Scientists { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;

    public ConcordiaContext(DbContextOptions<ConcordiaContext> options)
    : base(options)
    { }

    public ConcordiaContext()
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
	    optionsBuilder.UseSqlServer(DBSettings.GetConnectionString()); 
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ExperimentsConfiguration());
        modelBuilder.ApplyConfiguration(new ParticipantsConfiguration());
        modelBuilder.ApplyConfiguration(new PrioritiesConfiguration());
        modelBuilder.ApplyConfiguration(new RemarksConfiguration());
        modelBuilder.ApplyConfiguration(new ScientistsConfiguration());
        modelBuilder.ApplyConfiguration(new StatesConfiguration());
    }
}

// If You Use MacOS:
// ADD BEFORE THE INITIAL MIGRATION 
// REMOVE AFTER THE INITIAL MIGRATION

public class ConcordiaContextFactory : IDesignTimeDbContextFactory<ConcordiaContext>
{
    public ConcordiaContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConcordiaContext>();
        optionsBuilder.UseSqlServer(DBSettings.GetConnectionString());

        return new ConcordiaContext(optionsBuilder.Options);
    }
}

// NOTE FOR MIGRATION:
// ... ConcordiaDBLibrary % dotnet ef migrations add InitialMigration