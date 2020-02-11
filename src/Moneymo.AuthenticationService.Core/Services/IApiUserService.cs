using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moneymo.AuthenticationService.Core.DomainModels;
using Moneymo.AuthenticationService.Data.Models;

namespace Moneymo.AuthenticationService.Core.Services
{
    public interface IApiUserService
    {
        Task<ApiUser> CreateApiUserAsync(CreateApiUserModel createApiUserModel);
        Task<ApiToken> LoginUserAsync(LoginModel loginModel);
        Task<ApiToken> CheckToken(string token);
    }
}
