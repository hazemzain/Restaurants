using Xunit;
using Restaurants.Infrastructure.Authorization.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Restaurants.Application.Users;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using FluentAssertions;

namespace Restaurants.Infrastructure.Authorization.Requirements.Tests
{
    public class CreatedMultipleRestaurantsRequirementHandlerTests
    {
        [Fact()]
        public async Task HandleRequirementAsync_UserHasNotCreatedMultipleRestaurants_ShouldFail()
        {
            // arrange

            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new()
                {
                    OwnerId = currentUser.Id,
                },
                new()
                {
                    OwnerId = "2",
                },
            };

            var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
            restaurantsRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirement = new CreatedMultipleRestaurantsRequirement(2);
            var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object,
                userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirement], null, null);

            // act

            await handler.HandleAsync(context);

            // assert

            context.HasSucceeded.Should().BeFalse();
            context.HasFailed.Should().BeTrue();

        }

        [Fact()]
        public async Task HandleRequirementAsync_UserHasCreatedMultipleRestaurants_ShouldSucceed()
        {
            // arrange

            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new()
                {
                    OwnerId = currentUser.Id,
                },
                new()
                {
                    OwnerId = currentUser.Id,
                },
                new()
                {
                    OwnerId = "2",
                },
            };

            var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
            restaurantsRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirement = new CreatedMultipleRestaurantsRequirement(2);
            var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object,
                userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirement], null, null);

            // act

            await handler.HandleAsync(context);
            
            // assert
            
            context.HasSucceeded.Should().BeTrue();

        }
        [Fact]
        public async Task HandleRequirementAsync_WhenUserContextReturnsNull_ShouldFail()
        {
            // arrange
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(m => m.GetCurrentUser()).Returns((CurrentUser?)null);

            var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
            var requirement = new CreatedMultipleRestaurantsRequirement(2);
            var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirement], null, null);

            // act
            await handler.HandleAsync(context);

            // assert
            context.HasFailed.Should().BeTrue();
            context.HasSucceeded.Should().BeFalse();
        }

        [Fact]
        public async Task HandleRequirementAsync_WhenNoRestaurantsExist_ShouldFail()
        {
            // arrange
            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

            var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
            restaurantsRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Restaurant>()); // Empty list

            var requirement = new CreatedMultipleRestaurantsRequirement(2);
            var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirement], null, null);

            // act
            await handler.HandleAsync(context);

            // assert
            context.HasFailed.Should().BeTrue();
        }
        [Fact]
        public async Task HandleRequirementAsync_WhenRepositoryThrowsException_ShouldFail()
        {
            // arrange
            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

            var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
            restaurantsRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            var requirement = new CreatedMultipleRestaurantsRequirement(2);
            var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirement], null, null);

            // act & assert
            await Assert.ThrowsAsync<Exception>(() => handler.HandleAsync(context));
        }
        [Fact]
        public async Task HandleRequirementAsync_UserHasCreatedExactNumberOfRequiredRestaurants_ShouldSucceed()
        {
            // arrange
            var currentUser = new CurrentUser("1", "test@test.com", [], null, null);
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(m => m.GetCurrentUser()).Returns(currentUser);

            var restaurants = new List<Restaurant>()
            {
                new() { OwnerId = currentUser.Id },
                new() { OwnerId = currentUser.Id }
            };

            var restaurantsRepositoryMock = new Mock<IRestaurantsRepository>();
            restaurantsRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(restaurants);

            var requirement = new CreatedMultipleRestaurantsRequirement(2);
            var handler = new CreatedMultipleRestaurantsRequirementHandler(restaurantsRepositoryMock.Object, userContextMock.Object);
            var context = new AuthorizationHandlerContext([requirement], null, null);

            // act
            await handler.HandleAsync(context);

            // assert
            context.HasSucceeded.Should().BeTrue();
        }



    }
}