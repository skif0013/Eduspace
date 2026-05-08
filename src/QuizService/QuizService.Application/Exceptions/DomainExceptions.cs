namespace QuizService.Application.Exceptions;

// какая то странная хуета хуй пойми че оно тут делает но зачем-то нужно 
public class ApplicationException : Exception
{
    public ApplicationException(string message) : base(message) { }
    public ApplicationException(string message, Exception innerException) : base(message, innerException) { }
}


public class EventPublishingException : ApplicationException
{
    public EventPublishingException(string streamKey, Exception innerException)
        : base($"Failed to publish event to stream '{streamKey}'", innerException) { }
}


public class AttemptNotFoundException : ApplicationException
{
    public AttemptNotFoundException(Guid attemptId)
        : base($"Quiz attempt with ID '{attemptId}' not found") { }
}

