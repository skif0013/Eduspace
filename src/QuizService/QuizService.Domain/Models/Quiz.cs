namespace QuizService.Domain.Models;

public class Quiz
{
    private readonly List<Question> _questions = new();
    public Quiz(Guid creatorId, string name, string? description,double passPercentage)
    {
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Quiz name cannot be empty.");
            
        if (passPercentage < 0 || passPercentage > 100)
            throw new ArgumentException("Pass percentage must be between 0 and 100.");

        Id = Guid.NewGuid();
        
        CreatorId = creatorId;
        
        Name = name;
        
        Description = description;
        
        PassPercentage = passPercentage;
        
        IsActive = false;
        
        IsPublished = false;
        
        CreatedOn = DateTime.UtcNow;
        
        ModifiedOn = DateTime.UtcNow;
    }

    
    public Guid Id { get; private set; } 
    public Guid CreatorId { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public double PassPercentage { get; private set; } 
    public bool IsActive { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime ModifiedOn { get; private set; }

    public virtual IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();
    public double MaxScore => _questions.Sum(q => q.MaxScore);

    public void UpdateBasicInfo(string name, string? description, string? category, double passPercentage)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Quiz name cannot be empty.");
        
        if (passPercentage < 0 || passPercentage > 100)
            throw new ArgumentException("Pass percentage must be between 0 and 100.");

        Name = name;
        Description = description;
        PassPercentage = passPercentage;
        ModifiedOn = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (!_questions.Any())
            throw new InvalidOperationException("Cannot publish a quiz without questions.");
        
        IsPublished = true;
        IsActive = true;
        ModifiedOn = DateTime.UtcNow;
    }

    public void Archive()
    {
        IsActive = false;
        ModifiedOn = DateTime.UtcNow;
    }
}