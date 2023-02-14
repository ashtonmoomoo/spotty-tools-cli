using Moq;
using Xunit;
using System.Threading.Tasks;
using System.Collections.Generic;
using Application.Commands;
using Application.Interfaces;
using Application.CLI.Arguments;

namespace Tests.Dispatch;

public class DispatchTests
{
  [Fact]
  public async Task GetDispatcher_WhenTheCommandExists_ItReturnsTheDispatcher()
  {
    var mockCommand = new Mock<ICommand>();
    mockCommand.Setup(mock => mock.Alias).Returns("mock");
    mockCommand.Setup(mock => mock.Description).Returns("mock description");
    mockCommand.Setup(mock => mock.GetDispatcher()).Returns((IClient _, ArgumentParser _) => Task.FromResult(100));

    var mockClient = new Mock<IClient>();
    var mockArgParser = new ArgumentParser(new List<string>().ToArray());

    ICommand[] commands = { mockCommand.Object };

    var commandDispatcher = Application.Dispatch.Dispatch.GetDispatcher("mock", commands);

    var result = await commandDispatcher(mockClient.Object, mockArgParser);

    Assert.Equal(100, result);
  }

  [Fact]
  public async Task GetDispatcher_WhenTheCommandDoesntExist_ItReturnsTheErrorDispatcher()
  {
    var mockClient = new Mock<IClient>();
    var mockArgParser = new ArgumentParser(new List<string>().ToArray());

    var commandDispatcher = Application.Dispatch.Dispatch.GetDispatcher("mock", new ICommand[0]);

    var result = await commandDispatcher(mockClient.Object, mockArgParser);

    Assert.Equal(1, result);
  }
}
