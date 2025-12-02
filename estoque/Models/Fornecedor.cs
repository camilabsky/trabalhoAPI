using System.ComponentModel.DataAnnotations;

namespace Estoque.Models;

public class Fornecedor
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 100 caracteres")]
    public string Nome { get; set; } = "";
    
    [Required(ErrorMessage = "CNPJ é obrigatório")]
    [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter entre 14 e 18 caracteres")]
    public string CNPJ { get; set; } = "";
    
    [Required(ErrorMessage = "Telefone é obrigatório")]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 20 caracteres")]
    public string Telefone { get; set; } = "";
    
    // Relacionamento 1:N (Um fornecedor pode ter vários produtos)
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}