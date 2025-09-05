using System;
using System.Collections.Generic;
using System.Linq;

// Classes do modelo de dados para o sistema de pedidos.
public enum CustomerType { Standard, Premium, VIP }
public enum OrderStatus { Pending, Processing, Shipped, Delivered }

// Classe base para os produtos.
public abstract class Product
{
    public string Name { get; set; }
    public double Price { get; set; }
}

// Produto Físico (com peso).
public class PhysicalProduct : Product
{
    public double WeightInKg { get; set; }
}

// Produto Digital (com link de download).
public class DigitalProduct : Product
{
    public string DownloadLink { get; set; }
}

public class Order
{
    public string OrderId { get; set; }
    public CustomerType CustomerType { get; set; }
    public OrderStatus Status { get; set; }
    public double TotalValue { get; set; }
    public List<Product> Items { get; set; } = new List<Product>();
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("--- Sistema de Processamento de Pedidos ---");

        // Criação de um pedido de exemplo.
        var pedidoExemplo = new Order
        {
            OrderId = "P1001",
            CustomerType = CustomerType.Premium,
            Status = OrderStatus.Processing,
            TotalValue = 750.0,
            Items = new List<Product>
            {
                new PhysicalProduct { Name = "Smartphone", Price = 600.0, WeightInKg = 0.5 },
                new DigitalProduct { Name = "Ebook de C#", Price = 50.0, DownloadLink = "http://download.link/ebook" },
                new PhysicalProduct { Name = "Capa de Celular", Price = 100.0, WeightInKg = 0.1 }
            }
        };

        // --- Padrões de Relacionamento para Descontos ---
        Console.WriteLine("\nAnálise de Descontos:");
        string tipoDesconto = CalculateDiscountTier(pedidoExemplo.TotalValue);
        Console.WriteLine($"O pedido tem um valor de {pedidoExemplo.TotalValue:C} e se qualifica para: {tipoDesconto}");

        // --- Padrões de Tupla para Taxa de Envio ---
        Console.WriteLine("\nCálculo da Taxa de Envio:");
        double pesoTotal = pedidoExemplo.Items.OfType<PhysicalProduct>().Sum(p => p.WeightInKg);
        string destinoEnvio = "SP";
        double taxaEnvio = GetShippingFee(destinoEnvio, pesoTotal);
        Console.WriteLine($"O pedido para {destinoEnvio} com {pesoTotal} Kg tem uma taxa de envio de: {taxaEnvio:C}");

        // --- Padrões Lógicos para Bônus ---
        Console.WriteLine("\nVerificação de Bônus:");
        string bonus = GetOrderBonus(pedidoExemplo.CustomerType, pedidoExemplo.Status);
        Console.WriteLine($"O cliente ({pedidoExemplo.CustomerType}) e o status do pedido ({pedidoExemplo.Status}) resultam em: {bonus}");

        // --- Padrões de Tipo com Cláusula 'when' para Processamento de Itens ---
        Console.WriteLine("\nProcessamento de Itens:");
        foreach (var item in pedidoExemplo.Items)
        {
            ProcessItem(item);
        }
    }

    /// <summary>
    /// Calcula a categoria de desconto baseada no valor total do pedido, usando padrões de relacionamento.
    /// </summary>
    public static string CalculateDiscountTier(double orderTotal)
    {
        return orderTotal switch
        {
            < 100 => "Nenhum desconto",
            >= 100 and < 500 => "10% de desconto",
            >= 500 and < 1000 => "20% de desconto",
            _ => "Desconto VIP"
        };
    }

    /// <summary>
    /// Determina a taxa de envio usando padrões de tupla com o destino e o peso.
    /// </summary>
    public static double GetCustoEntrega(string destino, double pesoTotal)
    {
        return (destino, pesoTotal) switch
        {
            ("RJ", _) when pesoTotal < 1 => 5.0,
            ("RJ", _) => 10.0,
            ("SP", _) when pesoTotal < 1 => 7.0,
            ("SP", _) => 12.0,
            // Padrão com 'when' para qualquer outro destino com peso elevado
            (_, var weight) when weight > 5 => 30.0,
            _ => 15.0
        };
    }

    /// <summary>
    /// Determina um bônus para o pedido usando padrões lógicos com tipo de cliente e status.
    /// </summary>
    public static string GetOrderBonus(CustomerType customerType, OrderStatus status)
    {
        return (customerType, status) switch
        {
            (CustomerType.VIP or CustomerType.Premium, OrderStatus.Delivered) => "Bônus de fidelidade concedido!",
            (CustomerType.VIP, _) => "Bônus especial de boas-vindas!",
            _ => "Sem bônus aplicável."
        };
    }

    /// <summary>
    /// Processa um item do pedido usando padrões de tipo.
    /// </summary>
    /// 
    ///  ProcessItem(new DigitalProduct())
    public static void ProcessItem(Product item)
    {

        _ = item switch
        {
            // Padrão de tipo com 'is' e desconstrução implícita
            PhysicalProduct p when p.WeightInKg > 1 => $"📦 Processando produto físico pesado: {p.Name}. Peso: {p.WeightInKg} kg.",
            PhysicalProduct p => $"📦 Processando produto físico: {p.Name}.",
            // Padrão de tipo com 'is' para produto digital
            DigitalProduct d => $"💻 Processando produto digital: {d.Name}. Link: {d.DownloadLink}",
            _ => "Tipo de item desconhecido."
        };
        // A linha acima só tem o efeito colateral do Console.WriteLine.
    }
}
