using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Xunit;

namespace Restaurants.Application.Restaurants.Commands.CreateRestaurant.Tests;

public class CreateRestaurantCommandHandlerTests
{
    [Fact()]
    public async Task Handle_ForValidCommand_ReturnsCreatedRestaurantId()
    {
        // arrange
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
        var mapperMock = new Mock<IMapper>();

        var command = new CreateRestaurantCommand();
        var restaurant = new Restaurant();

        mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);

        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();
        restaurantRepositoryMock
            .Setup(repo => repo.Create(It.IsAny<Restaurant>()))
            .ReturnsAsync(1);

        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);


        var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object, 
            mapperMock.Object, 
            restaurantRepositoryMock.Object,
            userContextMock.Object);

        // act
        var result = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        result.Should().Be(1);
        restaurant.OwnerId.Should().Be("owner-id");
        restaurantRepositoryMock.Verify(r => r.Create(restaurant), Times.Once);
    }
    [Fact]
    public async Task Handle_WhenUserContextReturnsNull_ShouldThrowInvalidOperationException()
    {
        // arrange
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

        var userContextMock = new Mock<IUserContext>();
        userContextMock.Setup(u => u.GetCurrentUser()).Returns((CurrentUser?)null);

        var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object,
            mapperMock.Object,
            restaurantRepositoryMock.Object,
            userContextMock.Object);

        var command = new CreateRestaurantCommand();

        // act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenRepositoryThrowsException_ShouldThrowException()
    {
        // arrange
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();

        var command = new CreateRestaurantCommand();
        var restaurant = new Restaurant();

        mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);

        restaurantRepositoryMock
            .Setup(repo => repo.Create(It.IsAny<Restaurant>()))
            .ThrowsAsync(new Exception("Database error"));

        var userContextMock = new Mock<IUserContext>();
        var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object,
            mapperMock.Object,
            restaurantRepositoryMock.Object,
            userContextMock.Object);

        // act & assert
        await Assert.ThrowsAsync<Exception>(() => commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenMapperThrowsException_ShouldThrowException()
    {
        // arrange
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();
        var userContextMock = new Mock<IUserContext>();

        var command = new CreateRestaurantCommand();

        mapperMock.Setup(m => m.Map<Restaurant>(command))
            .Throws(new Exception("Mapping failure"));

        var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object,
            mapperMock.Object,
            restaurantRepositoryMock.Object,
            userContextMock.Object);

        // act & assert
        await Assert.ThrowsAsync<Exception>(() => commandHandler.Handle(command, CancellationToken.None));
    }
    [Fact]
    public async Task Handle_WhenCommandHasNullProperties_ShouldStillProceed()
    {
        // arrange
        var loggerMock = new Mock<ILogger<CreateRestaurantCommandHandler>>();
        var mapperMock = new Mock<IMapper>();
        var restaurantRepositoryMock = new Mock<IRestaurantsRepository>();
        var userContextMock = new Mock<IUserContext>();

        var command = new CreateRestaurantCommand();  // Missing required fields

        var restaurant = new Restaurant();
        mapperMock.Setup(m => m.Map<Restaurant>(command)).Returns(restaurant);

        restaurantRepositoryMock.Setup(repo => repo.Create(It.IsAny<Restaurant>())).ReturnsAsync(1);

        var currentUser = new CurrentUser("owner-id", "test@test.com", [], null, null);
        userContextMock.Setup(u => u.GetCurrentUser()).Returns(currentUser);

        var commandHandler = new CreateRestaurantCommandHandler(loggerMock.Object,
            mapperMock.Object,
            restaurantRepositoryMock.Object,
            userContextMock.Object);

        // act
        var result = await commandHandler.Handle(command, CancellationToken.None);

        // assert
        result.Should().Be(1);
    }



}