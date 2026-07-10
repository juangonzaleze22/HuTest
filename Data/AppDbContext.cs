using HuTest.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HuTest.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<BloqueoCuenta> Bloqueos => Set<BloqueoCuenta>();
    public DbSet<TokenActivacion> TokensActivacion => Set<TokenActivacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(e =>
        {
            e.HasIndex(u => new { u.Documento, u.TipoDocumento }).IsUnique();
            e.HasMany(u => u.Bloqueos)
                .WithOne(b => b.Usuario!)
                .HasForeignKey(b => b.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasMany(u => u.TokensActivacion)
                .WithOne(t => t.Usuario!)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TokenActivacion>()
            .HasIndex(t => t.Token)
            .IsUnique();
    }
}
