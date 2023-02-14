using Xunit;
using Moq;

using Application.Interfaces;
using Application.Handlers;

namespace Tests.Handlers;

public class LoginHandlerTests
{
  [Fact]
  public async void Dispatch_AlreadyLoggedIn_Returns1()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);

    var result = await LoginHandler.Dispatch(mockClient.Object);

    Assert.Equal(1, result);
  }

  [Fact]
  public async void Dispatch_NotLoggedIn_CallsLoginAndReturns0()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(false);

    var result = await LoginHandler.Dispatch(mockClient.Object);

    var invocations = mockClient.Invocations;

    Assert.Equal("IsLoggedIn", invocations[0].Method.Name);
    Assert.Equal("Login", invocations[1].Method.Name);
  }
}