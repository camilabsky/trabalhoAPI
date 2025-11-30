using System.ComponentModel.DataAnnotations;

namespace Estoque.Models;

public class Fornecedor
{
    public int Id { get; set; }
    
    public string Nome { get; set; } = "";
    
    public string CNPJ { get; set; } = "";
    
    public string Telefone { get; set; } = "";
    
    // Relacionamento 1:N (Um fornecedor pode ter vários produtos)
    public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
}