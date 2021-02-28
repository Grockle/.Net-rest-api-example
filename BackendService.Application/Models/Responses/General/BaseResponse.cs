namespace BackendService.Application.Models.Responses.General
{
    public class BaseResponse<T>
    {
        public bool HasError { get; set; }
        public BaseResponseErrorItem Error { get; set; }
        public T Data { get; set; }
    }
}