namespace Shared.SeedWork
{
    public class ApiResult<T>
    {
        public ApiResult()
        { }

        public ApiResult(bool isSucceeded, string message = null)
        {
            Message = message;
            IsSucceeded = isSucceeded;
        }
        public ApiResult(bool isSucceeded, T data, string message = null)
        {
            Message = message;
            IsSucceeded = isSucceeded;
            Data = data;
        }


        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
        public T Data { get; }
    }
}
