namespace AuthService.Domain.Results;

    public class Result<T> 
    {
        public T? Data { get; set; }
        public bool IsError { get; set; }
        public string? ErrorMessage { get; set; }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Data = data, IsError = false };
        }

        public static Result<T> Failure(string errorMessage)
        {
            return new Result<T> { IsError = true, ErrorMessage = errorMessage };
        }
    }