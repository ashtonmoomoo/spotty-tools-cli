using Xunit;
using Application.CLI.Arguments;

namespace Tests.CLI.Arguments;

public class ArgumentParserTests
{
  [Fact]
  public void NextArg_WithNoArgs_ThrowsArgumentLengthException()
  {
    var argParser = new ArgumentParser(new string[0]);

    Assert.Throws<ArgumentsLengthException>(() => argParser.NextArg());
  }

  [Fact]
  public void NextArg_WithArgs_ReturnsTheArgsInTheRightOrder()
  {
    var args = new string[2] { "first", "second" };

    var argParser = new ArgumentParser(args);

    Assert.Equal("first", argParser.NextArg());
    Assert.Equal("second", argParser.NextArg());
  }
}
