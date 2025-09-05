using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;

// 1. Nomenclatura vaga e inconsistente
// Classe em portugues, os atriutos devem seguir a classe
public class Tarefa
{
    public string Titulo { get; set; }
    public string Descricao { get; set; }
}

public class Program
{
    // 2. Lista Static Global
    // Deveria estar em uma classe própria
    // Se possivel, deveria ser um banco de dados - MSSQL, SQLITE

    private static List<Tarefa> tarefas = new List<Tarefa>();

    public static void Main(string[] a)
    {
        // 3. Falta injeção de dependecia
            // Falta uma Classe Service para fazer a Lógica

        var builder = WebApplication.CreateBuilder(a);
        var app = builder.Build();

        // 4. Falta lógica de negocio
        app.MapGet("/", () => "API de Tarefas, mas com código ruim.");

        // 5. Criação ou recepção de dados deve ser feita por um método HTTP POST
        // Parametros pouco descritivos
        // Post com parametros, é menos seguro que usar o body
        app.MapPost("/create", ([FromQuery] string titulo, [FromQuery] string descricao) =>
        {
            try
            {
                // 6. Não estamos validando os itens de entrada
                if(
                    titulo == null 
                    and descricao == null 
                    and titulo is not string
                    and descricao is not string
                ){
                    return Results.BadRequest("Parametros como null")
                }
                tarefas.Add(new Tarefa { Title = titulo, Description = descricao });
                
                // 7. I/O - Entrada e saida sincronizada, não precisa trava uma thead
                // Thread.Sleep(500); 

                return Results.Ok("Tarefa adicionada!");
            }
            // 8. Excessão genérica
            catch (System.Exception ex)
            {// 500 - um erro qualquer no servidor
                return Results.InternalServerError($"Erro! {ex.Message}"); // 8.1 Retorna 200 no erro
            }
        });

        // 9. Método errado, deveria ser um DELETE
        app.MapDelete("/remove/{id}", (int id) =>
        {
            try
            {
                tarefas.RemoveAt(id);
                return Results.NoContent("Tarefa removida, para id: {id}"); // Ok/200 ou NoContent/204
            }
            catch
            {
                // 10. Retorno errado para um erro
                Console.Write($"Erro ao remover tarefa ou índice inválido, para id: {id}") // apenas pessoal interno pode ver esse comando
                return Results.InternalServerError("Erro ao remover tarefa ou índice inválido"); // se for api de internet, qualquer pessoa pode ver 
                // return StatusCode(500, "mensagem") - asp.net
            }
        });

        // 11. Operacão post com o retorno errado
        app.MapPost("/api/tarefas", ([FromBody] TarefaRequest novaTarefa) =>
        {
            // 12. apenas valida o objeto todo, mas não valida os atributos
            // Deveriamso utilizar um DTO, ou uma classe de Request para receber dados
            // Dica de lib: FluentValidation
            if (novaTarefa == null)
            {
                return Results.Created("O objeto de tarefa não pode ser nulo.");
            }
            
            tarefas.Add(novaTarefa);
            return Results.Created(novaTarefa);
        });

        app.MapGet("/list", () =>
        {
            return Results.Ok(tarefas); // É bom usar uma classe de Response para limitar os dados expostos
        });

        app.Run();
    }
}
