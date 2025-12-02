using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Estoque.Data;
using Estoque.Models;

namespace Estoque.Controllers;

[ApiController]
[Route("fornecedores")]

public class FornecedorController : ControllerBase{
    private readonly AppDbContext _db;
    public FornecedorController(AppDbContext db)=>_db =db;

    //GET /api/v1/fornecedores
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Fornecedor>>> GetAll()
        => Ok(await _db.Fornecedores.OrderBy(s=>s.Id).ToListAsync());

    //GET /1(id)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Fornecedor>> GetById(int id)
        => await _db.Fornecedores.FindAsync(id) is { } s ? Ok(s) : NotFound();

    //POST 
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Fornecedor f){
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if(!string.IsNullOrWhiteSpace(f.Nome) &&
            await _db.Fornecedores.AnyAsync(x=>x.Nome == f.Nome)){
                return Conflict(new {error = "Fornecedor já cadastrado"});
            }
        
        // Valida CNPJ único
        if(!string.IsNullOrWhiteSpace(f.CNPJ) &&
            await _db.Fornecedores.AnyAsync(x=>x.CNPJ == f.CNPJ)){
                return Conflict(new {error = "CNPJ já cadastrado"});
            }

        _db.Fornecedores.Add(f);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof (GetById), new {id = f.Id}, f);
    }

    //PUT /1(id)
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Fornecedor f){
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        f.Id = id;

         if(!string.IsNullOrWhiteSpace(f.Nome) &&
            await _db.Fornecedores.AnyAsync(x=>x.Nome == f.Nome && x.Id != id)){
                return Conflict(new {error = "Fornecedor já cadastrado."});
            }
        
        // Valida CNPJ único
        if(!string.IsNullOrWhiteSpace(f.CNPJ) &&
            await _db.Fornecedores.AnyAsync(x=>x.CNPJ == f.CNPJ && x.Id != id)){
                return Conflict(new {error = "CNPJ já cadastrado"});
            }

        if(!await _db.Fornecedores.AnyAsync(x=> x.Id == id)) return NotFound();

        _db.Entry(f).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return Ok();
    } 

    // DELETE /1(id)
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete (int id){
        var f = await _db.Fornecedores.FindAsync(id);
        if(f is null) return NotFound();

        _db.Fornecedores.Remove(f);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}