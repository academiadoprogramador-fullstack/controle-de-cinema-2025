using ControledeCinema.Dominio.Compartilhado;
using ControleDeCinema.Dominio.ModuloAutenticacao;
using ControleDeCinema.Dominio.ModuloGeneroFilme;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ControleDeCinema.Infraestrutura.Orm.Compartilhado;

public class ControleDeCinemaDbContext : IdentityDbContext<Usuario, Cargo, Guid>, IUnitOfWork
{
    public DbSet<GeneroFilme> GenerosFilme { get; set; }

    private readonly ITenantProvider? tenantProvider;

    public ControleDeCinemaDbContext(DbContextOptions options, ITenantProvider? tenantProvider = null) : base(options)
    {
        this.tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (tenantProvider is not null)
        {
            modelBuilder.Entity<GeneroFilme>()
                .HasQueryFilter(x => x.UsuarioId.Equals(tenantProvider.UsuarioId));
        }

        var assembly = typeof(ControleDeCinemaDbContext).Assembly;

        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        base.OnModelCreating(modelBuilder);
    }

    public void Commit()
    {
        SaveChanges();
    }

    public void Rollback()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }
    }
}
