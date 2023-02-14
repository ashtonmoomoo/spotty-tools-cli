using Xunit;
using Moq;

using Application.Interfaces;
using Application.Handlers;

namespace Tests.Handlers;

public class LogoutHandlerTests
{
  [Fact]
  public void Dispatch_IsLoggedIn_Returns0()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);

    var result = LogoutHandler.Dispatch(mockClient.Object);

    var invocations = mockClient.Invocations;
    Assert.Equal("IsLoggedIn", invocations[0].Method.Name);
    Assert.Equal("Logout", invocations[1].Method.Name);

    Assert.Equal(0, result);
  }

  [Fact]
  public void Dispatch_NotLoggedIn_Returns1()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(false);

    var result = LogoutHandler.Dispatch(mockClient.Object);

    Assert.Equal(1, result);
  }
}
