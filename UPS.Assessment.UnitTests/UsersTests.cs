using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Interfaces.Services;
using UPS.Assessment.Models;
using UPS.Assessment.Services;


namespace UPS.Assessment.UnitTests
{
    [TestFixture]
    public class UsersTests
    {
        private IUserService _userService;
        private Mock<IHttpClientHelper> _httpClient;
        [SetUp]
        public void Setup()
        {
            _httpClient = new Mock<IHttpClientHelper>();
            IOptions<GoRestCred> options = Options.Create<GoRestCred>(new GoRestCred()
            {
                BaseUrl = "https://gorest.co.in",
                Token = "iuwehuhfuufw",
            });
            _userService = new UserService(_httpClient.Object, options);
        }


        [Test]
        [TestCase(null, null, null, null, 3)]
        [TestCase("John", null, null, null, 2)]
        [TestCase(null, "mustafa@gmail.com", null, null, 1)]
        [TestCase(null, null, "male", null, 3)]
        [TestCase(null, null, "female", null, 0)]
        [TestCase(null, null, null, "active", 2)]
        [TestCase("John", "john@gmail.com", null, "active", 1)]
        [TestCase("John", "john@gmail.com", null, "inactive", 0)]

        public async Task GetUsers_ReturnsResult(string? name, string? email, string? gender, string? status, int expectedDataCount)
        {
            UsersDataSetup.SetupGetListResponse_Success(_httpClient);

            var users = await _userService.GetUsers(new GetUsersForm()
            {
                Name = name,
                Email = email,
                Gender = gender,
                Status = status
            });
            Assert.IsNotNull(users);
            Assert.AreEqual(expectedDataCount, users.Data.Count);
        }

        [Test]
        public async Task GetUsers_Fail()
        {
            UsersDataSetup.SetupGetListResponses_Fail(_httpClient);
            var users = await _userService.GetUsers(new GetUsersForm()
            {

                PageNumber = 5
            });

            Assert.IsTrue(users.Failed);
        }


        [Test]
        public async Task AddUser_ReturnsOk()
        {
            UsersDataSetup.SetupAddUserSuccess(_httpClient);
            var user = await _userService.AddUser(new CreateUserForm()
            {
                name = "name",
                email = "email@test.com",
                gender = "male",
                status = "active"
            });
            Assert.IsNotNull(user.Data);

        }

        [Test]
        public async Task AddUser_ReturnsFail()
        {
            UsersDataSetup.SetupAddUserFail(_httpClient);
            var user = await _userService.AddUser(new CreateUserForm()
            {
                name = null,
                email = "email",
                gender = "male",
                status = "active"
            });
            Assert.IsNotNull(user.Errors);
            Assert.That(2, Is.EqualTo(user.Errors.Count));

        }


        [Test]
        public async Task DeleteUser_ReturnsSuccess()
        {
            UsersDataSetup.SetupDeleteUserSuccess(_httpClient);
            var response = await _userService.DeleteUser(1);
            Assert.False(response.Errors.Any());
            Assert.That(response.Succeeded);

        }

    }
}