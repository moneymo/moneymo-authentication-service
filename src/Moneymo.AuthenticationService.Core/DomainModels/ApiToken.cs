using System;

namespace Moneymo.AuthenticationService.Core.DomainModels
{
    public class ApiToken
    {
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsValid { get; set; }
    }
}