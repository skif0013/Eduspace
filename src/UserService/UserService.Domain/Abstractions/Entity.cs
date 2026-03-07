using System.ComponentModel.DataAnnotations;

namespace UserService.Domain.Abstractions;

public class Entity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}