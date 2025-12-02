using Microsoft.EntityFrameworkCore;
using Estoque.Models;

namespace Estoque.Data;

public class AppDbContext : DbContext{
    public AppDbContext(){}
    public AppDbContext(DbContextOptions<AppDbContext> options) :base(options){}

    public  DbSet<Produto> Produtos => Set<Produto>();
    public  DbSet<Fornecedor> Fornecedores => Set<Fornecedor>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=estoque.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fornecedor>(e =>{
            e.HasKey(f => f.Id);
            e.Property(f => f.Nome).IsRequired().HasMaxLength(120);
            e.Property(f => f.CNPJ).IsRequired().HasMaxLength(18);
            e.Property(f => f.Telefone).HasMaxLength(20);
        });

        modelBuilder.Entity<Produto>(e =>{
            e.HasKey(p => p.Id);
            e.Property(p => p.Nome).IsRequired().HasMaxLength(120);
            e.HasIndex(p => p.Nome).IsUnique();
            e.Property(p => p.Categoria).IsRequired().HasMaxLength(100);
            e.Property(p => p.CriadoEm).IsRequired();
            e.Property(p => p.Quantidade).IsRequired();
            
            // Configurar relacionamento
            e.HasOne(p => p.Fornecedor)
             .WithMany(f => f.Produtos)
             .HasForeignKey(p => p.FornecedorId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}