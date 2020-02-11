using NUnit.Framework;
using Moneymo.AuthenticationService.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Moneymo.AuthenticationService.Core.DomainModels;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Moq;
using Moneymo.AuthenticationService.Core.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using static Moneymo.AuthenticationService.Core.Exceptions.AuthenticationServiceExceptions;
using System.Linq;
using Moneymo.AuthenticationService.Core.Helpers;

namespace Moneymo.AuthenticationService.Core.Tests
{
    public class ApiUserServiceTests
    {
        private MoneymoDbContext mockDbContext;
        private AuthenticationServiceConfiguration configuration;
        private ApiUserService apiUserService;
        private ServiceProvider serviceProvider;
        private ApiUser alreadyExistsApiUser;
        private Mock<IDateTimeProvider> mockDateTimeProvider;

        [SetUp]
        public async Task Setup()
        {
            configuration = TestHelper.GetApplicationConfiguration(TestContext.CurrentContext.TestDirectory);

            mockDateTimeProvider = new Mock<IDateTimeProvider>();
            mockDateTimeProvider.SetupGet(p => p.NowInUTC).Returns(DateTime.UtcNow);

            var services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddDbContext<MoneymoDbContext>(builder =>
            {
                builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
            services.AddTransient<ApiUserService>();
            services.AddSingleton<IDateTimeProvider>(mockDateTimeProvider.Object);

            serviceProvider = services.BuildServiceProvider();
            apiUserService = serviceProvider.GetRequiredService<ApiUserService>();
            mockDbContext = serviceProvider.GetRequiredService<MoneymoDbContext>();

            alreadyExistsApiUser = await apiUserService.CreateApiUserAsync(ExistingUserCreateModel);
        }

        [Test]
        public async Task CreateUser_ReturnsUserInformation_WhenCalled()
        {
            var apiUser = await apiUserService.CreateApiUserAsync(NewUserCreateModel);

            Assert.IsNotNull(apiUser);
            Assert.IsTrue(apiUser.Id > 0);
        }

        [Test]
        public async Task CreateUser_CreatesUserInDb_WhenCalled()
        {
            var apiUser = await apiUserService.CreateApiUserAsync(NewUserCreateModel);

            var userInDb = await mockDbContext.FwPersons.FindAsync(apiUser.Id);

            Assert.IsNotNull(userInDb);
        }

        [Test]
        public void CreateUser_ThrowsException_IfUsernameAlreadyExists()
        {
            Assert.ThrowsAsync<UsernameAlreadyExistsException>(async () => await apiUserService.CreateApiUserAsync(ExistingUserCreateModel));
        }

        [TestCaseSource("CreateApiUserProvider")]
        public void CreateUser_ThrowsException_WhenAnyPropertyOfParameterIsNull(CreateApiUserModel createApiUserModel)
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await apiUserService.CreateApiUserAsync(createApiUserModel));
        }

        [Test]
        public async Task Login_ReturnsTokenInformation_WhenCalled()
        {
            var loginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = ExistingUserCreateModel.Password
            };

            var apiToken = await apiUserService.LoginUserAsync(loginModel);
            var tokenValidDayCount = (apiToken.ValidUntil - apiToken.CreatedAt).Days;

            Assert.IsNotNull(apiToken);
            Assert.IsNotNull(apiToken.Token);
            Assert.AreEqual(configuration.TokenLifetimeDayCount, tokenValidDayCount);
        }

        [Test]
        public async Task Login_CreatesSessionInDb_WhenCalled()
        {
            var loginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = ExistingUserCreateModel.Password
            };

            var apiToken = await apiUserService.LoginUserAsync(loginModel);

            var session = await mockDbContext.FwSessions.Where(s => s.SessionKey == apiToken.Token && s.PersonId == alreadyExistsApiUser.Id).FirstOrDefaultAsync();

            Assert.IsNotNull(session);
            Assert.AreNotEqual(session, default(FwSession));
        }

        [Test]
        public async Task Login_ReturnsSameToken_WhenUserHaveTokenAlready()
        {
            var loginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = ExistingUserCreateModel.Password
            };

            var firstToken = await apiUserService.LoginUserAsync(loginModel);

            var seconToken = await apiUserService.LoginUserAsync(loginModel);

            Assert.AreEqual(firstToken.Token, seconToken.Token);
        }

        [Test]
        public async Task Login_CreatesNewToken_WhenTokenIsExpired()
        {
            var loginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = ExistingUserCreateModel.Password
            };
           var firstToken = await apiUserService.LoginUserAsync(loginModel);

            mockDateTimeProvider.SetupGet(p => p.NowInUTC).Returns(DateTime.UtcNow.AddDays(configuration.TokenLifetimeDayCount +1 ));

            var secondToken = await apiUserService.LoginUserAsync(loginModel);

            Assert.AreNotEqual(firstToken.Token, secondToken.Token);
        }

        [Test]
        public void Login_ThrowsException_WhenPasswordIsWrong()
        {
            var wrongPasswordLoginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = "WrongPassword"
            };

            Assert.ThrowsAsync<InvalidPasswordException>(async () => await apiUserService.LoginUserAsync(wrongPasswordLoginModel));
        }

        [Test]
        public void Login_ThrowsException_WhenUserNotFound()
        {
            var notExistingLoginModel = new LoginModel
            {
                Username = "ImNotInDb",
                Password = "SoPasswordIsWrongAlso"
            };

            Assert.ThrowsAsync<RecordNotFoundException>(async () => await apiUserService.LoginUserAsync(notExistingLoginModel));
        }

        [Test]
        public async Task CheckToken_ReturnsTokenInfo_IfTokenIsNotExpired()
        {
            var loginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = ExistingUserCreateModel.Password
            };
            var apiToken = await apiUserService.LoginUserAsync(loginModel);

            var result = await apiUserService.CheckToken(apiToken.Token);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Token, apiToken.Token);
        }

        [Test]
        public async Task CheckToken_ThrowsTokenExpiredException_IfTokenIsExpired()
        {
            var loginModel = new LoginModel
            {
                Username = ExistingUserCreateModel.Username,
                Password = ExistingUserCreateModel.Password
            };
            var apiToken = await apiUserService.LoginUserAsync(loginModel);

            mockDateTimeProvider.SetupGet(p => p.NowInUTC).Returns(DateTime.UtcNow.AddDays(configuration.TokenLifetimeDayCount + 1));

            Assert.ThrowsAsync<TokenExpiredException>(async () => await apiUserService.CheckToken(apiToken.Token));
        }

        [Test]
        public async Task CheckToken_ThrowsInvalidTokenException_IfTokenIsNotExists()
        {
            Assert.ThrowsAsync<InvalidTokenException>(async () => await apiUserService.CheckToken("RandomTokenThatNotExists"));
        }

        private static IEnumerable<CreateApiUserModel> CreateApiUserProvider()
        {
            yield return new CreateApiUserModel
            {
                Username = null,
                Password = "password",
                Fullname = "fullname"
            };

            yield return new CreateApiUserModel
            {
                Username = "username",
                Password = null,
                Fullname = "fullname"
            };

            yield return new CreateApiUserModel
            {
                Username = "username",
                Password = "password",
                Fullname = null
            };
        }

        private CreateApiUserModel ExistingUserCreateModel => new CreateApiUserModel()
        {
            Username = "username",
            Password = "password",
            Fullname = "Already Exists"
        };

        private CreateApiUserModel NewUserCreateModel => new CreateApiUserModel()
        {
            Username = "new_username",
            Password = "new_password",
            Fullname = "New User"
        };
    }
}