using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moneymo.AuthenticationService.API.DTO;
using Moneymo.AuthenticationService.Core.DomainModels;
using Moneymo.AuthenticationService.Core.Services;

namespace Moneymo.AuthenticationService.API.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IApiUserService apiUserService;

        public AuthenticationController(IApiUserService apiUserService)
        {
            this.apiUserService = apiUserService;
        }

        [HttpPost("authapi/users/login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDTO loginDTO)
        {
            var loginModel = new LoginModel
            {
                Username = loginDTO.Username,
                Password = loginDTO.Password
            };

            var loginResult = await apiUserService.LoginUserAsync(loginModel);

            var response = new LoginResponseDTO
            {
                Token = loginResult.Token,
                CreatedAt = loginResult.CreatedAt,
                ValidUntil = loginResult.ValidUntil
            };

            return Ok(response);
        }

        [HttpPost("authapi/users")]
        public async Task<IActionResult> CreateApiUser([FromBody]CreateUserRequestDTO createUserRequestDTO)
        {
            var createApiUserModel = new CreateApiUserModel
            {
                Username = createUserRequestDTO.Username,
                Password = createUserRequestDTO.Password,
                Fullname = createUserRequestDTO.Fullname
            };

            var serviceResult = await apiUserService.CreateApiUserAsync(createApiUserModel);

            var response = new CreateUserResponseDTO
            {
                Id = serviceResult.Id,
                Username = serviceResult.Username
            };

            return Ok(response);
        }

        [HttpPost("authapi/token/check")]
        public async Task<IActionResult> CheckToken([FromBody]CheckTokenRequestDTO tokenToCheck)
        {
            var result = await apiUserService.CheckToken(tokenToCheck.Token);

            var response = new CheckTokenResponseDTO
            {
                Token = result.Token,
                CreatedAt = result.CreatedAt,
                ValidUntil = result.ValidUntil
            };

            return Ok(response);
        }
    }
}
