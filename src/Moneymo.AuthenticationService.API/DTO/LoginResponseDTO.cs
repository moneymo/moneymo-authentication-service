using System;

namespace Moneymo.AuthenticationService.API.DTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}