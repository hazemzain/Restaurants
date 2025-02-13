using Xunit;
using Restaurants.Application.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Restaurants.Domain.Constants;
using FluentAssertions;

namespace Restaurants.Application.Users.Tests
{
    public class UserContextTests
    {
        [Fact()]
        public void GetCurrentUser_WithAuthenticatedUser_ShouldReturnCurrentUser()
        {
            // arrange
            var dateOfBirth = new DateOnly(1990, 1, 1);

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, "1"),
                new(ClaimTypes.Email, "test@test.com"),
                new(ClaimTypes.Role, UserRoles.Admin),
                new(ClaimTypes.Role, UserRoles.User),
                new("Nationality", "German"),
                new("DateOfBirth", dateOfBirth.ToString("yyyy-MM-dd"))
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));

            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext()
            {
                User = user
            });

            var userContext = new UserContext(httpContextAccessorMock.Object);

            // act
            var currentUser = userContext.GetCurrentUser();


            // asset

            currentUser.Should().NotBeNull();
            currentUser.Id.Should().Be("1");
            currentUser.Email.Should().Be("test@test.com");
            currentUser.Roles.Should().ContainInOrder(UserRoles.Admin, UserRoles.User);
            currentUser.Nationality.Should().Be("German");
            currentUser.DateOfBirth.Should().Be(dateOfBirth);


        }

        [Fact]
        public void GetCurrentUser_WithUserContextNotPresent_ThrowsInvalidOperationException()
        {
            // Arrange
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null);

            var userContext = new UserContext(httpContextAccessorMock.Object);

            // act

            Action action = () => userContext.GetCurrentUser();

            // assert 

            action.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("User context is not present");
        }
        [Fact]
        public void GetCurrentUser_WhenHttpContextIsNull_ShouldThrowInvalidOperationException()
        {
            // arrange
            var httpContextAccessor = new HttpContextAccessor { HttpContext = null };
            var userContext = new UserContext(httpContextAccessor);

            // act & assert
            Assert.Throws<InvalidOperationException>(() => userContext.GetCurrentUser());
        }

        [Fact]
        public void GetCurrentUser_WhenUserIdentityIsNull_ShouldReturnNull()
        {
            // arrange
            var user = new ClaimsPrincipal(); // No identity set
            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var userContext = new UserContext(httpContextAccessor);

            // act
            var result = userContext.GetCurrentUser();

            // assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetCurrentUser_WhenUserIsNotAuthenticated_ShouldReturnNull()
        {
            // arrange
            var identity = new ClaimsIdentity(); // Unauthenticated
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var userContext = new UserContext(httpContextAccessor);

            // act
            var result = userContext.GetCurrentUser();

            // assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetCurrentUser_WhenUserHasNoRoles_ShouldReturnUserWithEmptyRoles()
        {
            // arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@test.com")
                // No roles added
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var userContext = new UserContext(httpContextAccessor);

            // act
            var result = userContext.GetCurrentUser();

            // assert
            result.Should().NotBeNull();
            result!.Roles.Should().BeEmpty();
        }
        [Fact]
        public void GetCurrentUser_WhenUserHasNoIdOrEmail_ShouldThrowException()
        {
            // arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Admin") // No ID or Email
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var userContext = new UserContext(httpContextAccessor);

            // act & assert
            Assert.Throws<NullReferenceException>(() => userContext.GetCurrentUser());
        }
        [Fact]
        public void GetCurrentUser_WhenUserHasNationality_ShouldReturnCorrectNationality()
        {
            // arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@test.com"),
                new Claim("Nationality", "Egyptian")
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var userContext = new UserContext(httpContextAccessor);

            // act
            var result = userContext.GetCurrentUser();

            // assert
            result.Should().NotBeNull();
            result!.Nationality.Should().Be("Egyptian");
        }
        [Fact]
        public void GetCurrentUser_WhenDateOfBirthIsInvalid_ShouldThrowException()
        {
            // arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Email, "test@test.com"),
                new Claim("DateOfBirth", "invalid-date")
            };

            var identity = new ClaimsIdentity(claims, "test");
            var user = new ClaimsPrincipal(identity);
            var httpContext = new DefaultHttpContext { User = user };
            var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };
            var userContext = new UserContext(httpContextAccessor);

            // act & assert
            Assert.Throws<FormatException>(() => userContext.GetCurrentUser());
        }



    }


}