using System;

namespace Moneymo.AuthenticationService.Core.Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime NowInUTC => DateTime.UtcNow;
    }
}