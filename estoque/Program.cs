using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Estoque.Data;
using Estoque.Models;

var builder = WebApplication.CreateBuilder(args);

// Porta fixa (opcional, facilita testes)
builder.WebHost.UseUrls("http://localhost:5099");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=estoque.db"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

var webTask = app.RunAsync();
Console.WriteLine("API online em http://localhost:5099 (Swagger em /swagger)");

Console.WriteLine("== EstoqueDbLab ==");
Console.WriteLine("Console + API executando juntos!");

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Escolha uma opção:");
    Console.WriteLine("1 - Cadastrar produto");
    Console.WriteLine("2 - Listar produtos");
    Console.WriteLine("3 - Atualizar produto (por Id)");
    Console.WriteLine("4 - Remover produto (por Id)");
    Console.WriteLine("0 - Sair");
    Console.Write("> ");

    var opt = Console.ReadLine();

    if (opt == "0") break;

    switch (opt)
    {
        case "1": await CreateStudentAsync(); break;
        case "2": await ListStudentsAsync(); break;
        case "3": await UpdateStudentAsync(); break;
        case "4": await DeleteStudentAsync(); break;
        default: Console.WriteLine("Opção inválida."); break;
    }
}

await app.StopAsync();
await webTask;

async Task CreateStudentAsync()
{
    Console.Write("Nome: ");
    var name = (Console.ReadLine() ?? "").Trim();

    Console.Write("Email: ");
    var email = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
    {
        Console.WriteLine("Nome e Email são obrigatórios.");
        return;
    }

    using var db = new AppDbContext();
    var exists = await db.Produtos.AnyAsync(s => s.Nome == name);
    if (exists) { Console.WriteLine("Já existe um produto com esse nome."); return; }

    var product = new Produto { Nome = name, Categoria = email, CriadoEm = DateTime.UtcNow };
    db.Produtos.Add(product);
    await db.SaveChangesAsync();
    Console.WriteLine($"Cadastrado com sucesso! Id: {product.Id}");
}

async Task ListProductsAsync()
{
    using var db = new AppDbContext();
    var products = await db.Produtos.OrderBy(s => s.Id).ToListAsync();

    if (products.Count == 0) { Console.WriteLine("Nenhum produto encontrado."); return; }

    Console.WriteLine("Id | Nome                 | Categoria               | CriadoEm (UTC)");
    foreach (var p in products)
        Console.WriteLine($"{p.Id,2} | {p.Nome,-20} | {p.Categoria,-24} | {p.CriadoEm:yyyy-MM-dd HH:mm:ss}");
}

async Task UpdateProductAsync()
{
    Console.Write("Informe o Id do produto a atualizar: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var product = await db.Produtos.FirstOrDefaultAsync(s => s.Id == id);
    if (product is null) { Console.WriteLine("Produto não encontrado."); return; }

    Console.WriteLine($"Atualizando Id {product.Id}. Deixe em branco para manter.");
    Console.WriteLine($"Nome atual : {product.Nome}");
    Console.Write("Novo nome  : ");
    var newName = (Console.ReadLine() ?? "").Trim();

    Console.WriteLine($"Categoria atual: {product.Categoria}");
    Console.Write("Nova categoria : ");
    var newEmail = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (!string.IsNullOrWhiteSpace(newName)) student.Name = newName;
    if (!string.IsNullOrWhiteSpace(newEmail))
    {
        var emailTaken = await db.Students.AnyAsync(s => s.Email == newEmail && s.Id != id);
        if (emailTaken) { Console.WriteLine("Já existe outro estudante com esse email."); return; }
        student.Email = newEmail;
    }

    await db.SaveChangesAsync();
    Console.WriteLine("Estudante atualizado com sucesso.");
}

async Task DeleteStudentAsync()
{
    Console.Write("Informe o Id do estudante a remover: ");
    if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("Id inválido."); return; }

    using var db = new AppDbContext();
    var student = await db.Students.FirstOrDefaultAsync(s => s.Id == id);
    if (student is null) { Console.WriteLine("Estudante não encontrado."); return; }

    db.Students.Remove(student);
    await db.SaveChangesAsync();
    Console.WriteLine("Estudante removido com sucesso.");
}