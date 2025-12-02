using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Estoque.Data;
using Estoque.Models;

namespace Estoque.Controllers;

[ApiController]
[Route("")]

public class ProductsController : ControllerBase{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db)=>_db =db;

    //GET /api/v1/products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> GetAll()
        => Ok(await _db.Produtos.OrderBy(s=>s.Id).ToListAsync());

    //GET /1(id)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Produto>> GetById(int id)
        => await _db.Produtos.FindAsync(id) is { } s ? Ok(s) : NotFound();

    //POST 
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Produto p){
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if(!string.IsNullOrWhiteSpace(p.Nome) &&
            await _db.Produtos.AnyAsync(x=>x.Nome == p.Nome)){
                return Conflict(new {error = "Produto já cadastrado"});
            }
        
        // Valida se o fornecedor existe
        if(!await _db.Fornecedores.AnyAsync(f => f.Id == p.FornecedorId)){
            return UnprocessableEntity(new {error = "Fornecedor não encontrado"});
        }

        _db.Produtos.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof (GetById), new {id = p.Id}, p);
    }

    //PUT /1(id)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Produto p){
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        p.Id = id;

         if(!string.IsNullOrWhiteSpace(p.Nome) &&
            await _db.Produtos.AnyAsync(x=>x.Nome == p.Nome && x.Id != id)){
                return Conflict(new {error = "Produto já cadastrado."});
            }

        if(!await _db.Produtos.AnyAsync(x=> x.Id == id)) return NotFound();

        // Valida se o fornecedor existe
        if(!await _db.Fornecedores.AnyAsync(f => f.Id == p.FornecedorId)){
            return UnprocessableEntity(new {error = "Fornecedor não encontrado"});
        }

        _db.Entry(p).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return Ok();
    } 

    // DELETE /1(id)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete (int id){
        var p = await _db.Produtos.FindAsync(id);
        if(p is null) return NotFound();

        _db.Produtos.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("com-fornecedor")]
    public async Task<ActionResult<IEnumerable<object>>> GetProdutosComFornecedor()
    {
        var produtos = await _db.Produtos
            .Include(p => p.Fornecedor)
            .Select(p => new {
                p.Id,
                p.Nome,
                p.Categoria,
                p.Quantidade,
                Fornecedor = new {
                    p.Fornecedor.Id,
                    p.Fornecedor.Nome,
                    p.Fornecedor.CNPJ
                }
            })
            .ToListAsync();
            
        return Ok(produtos);
    }
}