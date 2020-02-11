using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moneymo.AuthenticationService.Core.DomainModels;
using Moneymo.AuthenticationService.Data.Models;
using Moneymo.AuthenticationService.Core.Helpers;
using System.Linq;
using static Moneymo.AuthenticationService.Core.Exceptions.AuthenticationServiceExceptions;

namespace Moneymo.AuthenticationService.Core.Services
{
    public class ApiUserService : IApiUserService
    {
        private readonly MoneymoDbContext dbContext;
        private readonly AuthenticationServiceConfiguration configuration;
        private readonly IDateTimeProvider dateTimeProvider;

        public ApiUserService(MoneymoDbContext dbContext, AuthenticationServiceConfiguration configuration, IDateTimeProvider datetimeProvider)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.dateTimeProvider = datetimeProvider;
        }

        public async Task<ApiUser> CreateApiUserAsync(CreateApiUserModel createApiUserModel)
        {
            if (string.IsNullOrWhiteSpace(createApiUserModel.Username)) throw new ArgumentNullException(nameof(createApiUserModel.Username));
            if (string.IsNullOrWhiteSpace(createApiUserModel.Password)) throw new ArgumentNullException(nameof(createApiUserModel.Password));
            if (string.IsNullOrWhiteSpace(createApiUserModel.Fullname)) throw new ArgumentNullException(nameof(createApiUserModel.Fullname));

            if (dbContext.FwPersons.Any(p => p.UserName == createApiUserModel.Username)) throw new UsernameAlreadyExistsException(createApiUserModel.Username);

            var passwordHashAndSalt = HashHelper.GeneratePasswordHash(createApiUserModel.Password);

            FwPerson person = new FwPerson()
            {
                FullName = createApiUserModel.Fullname,
                UserName = createApiUserModel.Username,
                Password = passwordHashAndSalt.passwordHash,
                Salt = passwordHashAndSalt.salt,
                Status = true,
                RoleId = 1,
                UpdServiceId = 1,
                LastUpdBy = 1
            };

            await dbContext.FwPersons.AddAsync(person);
            await dbContext.SaveChangesAsync();

            var apiUser = new ApiUser
            {
                Id = person.Id,
                Username = person.UserName
            };

            return apiUser;
        }

        public async Task<ApiToken> LoginUserAsync(LoginModel loginModel)
        {
            var now = dateTimeProvider.NowInUTC;
            FwPerson user = await GetPerson(loginModel);

            CheckUserPassword(loginModel, user);

            FwSession existingSession = await GetLastSession(user);
            FwSession fwSession;
            if (existingSession != default(FwSession) && existingSession.Valid > now)
            {
                fwSession = existingSession;
            }
            else
            {
                fwSession = new FwSession
                {
                    PersonId = user.Id,
                    SessionKey = Guid.NewGuid().ToString("N"),
                    Created = now,
                    Valid = now.AddDays(configuration.TokenLifetimeDayCount),
                    UpdServiceId = 1,
                    ChannelId = 1
                };

                await dbContext.FwSessions.AddAsync(fwSession);
                await dbContext.SaveChangesAsync();
            }

            var apiToken = new ApiToken
            {
                Token = fwSession.SessionKey,
                CreatedAt = fwSession.Created,
                ValidUntil = fwSession.Valid
            };

            return apiToken;
        }

        public async Task<ApiToken> CheckToken(string token)
        {
            var session = await dbContext.FwSessions.Where(s => s.SessionKey == token).FirstOrDefaultAsync();
            var now = dateTimeProvider.NowInUTC;

            if (session == default(FwSession)) throw new InvalidTokenException(token);

            if (session.Valid < now) throw new TokenExpiredException(token);

            return new ApiToken
            {
                Token = session.SessionKey,
                CreatedAt = session.Created,
                ValidUntil = session.Valid
            };
        }

        private async Task<FwSession> GetLastSession(FwPerson user)
        {
            return await dbContext.FwSessions.Where(s => s.PersonId == user.Id).OrderByDescending(s => s.Created).FirstOrDefaultAsync();
        }

        private void CheckUserPassword(LoginModel loginModel, FwPerson user)
        {
            if (UserPasswordIsNotValid(loginModel.Password, user)) throw new InvalidPasswordException();
        }

        private async Task<FwPerson> GetPerson(LoginModel loginModel)
        {
            var user = await dbContext.FwPersons.Where(p => p.UserName == loginModel.Username).FirstOrDefaultAsync();
            if (user == default(FwPerson)) throw new RecordNotFoundException("User");
            return user;
        }

        private bool UserPasswordIsNotValid(string attemptedPassword, FwPerson user)
        {
            return !HashHelper.Verify(attemptedPassword, user.Password, user.Salt);
        }


    }
}
