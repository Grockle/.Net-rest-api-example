using System.Collections.Generic;

namespace BackendService.Data.DTOs
{
    public class BaseResponse<T>
    {
        public bool HasError { get; set; }
        public ErrorMessageDto Error { get; set; }
        public T Data { get; set; }
    }
}