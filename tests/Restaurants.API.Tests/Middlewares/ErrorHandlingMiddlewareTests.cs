﻿using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Restaurants.Domain.Entities;
using Restaurants.Domain.Exceptions;
using FluentAssertions;

namespace Restaurants.API.Middlewares.Tests
{
    public class ErrorHandlingMiddlewareTests
    {
        [Fact()]
        public async Task InvokeAsync_WhenNoExceptionThrown_ShouldCallNextDelegate()
        {
            // arrange

            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var nextDelegateMock = new Mock<RequestDelegate>();

            // act

            await middleware.InvokeAsync(context, nextDelegateMock.Object);

            // assert

            nextDelegateMock.Verify(next => next.Invoke(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldSetStatusCode404()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var notFoundException = new NotFoundException(nameof(Restaurant), "1");

            // act
            await middleware.InvokeAsync(context, _ => throw notFoundException);

            // Assert
            context.Response.StatusCode.Should().Be(404);

        }

        [Fact]
        public async Task InvokeAsync_WhenForbidExceptionThrown_ShouldSetStatusCode403()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var exception = new ForbidException();

            // act
            await middleware.InvokeAsync(context, _ => throw exception);

            // Assert
            context.Response.StatusCode.Should().Be(403);

        }

        [Fact]
        public async Task InvokeAsync_WhenGenericExceptionThrown_ShouldSetStatusCode500()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var exception = new Exception();

            // act
            await middleware.InvokeAsync(context, _ => throw exception);

            // Assert
            context.Response.StatusCode.Should().Be(500);

        }
        [Fact]
        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldReturnErrorMessageInBody()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var notFoundException = new NotFoundException(nameof(Restaurant), "1");

            var expectedMessage = notFoundException.Message;
            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // Act
            await middleware.InvokeAsync(context, _ => throw notFoundException);

            // Assert
            context.Response.StatusCode.Should().Be(404);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memoryStream);
            var responseBody = await reader.ReadToEndAsync();

            responseBody.Should().Be(expectedMessage);
        }
        [Fact]
        public async Task InvokeAsync_WhenForbidExceptionThrown_ShouldReturnAccessForbiddenMessage()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var exception = new ForbidException();

            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // Act
            await middleware.InvokeAsync(context, _ => throw exception);

            // Assert
            context.Response.StatusCode.Should().Be(403);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memoryStream);
            var responseBody = await reader.ReadToEndAsync();

            responseBody.Should().Be("Access forbidden");
        }
        [Fact]
        public async Task InvokeAsync_WhenGenericExceptionThrown_ShouldReturnGenericErrorMessage()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var exception = new Exception("Unexpected error");

            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // Act
            await middleware.InvokeAsync(context, _ => throw exception);

            // Assert
            context.Response.StatusCode.Should().Be(500);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memoryStream);
            var responseBody = await reader.ReadToEndAsync();

            responseBody.Should().Be("Something went wrong");
        }
        [Fact]
        public async Task InvokeAsync_WhenNotFoundExceptionThrown_ShouldLogWarning()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var notFoundException = new NotFoundException(nameof(Restaurant), "1");

            // Act
            await middleware.InvokeAsync(context, _ => throw notFoundException);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(notFoundException.Message)),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }
        [Fact]
        public async Task InvokeAsync_WhenGenericExceptionThrown_ShouldLogError()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ErrorHandlingMiddleware>>();
            var middleware = new ErrorHandlingMiddleware(loggerMock.Object);
            var context = new DefaultHttpContext();
            var exception = new Exception("Critical failure");

            // Act
            await middleware.InvokeAsync(context, _ => throw exception);

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Critical failure")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
                Times.Once
            );
        }




    }
}