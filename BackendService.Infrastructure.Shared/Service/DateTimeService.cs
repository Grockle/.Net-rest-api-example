using System;
using BackendService.Application.Interface.Helper;

namespace BackendService.Infrastructure.Shared.Service
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
        public DateTime Now => DateTime.Now;
    }
}
