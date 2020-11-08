using BackendService.Data.DTOs;

namespace BackendService.Mappings
{
    public class GeneralMapping<T>
    {
        public BaseResponse<T> MapBaseResponse(bool hasError, string errorMessage, T data)
        {
            return new BaseResponse<T>
            {
                HasError = hasError,
                Message = errorMessage,
                Data = data
            };
        }
    }
}