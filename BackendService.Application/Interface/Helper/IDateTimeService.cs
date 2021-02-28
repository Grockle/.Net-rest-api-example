using System;

namespace BackendService.Application.Interface.Helper
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTime Now { get; }
    }
}
