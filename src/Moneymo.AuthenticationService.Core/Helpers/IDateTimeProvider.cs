using System;

namespace Moneymo.AuthenticationService.Core.Helpers
{
    public interface IDateTimeProvider
    {
        DateTime NowInUTC { get; }
    }
}