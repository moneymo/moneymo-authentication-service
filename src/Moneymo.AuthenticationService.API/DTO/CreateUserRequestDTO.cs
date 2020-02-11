namespace Moneymo.AuthenticationService.API.DTO
{
    public class CreateUserRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
    }
}