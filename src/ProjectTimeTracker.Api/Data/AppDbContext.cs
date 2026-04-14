using Microsoft.EntityFrameworkCore;
using ProjectTimeTracker.Api.Models;

namespace ProjectTimeTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Utente> Utenti => Set<Utente>();
    public DbSet<Area> Aree => Set<Area>();
    public DbSet<Stato> Stati => Set<Stato>();
    public DbSet<Urgenza> Urgenze => Set<Urgenza>();
    public DbSet<Progetto> Progetti => Set<Progetto>();
    public DbSet<ProgettoDettaglio> ProgettoDettagli => Set<ProgettoDettaglio>();
    public DbSet<Cronologia> Cronologie => Set<Cronologia>();
    public DbSet<TempoLavorato> TempiLavorati => Set<TempoLavorato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Utente>(entity =>
        {
            entity.ToTable("Utenti");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Cognome).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.ToTable("Aree");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descrizione).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Stato>(entity =>
        {
            entity.ToTable("Stati");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descrizione).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Urgenza>(entity =>
        {
            entity.ToTable("Urgenze");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Descrizione).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Progetto>(entity =>
        {
            entity.ToTable("Progetti");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Oggetto).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Descrizione).HasMaxLength(2000);

            entity.HasOne(x => x.Area)
                .WithMany(x => x.Progetti)
                .HasForeignKey(x => x.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Stato)
                .WithMany(x => x.Progetti)
                .HasForeignKey(x => x.StatoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Urgenza)
                .WithMany(x => x.Progetti)
                .HasForeignKey(x => x.UrgenzaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProgettoDettaglio>(entity =>
        {
            entity.ToTable("ProgettoDettagli");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Dettaglio).HasMaxLength(2000).IsRequired();

            entity.HasOne(x => x.Progetto)
                .WithMany(x => x.Dettagli)
                .HasForeignKey(x => x.ProgettoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Utente)
                .WithMany(x => x.ProgettoDettagli)
                .HasForeignKey(x => x.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Cronologia>(entity =>
        {
            entity.ToTable("Cronologie");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Azione).HasMaxLength(200).IsRequired();

            entity.HasOne(x => x.Progetto)
                .WithMany(x => x.Cronologie)
                .HasForeignKey(x => x.ProgettoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Utente)
                .WithMany(x => x.Cronologie)
                .HasForeignKey(x => x.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TempoLavorato>(entity =>
        {
            entity.ToTable("TempiLavorati");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nota).HasMaxLength(1000);

            entity.HasOne(x => x.Progetto)
                .WithMany(x => x.TempiLavorati)
                .HasForeignKey(x => x.ProgettoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Utente)
                .WithMany(x => x.TempiLavorati)
                .HasForeignKey(x => x.UtenteId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}