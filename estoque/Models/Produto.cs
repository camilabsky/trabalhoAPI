using System.ComponentModel.DataAnnotations;

namespace Estoque.Models;

public class Produto{
    public int Id { get; set; }
    
    public string Nome { get; set; } = "";
    
    public string Categoria { get; set; } = "";
    
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    
    public int Quantidade { get; set; } = 0;
    
    // Chave estrangeira
    public int FornecedorId { get; set; }
    
    // Propriedade de navegação
    public Fornecedor Fornecedor { get; set; } = null!;
}