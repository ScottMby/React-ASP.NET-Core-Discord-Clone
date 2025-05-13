using Discord_Clone.Server.Models;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Services;
using Discord_Clone.Server.Tests.Utilities;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Security.Claims;

namespace Discord_Clone.Server.Tests.UnitTests.Services
{
    public class UserServiceUnitTests : IDisposable
    {

        readonly Mock<IUserRepository> mockUserRepository;
        readonly Mock<UserManager<User>> mockUserManager;
        readonly Mock<ILogger<UserService>> mockLogger;


        public UserServiceUnitTests()
        {
            mockUserRepository = new Mock<IUserRepository>();

            mockUserManager = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<User>>().Object,
                Array.Empty<IUserValidator<User>>(),
                Array.Empty<IPasswordValidator<User>>(),
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);
            mockUserManager
                .Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
                .Returns("mockUserId");

            mockLogger = new Mock<ILogger<UserService>>();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        [Fact]
        public async Task CheckDisplayNameValid_Success_NullDisplayName()
        {
            //Arrange
            string displayNameResult = "";

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetUserDisplayName(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, displayName) =>
                {
                    displayNameResult = displayName;
                });

            //Act
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);
            await userService.CheckDisplayNameValid(new ClaimsPrincipal());

            //Assert
            mockUserRepository.Verify(ur => ur.SetUserDisplayName(It.IsAny<User>(), It.IsAny<string>()), Times.AtLeastOnce());
            Assert.False(String.IsNullOrEmpty(displayNameResult));
        }

        [Fact]
        public async Task CheckDisplayNameValid_Success_AlreadyHasDisplayName()
        {
            //Arrange

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    DisplayName = "NotEmpty"
                });

            mockUserRepository
                .Setup(ur => ur.SetUserDisplayName(It.IsAny<User>(), It.IsAny<string>()));

            //Act
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);
            await userService.CheckDisplayNameValid(new ClaimsPrincipal());

            //Assert
            mockUserRepository.Verify(ur => ur.SetUserDisplayName(It.IsAny<User>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [InlineData("test")]
        [InlineData("abc")]
        [InlineData("CuqqLaUR4aj*yUKXk$KEp=L?f.R!d[;C5wA@WwLS4:7S[i&zr5?7f&yJ/8fVT+-Pcm)4zT;U;YLm&S2WL1):ex7S$#z:=QR4q+AqVNF}SuRFGuD@%?rH{R5_Kt0jyq#p:7QU?ixGiKXHxt7+3XvTpr*gy,6=Kh!3fjd)1y%$7kRmz)8qK)S7,g*bK0nR$.uJXi{]c:HBT!:yuuN2nK,;m5;G:W(1rBgAX%v9W4ce7UFmKie)!A-qSB2){{&4uFN")] //255 Characters
        public async Task ChangeDisplayName_Success(string enteredDisplayName)
        {
            string displayNameResult = "";

            //Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetUserDisplayName(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, displayName) =>
                {
                    displayNameResult = displayName;
                });

            //Act
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);
            await userService.ChangeDisplayName(new ClaimsPrincipal(), enteredDisplayName);

            //Assert
            mockUserRepository.Verify(ur => ur.SetUserDisplayName(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(enteredDisplayName, displayNameResult);
        }

        [Theory]
        [InlineData("")]
        [InlineData("ab")]
        [InlineData("CuqqLaUR4aj*yUKXk$KEp=L?f.R!d[;C5wA@WwLS4:7S[i&zr5?7f&yJ/8fVT+-Pcm)4zT;U;YLm&S2WL1):ex7S$#z:=QR4q+AqVNF}SuRFGuD@%?rH{R5_Kt0jyq#p:7QU?ixGiKXHxt7+3XvTpr*gy,6=Kh!3fjd)1y%$7kRmz)8qK)S7,g*bK0nR$.uJXi{]c:HBT!:yuuN2nK,;m5;G:W(1rBgAX%v9W4ce7UFmKie)!A-qSB2){{&4uFNa")] //256 Characters
        [InlineData("    ")]
        public async Task ChangeDisplayName_Exception_InvalidInput(string enteredDisplayName)
        {
            //Arrange  
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);

            //Act & Assert  
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userService.ChangeDisplayName(new ClaimsPrincipal(), enteredDisplayName);
            });
        }

        [Theory]
        [InlineData("Jo")]
        [InlineData("Jane")]
        [InlineData("0t$ez&y2P+(L#jdq4%uafh.r#[8STK@U!0C}eUukt4AT#R4&RB")] //50 Characters
        public async Task EditFirstName_Success(string enteredFirstName)
        {
            string firstNameResult = "";

            // Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetFirstName(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, firstName) =>
                {
                    firstNameResult = firstName;
                });

            // Act
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);
            await userService.EditFirstName(new ClaimsPrincipal(), enteredFirstName);

            // Assert
            mockUserRepository.Verify(ur => ur.SetFirstName(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(enteredFirstName, firstNameResult);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("    ")]
        [InlineData("0t$ez&y2P+(L#jdq4%uafh.r#[8STK@U!0C}eUukt4AT#R4&RB&")] //51 Characters
        public async Task EditFirstName_Exception_InvalidInput(string enteredFirstName)
        {
            string firstNameResult = "";

            // Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetFirstName(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, firstName) =>
                {
                    firstNameResult = firstName;
                });

            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userService.EditFirstName(new ClaimsPrincipal(), enteredFirstName);
            });
        }

        [Theory]
        [InlineData("Do")]
        [InlineData("Test")]
        [InlineData("CuqqLaUR4aj*yUKXk$KEp=L?f.R!d[;C5wA@WwLS4:7S[i&zr5?7f&yJ/8fVT+-Pcm)4zT;U;YLm&S2WL1):ex7S$#z:=QR4q+AqVNF}SuRFGuD@%?rH{R5_Kt0jyq#p:7QU?ixGiKXHxt7+3XvTpr*gy,6=Kh!3fjd)1y%$7kRmz)8qK)S7,g*bK0nR$.uJXi{]c:HBT!:yuuN2nK,;m5;G:W(1rBgAX%v9W4ce7UFmKie)!A-qSB2){{&4uFN")] //255 Characters
        public async Task EditLastName_Success(string enteredLastName)
        {
            string lastNameResult = "";

            // Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetLastName(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, lastName) =>
                {
                    lastNameResult = lastName;
                });

            // Act
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);
            await userService.EditLastName(new ClaimsPrincipal(), enteredLastName);

            // Assert
            mockUserRepository.Verify(ur => ur.SetLastName(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(enteredLastName, lastNameResult);
        }

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("    ")]
        [InlineData("CuqqLaUR4aj*yUKXk$KEp=L?f.R!d[;C5wA@WwLS4:7S[i&zr5?7f&yJ/8fVT+-Pcm)4zT;U;YLm&S2WL1):ex7S$#z:=QR4q+AqVNF}SuRFGuD@%?rH{R5_Kt0jyq#p:7QU?ixGiKXHxt7+3XvTpr*gy,6=Kh!3fjd)1y%$7kRmz)8qK)S7,g*bK0nR$.uJXi{]c:HBT!:yuuN2nK,;m5;G:W(1rBgAX%v9W4ce7UFmKie)!A-qSB2){{&4uFNa")] //256 Characters

        public async Task EditLastName_Exception_InvalidInput(string enteredLastName)
        {
            string lastNameResult = "";

            // Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetLastName(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, lastName) =>
                {
                    lastNameResult = lastName;
                });

            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userService.EditLastName(new ClaimsPrincipal(), enteredLastName);
            });
        }

        [Theory]
        [InlineData("I")]
        [InlineData("Software developer and tech enthusiast.")]
        [InlineData("CuqqLaUR4aj*yUKXk$KEp=L?f.R!d[;C5wA@WwLS4:7S[i&zr5?7f&yJ/8fVT+-Pcm)4zT;U;YLm&S2WL1):ex7S$#z:=QR4q+AqVNF}SuRFGuD@%?rH{R5_Kt0jyq#p:7QU?ixGiKXHxt7+3XvTpr*gy,6=Kh!3fjd)1y%$7kRmz)8qK)S7,g*bK0nR$.uJXi{]c:HBT!:yuuN2nK,;m5;G:W(1rBgAX%v9W4ce7UFmKie)!A-qSB2){{&4uFN")] //255 Characters
        public async Task EditAboutMe_Success(string enteredAboutMe)
        {
            string aboutMeResult = "";

            // Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetAboutMe(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, aboutMe) =>
                {
                    aboutMeResult = aboutMe;
                });

            // Act
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);
            await userService.EditAboutMe(new ClaimsPrincipal(), enteredAboutMe);

            // Assert
            mockUserRepository.Verify(ur => ur.SetAboutMe(It.IsAny<User>(), It.IsAny<string>()), Times.Once);
            Assert.Equal(enteredAboutMe, aboutMeResult);
        }

        [Theory]
        [InlineData("CuqqLaUR4aj*yUKXk$KEp=L?f.R!d[;C5wA@WwLS4:7S[i&zr5?7f&yJ/8fVT+-Pcm)4zT;U;YLm&S2WL1):ex7S$#z:=QR4q+AqVNF}SuRFGuD@%?rH{R5_Kt0jyq#p:7QU?ixGiKXHxt7+3XvTpr*gy,6=Kh!3fjd)1y%$7kRmz)8qK)S7,g*bK0nR$.uJXi{]c:HBT!:yuuN2nK,;m5;G:W(1rBgAX%v9W4ce7UFmKie)!A-qSB2){{&4uFNa")] //256 Characters
        public async Task EditAboutMe_Exception_InvalidInput(string enteredAboutMe)
        {
            //Arrange
            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);

            //Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userService.EditAboutMe(new ClaimsPrincipal(), enteredAboutMe);
            });
        }

        [Fact]
        public async Task StoreUserImage_Success_StoresNewFileName()
        {
            string photoURLResult = "";
            //Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetPhotoURL(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, photoURL) =>
                {
                    photoURLResult = photoURL;
                });

            IFormFile file = FileHelper.CreateFormFile("text.png", "test");

            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);

            //Act
            await userService.StoreUserImage(new ClaimsPrincipal(), file);

            //Assert
            Assert.NotEmpty(photoURLResult);
            Assert.NotEqual(Path.GetFileName("text.png"), Path.GetFileName(photoURLResult));
        }

        [Fact]
        public async Task StoreUserImage_Exception_FileTooSmall()
        {
            string photoURLResult = "";
            //Arrange
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserRepository
                .Setup(ur => ur.SetPhotoURL(It.IsAny<User>(), It.IsAny<string>()))
                .Callback<User, string>((user, photoURL) =>
                {
                    photoURLResult = photoURL;
                });

            IFormFile file = FileHelper.CreateFormFile("text.png", "");

            UserService userService = new(mockUserRepository.Object, mockUserManager.Object, mockLogger.Object);

            //Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userService.StoreUserImage(new ClaimsPrincipal(), file);
            });
        }
    }
}
