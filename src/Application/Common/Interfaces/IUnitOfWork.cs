namespace Application.Common.Interfaces;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }
    IProductRepository ProductRepository { get; }
    ITokenRepository TokenRepository { get; }
    Task<int> CompleteAsync();
}