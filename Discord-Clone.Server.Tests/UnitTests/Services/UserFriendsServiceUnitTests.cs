﻿using Discord_Clone.Server.Models;
using Discord_Clone.Server.Models.Data_Transfer_Objects;
using Discord_Clone.Server.Repositories.Interfaces;
using Discord_Clone.Server.Services;
using Discord_Clone.Server.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Clone.Server.Tests.UnitTests.Services
{
    public class UserFriendsServiceUnitTests
    {
        readonly Mock<IUserRepository> mockUserRepository;
        readonly Mock<IUserFriendsRepository> mockUserFriendsRepository;
        readonly Mock<UserManager<User>> mockUserManager;
        readonly Mock<ILogger<UserFriendsService>> mockUserFriendsLogger;


        public UserFriendsServiceUnitTests()
        {
            mockUserRepository = new Mock<IUserRepository>();

            mockUserFriendsRepository = new Mock<IUserFriendsRepository>();

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

            mockUserFriendsLogger = new Mock<ILogger<UserFriendsService>>();
        }

        [Fact]
        public async Task UserSearch_Success()
        {
            //Arrange
            UserSearchResult userSearchResult = new()
            {
                DisplayName = "test",
                Id = "testId",
                PhotoURL = "tmp/test.png",
                Rank = 2.5f
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserFriendsRepository
                .Setup(ufr => ufr.UserSearch(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    List<UserSearchResult> userSearchResults = [userSearchResult];
                    return userSearchResults;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act
            List<UserSearchResult> results = await userFriendService.UserSearch(new ClaimsPrincipal(), "test");

            //Assert
            Assert.Equal(userSearchResult, results.First());
        }

        [Fact]
        public async Task UserSearch_Success_RemovesSearchingUserFromResults()
        {
            //Arrange
            UserSearchResult userSearchResult = new()
            {
                DisplayName = "test",
                Id = "testId",
                PhotoURL = "tmp/test.png",
                Rank = 2.5f
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User
                {
                    Id = "currentUser"
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.UserSearch(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    List<UserSearchResult> userSearchResults =
                    [
                        new UserSearchResult
                        {
                            DisplayName = "test2",
                            Id = "currentUser",
                            PhotoURL = "tmp/random.png",
                            Rank = 1.5f
                        },
                        userSearchResult,
                    ];
                    return userSearchResults;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act
            List<UserSearchResult> results = await userFriendService.UserSearch(new ClaimsPrincipal(), "test");

            //Assert
            Assert.False(results.Where(r => r.Id == "currentUser").Any());
            Assert.Equal(userSearchResult, results.First());
        }

        [Fact]
        public async Task UserFriendRequest_Success()
        {
            //Arrange
            UserFriendRequests userFriendRequestResult = null!;
            List<User> users = [];

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(() =>
                {
                    User user = new();
                    users.Add(user);
                    return user;
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserHasPendingFriendRequest(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserIsFriends(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriendRequest(It.IsAny<UserFriendRequests>()))
                .Callback<UserFriendRequests>((userFriendRequest) =>
                {
                    userFriendRequestResult = userFriendRequest;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act
            await userFriendService.UserFriendRequest(new ClaimsPrincipal(), "sendingUserId");

            //Assert
            Assert.NotNull(userFriendRequestResult);
            Assert.Equal(users.First().Id, userFriendRequestResult.SenderId);
            Assert.Equal(users.First(), userFriendRequestResult.Sender);
            Assert.Equal(users.Last().Id, userFriendRequestResult.ReceiverId);
            Assert.Equal(users.Last(), userFriendRequestResult.Receiver);

        }

        [Fact]
        public async Task UserFriendRequest_Exception_SenderIsReceiver()
        {
            UserFriendRequests userFriendRequestResult = null!;

            //Arrange
            User user = new()
            {
                Id = "sendingUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(user);

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserHasPendingFriendRequest(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserIsFriends(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriendRequest(It.IsAny<UserFriendRequests>()))
                .Callback<UserFriendRequests>((userFriendRequest) =>
                {
                    userFriendRequestResult = userFriendRequest;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userFriendService.UserFriendRequest(new ClaimsPrincipal(), "sendingUserId");
            });
            
        }

        [Fact]
        public async Task UserFriendRequest_Exception_RequestAlreadyPending()
        {
            //Arrange
            UserFriendRequests userFriendRequestResult = null!;
            
            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserHasPendingFriendRequest(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserIsFriends(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriendRequest(It.IsAny<UserFriendRequests>()))
                .Callback<UserFriendRequests>((userFriendRequest) =>
                {
                    userFriendRequestResult = userFriendRequest;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userFriendService.UserFriendRequest(new ClaimsPrincipal(), "receiverUserId");
            });
        }

        [Fact]
        public async Task UserFriendRequest_Exception_AlreadyFriends()
        {
            //Arrange
            UserFriendRequests userFriendRequestResult = null!;

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(new User());

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserHasPendingFriendRequest(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserIsFriends(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriendRequest(It.IsAny<UserFriendRequests>()))
                .Callback<UserFriendRequests>((userFriendRequest) =>
                {
                    userFriendRequestResult = userFriendRequest;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(async () =>
            {
                await userFriendService.UserFriendRequest(new ClaimsPrincipal(), "receiverUserId");
            });
        }

        [Fact]
        public async Task UserFriendRequest_Exception_CouldNotFindReceiver()
        {
            //Arrange
            UserFriendRequests userFriendRequestResult = null!;

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync((string userId) =>
                {
                    if (userId == "receiverUserId")
                        return (User)null!;

                    return new User();
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserHasPendingFriendRequest(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.CheckUserIsFriends(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriendRequest(It.IsAny<UserFriendRequests>()))
                .Callback<UserFriendRequests>((userFriendRequest) =>
                {
                    userFriendRequestResult = userFriendRequest;
                });

            UserFriendsService userFriendService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await userFriendService.UserFriendRequest(new ClaimsPrincipal(), "receiverUserId");
            });
        }

        [Fact]
        public async Task AcceptFriendRequest_Success()
        {
            //Arrange
            UserFriends userFriendsResult = null!;

            User sender = new()
            {
                Id = "receiverUserId"
            };

            User receiver = new()
            {
                Id = "senderUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(receiver);

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetFriendRequest(It.IsAny<string>()))
                .ReturnsAsync(new UserFriendRequests
                {
                    Receiver = receiver,
                    ReceiverId = receiver.Id,
                    Sender = sender,
                    SenderId = sender.Id
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()));

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriend(It.IsAny<UserFriends>()))
                .Callback((UserFriends userFriends) =>
                {
                    userFriendsResult = userFriends;
                });

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act
            await userFriendsService.AcceptFriendRequest(new ClaimsPrincipal(), "test");

            //Assert
            mockUserFriendsRepository.Verify(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()), Times.Once);
            mockUserFriendsRepository.Verify(ufr => ufr.AddUserFriend(It.IsAny<UserFriends>()), Times.Once);
            Assert.Equal(receiver, userFriendsResult.Receiver);
            Assert.Equal(sender, userFriendsResult.Sender);
            Assert.Equal(receiver.Id, userFriendsResult.ReceiverId);
            Assert.Equal(sender.Id, userFriendsResult.SenderId);
            Assert.NotNull(userFriendsResult.Chat);
        }

        [Fact]
        public async Task AcceptFriendRequest_Exception_UserNotReceiver()
        {
            //Arrange
            UserFriends userFriendsResult = null!;

            User sender = new()
            {
                Id = "receiverUserId"
            };

            User receiver = new()
            {
                Id = "senderUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(sender);

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetFriendRequest(It.IsAny<string>()))
                .ReturnsAsync(new UserFriendRequests
                {
                    Receiver = receiver,
                    ReceiverId = receiver.Id,
                    Sender = sender,
                    SenderId = sender.Id
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()));

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriend(It.IsAny<UserFriends>()))
                .Callback((UserFriends userFriends) =>
                {
                    userFriendsResult = userFriends;
                });

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () =>
            {
                await userFriendsService.AcceptFriendRequest(new ClaimsPrincipal(), "test");
            });
        }

        [Fact]
        public async Task AcceptFriendRequest_Exception_RequestDoesNotExist()
        {
            //Arrange
            UserFriends userFriendsResult = null!;

            User sender = new()
            {
                Id = "receiverUserId"
            };

            User receiver = new()
            {
                Id = "senderUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(receiver);

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetFriendRequest(It.IsAny<string>()))
                .ReturnsAsync((UserFriendRequests)null!);

            mockUserFriendsRepository
                .Setup(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()));

            mockUserFriendsRepository
                .Setup(ufr => ufr.AddUserFriend(It.IsAny<UserFriends>()))
                .Callback((UserFriends userFriends) =>
                {
                    userFriendsResult = userFriends;
                });

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await userFriendsService.AcceptFriendRequest(new ClaimsPrincipal(), "test");
            });
        }

        [Fact]
        public async Task DeclineFriendRequest_Success()
        {
            //Arrange

            User sender = new()
            {
                Id = "receiverUserId"
            };

            User receiver = new()
            {
                Id = "senderUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(receiver);

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetFriendRequest(It.IsAny<string>()))
                .ReturnsAsync(new UserFriendRequests
                {
                    Receiver = receiver,
                    ReceiverId = receiver.Id,
                    Sender = sender,
                    SenderId = sender.Id
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()));

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act
            await userFriendsService.DeclineFriendRequest(new ClaimsPrincipal(), "test");

            //Assert
            mockUserFriendsRepository.Verify(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeclineFriendRequest_Exception_UserNotReceiver()
        {
            //Arrange

            User sender = new()
            {
                Id = "receiverUserId"
            };

            User receiver = new()
            {
                Id = "senderUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(sender);

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetFriendRequest(It.IsAny<string>()))
                .ReturnsAsync(new UserFriendRequests
                {
                    Receiver = receiver,
                    ReceiverId = receiver.Id,
                    Sender = sender,
                    SenderId = sender.Id
                });

            mockUserFriendsRepository
                .Setup(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()));

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(async () =>
            {
                await userFriendsService.DeclineFriendRequest(new ClaimsPrincipal(), "test");
            });
        }

        [Fact]
        public async Task DeclineFriendRequest_RequestDoesNotExist()
        {
            //Arrange

            User sender = new()
            {
                Id = "receiverUserId"
            };

            User receiver = new()
            {
                Id = "senderUserId"
            };

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(receiver);

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetFriendRequest(It.IsAny<string>()))
                .ReturnsAsync((UserFriendRequests)null!);

            mockUserFriendsRepository
                .Setup(ufr => ufr.DeleteFriendRequest(It.IsAny<string>()));

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            //Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await userFriendsService.DeclineFriendRequest(new ClaimsPrincipal(), "test");
            });
        }

        [Fact]
        public async Task GetFriendRequests_Success()
        {
            // Arrange
            User userA = new()
            {
                Id = "userAId"
            };

            User userB = new()
            {
                Id = "userBId"
            };

            User userC = new()
            {
                Id = "userCId"
            };

            List<UserFriendRequests> userFriendRequests = new()
            {
                new UserFriendRequests
                {
                    Sender = userA,
                    SenderId = userA.Id,
                    Receiver = userB,
                    ReceiverId = userB.Id
                },
                new UserFriendRequests
                {
                    Sender = userB,
                    SenderId = userB.Id,
                    Receiver = userC,
                    ReceiverId = userC.Id
                },
                new UserFriendRequests
                {
                    Sender = userC,
                    SenderId = userC.Id,
                    Receiver = userA,
                    ReceiverId = userA.Id
                }
            };

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetUserFriendRequests(It.IsAny<User>()))
                .ReturnsAsync(userFriendRequests.Where(ufr => ufr.SenderId == "userAId" || ufr.ReceiverId == "userAId").ToList());

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(userA);

            UserFriendsService userFriendsService = new UserFriendsService(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            // Act
            List<UserFriendRequests> result = await userFriendsService.GetUserFriendRequests(new ClaimsPrincipal());

            // Assert
            Assert.True(result.Count == 2);
            foreach (var request in result)
            {
                Assert.True(request.SenderId == "userAId" || request.ReceiverId == "userAId");
            }
        }

        [Fact]
        public async Task GetFriendRequests_NoResults()
        {
            // Arrange
            User userA = new()
            {
                Id = "userAId"
            };

            User userB = new()
            {
                Id = "userBId"
            };

            User userC = new()
            {
                Id = "userCId"
            };

            List<UserFriendRequests> userFriendRequests = new()
            {
                new UserFriendRequests
                {
                    Sender = userA,
                    SenderId = userA.Id,
                    Receiver = userB,
                    ReceiverId = userB.Id
                }
            };

            mockUserFriendsRepository
                .Setup(ufr => ufr.GetUserFriendRequests(It.IsAny<User>()))
                .ReturnsAsync(userFriendRequests.Where(ufr => ufr.SenderId == "userCId" || ufr.ReceiverId == "userCId").ToList());

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(userC);

            UserFriendsService userFriendsService = new UserFriendsService(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            // Assert & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await userFriendsService.GetUserFriendRequests(new ClaimsPrincipal());
            });
        }

        [Fact]
        public async Task GetFriends_Sucess()
        {
            // Arrange
            User userA = new()
            {
                Id = "userAId"
            };

            User userB = new()
            {
                Id = "userBId"
            };

            User userC = new()
            {
                Id = "userCId"
            };

            DateTime dateTime = DateTime.UtcNow;

            List<UserFriendsResult> userFriendsResults = new()
            {
                new UserFriendsResult()
                {
                    Id = "userAId",
                    FriendsSince = dateTime,
                    DisplayName = "userADisplayName",
                    AboutMe = "Hello I am User A",
                    PhotoURL = "test.png"
                },
                new UserFriendsResult()
                {
                    Id = "userBId",
                    FriendsSince = dateTime,
                    DisplayName = "userBDisplayName",
                    AboutMe = "Hello I am User B",
                    PhotoURL = "testB.png"
                }
            };

            mockUserFriendsRepository.Setup(ufr => ufr.GetUserFriends(It.IsAny<User>()))
                .ReturnsAsync(userFriendsResults);

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(userC);

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            // Act
            List<UserFriendsResult> result = await userFriendsService.GetUserFriends(new ClaimsPrincipal());

            // Assert
            Assert.Equal(userFriendsResults, result);
        }

        [Fact]
        public async Task GetFriends_Exception_NoFriends()
        {
            // Arrange
            User userA = new()
            {
                Id = "userAId"
            };

            User userB = new()
            {
                Id = "userBId"
            };

            User userC = new()
            {
                Id = "userCId"
            };

            DateTime dateTime = DateTime.UtcNow;

            mockUserFriendsRepository.Setup(ufr => ufr.GetUserFriends(It.IsAny<User>()))
                .ReturnsAsync(new List<UserFriendsResult>());

            mockUserRepository
                .Setup(ur => ur.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(userC);

            UserFriendsService userFriendsService = new(mockUserRepository.Object, mockUserFriendsRepository.Object, mockUserFriendsLogger.Object, mockUserManager.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(async () =>
            {
                await userFriendsService.GetUserFriends(new ClaimsPrincipal());
            });
            
        }
    }
}
