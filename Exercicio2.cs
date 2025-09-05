using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

// 1. 
public class Tarefa
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class Program
{
    // 2. 
    private static List<Tarefa> tarefas = new List<Tarefa>();

    public static void Main(string[] a)
    {
        // 3. 
        var builder = WebApplication.CreateBuilder(a);
        var app = builder.Build();

        // 4. 
        app.MapGet("/", () => "API de Tarefas, mas com código ruim.");

        // 5. 
        app.MapGet("/create", ([FromQuery] string t, [FromQuery] string d) =>
        {
            try
            {
                // 6.
                tarefas.Add(new Tarefa { Title = t, Description = d });
                
                // 7. 
                Thread.Sleep(500); 

                return Results.Ok("Tarefa adicionada!");
            }
            // 8. 
            catch (System.Exception ex)
            {
                return Results.Ok($"Erro! {ex.Message}"); // 8.1
            }
        });

        // 9. 
        app.MapGet("/remove/{i}", (int i) =>
        {
            try
            {
                tarefas.RemoveAt(i);
                return Results.Ok("Tarefa removida.");
            }
            catch
            {
                // 10. 
                return Results.Ok("Erro ao remover tarefa ou índice inválido.");
            }
        });

        // 11. 
        app.MapPost("/api/tarefas", ([FromBody] Tarefa novaTarefa) =>
        {
            // 12. 
            if (novaTarefa == null)
            {
                return Results.Ok("O objeto de tarefa não pode ser nulo.");
            }
            
            tarefas.Add(novaTarefa);
            return Results.Ok(novaTarefa);
        });

        app.MapGet("/list", () =>
        {
            return Results.Ok(tarefas);
        });

        app.Run();
    }
}
