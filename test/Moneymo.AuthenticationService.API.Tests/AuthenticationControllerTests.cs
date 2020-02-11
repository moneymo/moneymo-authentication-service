using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moneymo.AuthenticationService.API.Controllers;
using Moneymo.AuthenticationService.API.DTO;
using Moneymo.AuthenticationService.Core.DomainModels;
using Moneymo.AuthenticationService.Core.Services;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class AuthenticationControllerTests
    {
        private AuthenticationController authenticationController;
        private Mock<IApiUserService> apiUserService;

        [SetUp]
        public void Setup()
        {
            apiUserService = new Mock<IApiUserService>();
            apiUserService.Setup(s => s.CreateApiUserAsync(It.IsAny<CreateApiUserModel>())).Returns(Task.FromResult(ExistingApiUser));

            authenticationController = new AuthenticationController(apiUserService.Object);
        }

        [Test]
        public async Task CreateApiUser_ReturnsUserInformationWith200_WhenCalled()
        {
            var createUserRequestDTO = new CreateUserRequestDTO
            {
                Username = ExistingApiUser.Username,
                Password = "Password",
                Fullname = "User Full Name"
            };

            var result = await authenticationController.CreateApiUser(createUserRequestDTO);


            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okObjectResult = result as OkObjectResult;

            Assert.That(okObjectResult.Value, Is.InstanceOf<CreateUserResponseDTO>());

            var createUserResponseDTO = okObjectResult.Value as CreateUserResponseDTO;
            Assert.AreEqual(createUserRequestDTO.Username, createUserResponseDTO.Username);
        }

        private ApiUser ExistingApiUser => new ApiUser
        {
            Id = 1,
            Username = "existingUser"
        };
    }
}