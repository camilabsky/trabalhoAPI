using System.ComponentModel.DataAnnotations;

namespace Estoque.Models;

public class Produto{
    public int Id { get; set; }
    
    //[Required(ErrorMessage = "Nome é obrigatório")]
    //[StringLength(120, ErrorMessage = "Nome deve ter no máximo 120 caracteres")]
    public string Name { get; set; } = "";
    
    //[Required(ErrorMessage = "Categoria é obrigatória")]
    //[StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
    public string Categoria { get; set; } = "";
    
    //[Required]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    
    //[Required]
    //[Range(0, int.MaxValue, ErrorMessage = "Quantidade deve ser maior ou igual a 0")]
    public int Quantidade { get; set; } = 0;
}