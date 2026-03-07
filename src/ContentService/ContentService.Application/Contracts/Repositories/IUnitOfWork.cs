namespace ContentService.Application.Contracts.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}