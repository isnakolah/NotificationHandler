namespace WebApplication1.Models;

public record Invoice
{
    public string InvoiceNo { get; init; } = string.Empty;
    public double Total { get; init; }
}