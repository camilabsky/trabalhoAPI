using System.ComponentModel.DataAnnotations;

namespace Estoque.Models;

public class Produto{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; } = "";
    
    [Required(ErrorMessage = "Categoria é obrigatória")]
    [StringLength(50, ErrorMessage = "Categoria deve ter no máximo 50 caracteres")]
    public string Categoria { get; set; } = "";
    
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    
    [Required(ErrorMessage = "Quantidade é obrigatória")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser maior ou igual a 0")]
    public int Quantidade { get; set; } = 0;
    
    // Chave estrangeira
    [Required(ErrorMessage = "FornecedorId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "FornecedorId deve ser maior que 0")]
    public int FornecedorId { get; set; }
    
    // Propriedade de navegação
    public Fornecedor? Fornecedor { get; set; }
}