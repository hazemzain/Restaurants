using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Restaurants.Application.Dishes.Commands.CreateDish;
using Restaurants.Application.Dishes.Commands.DeleteDishes;
using Restaurants.Application.Dishes.Dtos;
using Restaurants.Application.Dishes.Queries.GetDishByIdForRestaurant;
using Restaurants.Application.Dishes.Queries.GetDishesForRestaurant;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace Restaurants.API.Tests.Controllers
{
    public class DishesControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly Mock<IMediator> _mediatorMock = new();

        public DishesControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.Replace(ServiceDescriptor.Scoped(typeof(IMediator), _ => _mediatorMock.Object));
                });
            });
        }

        [Fact]
        public async Task CreateDish_ShouldReturn201Created()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            var command = new CreateDishCommand { Name = "New Dish", Description = "Test", Price = 10.99M, KiloCalories = 500 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDishCommand>(), default)).ReturnsAsync(1);

            // Act
            var response = await client.PostAsJsonAsync("/api/restaurants/1/dishes", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        

        [Fact]
        public async Task GetAllForRestaurant_ShouldReturn200Ok()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            var dishes = new List<DishDto> { new DishDto { Id = 1, Name = "Dish 1", Description = "Test", Price = 10.99M, KiloCalories = 500 } };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDishesForRestaurantQuery>(), default)).ReturnsAsync(dishes);

            // Act
            var response = await client.GetAsync("/api/restaurants/1/dishes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<DishDto>>();
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAllForRestaurant_ShouldReturn403Forbidden_WhenUserNotAuthorized()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/restaurants/1/dishes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetByIdForRestaurant_ShouldReturn200Ok()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            var dish = new DishDto { Id = 1, Name = "Dish 1", Description = "Test", Price = 10.99M, KiloCalories = 500 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDishByIdForRestaurantQuery>(), default)).ReturnsAsync(dish);

            // Act
            var response = await client.GetAsync("/api/restaurants/1/dishes/1");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<DishDto>();
            result.Should().NotBeNull();
            result.Name.Should().Be("Dish 1");
        }

        [Fact]
        public async Task GetByIdForRestaurant_ShouldReturn404NotFound_WhenDishDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetDishByIdForRestaurantQuery>(), default)).ReturnsAsync((DishDto)null);

            // Act
            var response = await client.GetAsync("/api/restaurants/1/dishes/99");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        

        [Fact]
        public async Task DeleteDishesForRestaurant_ShouldReturn404NotFound_WhenRestaurantDoesNotExist()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "valid-token");

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDishesForRestaurantCommand>(), default))
                .ThrowsAsync(new KeyNotFoundException("Restaurant not found"));

            // Act
            var response = await client.DeleteAsync("/api/restaurants/99/dishes");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
