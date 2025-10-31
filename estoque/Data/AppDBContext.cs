using Microsoft.EntityFrameworkCore;
using Estoque.Models;

namespace Estoque.Data;

public class AppDbContext : DbContext{
    public AppDbContext(){}
    public AppDbContext(DbContextOptions<AppDbContext> options) :base(options){}

    public  DbSet<Produto> Produtos => Set<Produto>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=estoque.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Produto>(e =>{
            e.HasKey(p=>p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(120);
            e.HasIndex(p => p.Name).IsUnique();
            e.Property(p=>p.Categoria).IsRequired().HasMaxLength(100);
            e.Property(p=>p.CriadoEm).IsRequired();
            e.Property(p=>p.Quantidade).IsRequired();
        });
    }
}