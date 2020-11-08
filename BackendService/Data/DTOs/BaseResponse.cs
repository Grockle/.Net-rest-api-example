namespace BackendService.Data.DTOs
{
    public class BaseResponse<T>
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}