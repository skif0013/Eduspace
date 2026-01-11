namespace FileService.Application.Contracts.Repositories;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}