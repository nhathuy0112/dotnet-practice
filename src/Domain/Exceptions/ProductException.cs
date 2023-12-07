namespace Domain.Exceptions;

public class ProductException : Exception
{
    public ProductException(string? message) : base(message)
    {
    }
}