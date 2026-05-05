namespace QuizService.Application.Exceptions;

/// <summary>
/// Базовое исключение приложения
/// </summary>
public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
    public ApplicationException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Исключение при неудаче публикации события
/// </summary>
public class EventPublishingException : ApplicationException
{
    public EventPublishingException(string streamKey, Exception innerException)
        : base($"Failed to publish event to stream '{streamKey}'", innerException) { }
}

/// <summary>
/// Исключение при попытке завершить несуществующую попытку
/// </summary>
public class AttemptNotFoundException : ApplicationException
{
    public AttemptNotFoundException(Guid attemptId)
        : base($"Quiz attempt with ID '{attemptId}' not found") { }
}

