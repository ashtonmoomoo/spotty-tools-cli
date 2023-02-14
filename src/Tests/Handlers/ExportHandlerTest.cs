using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;

using Application.Interfaces;
using Application.Handlers;
using Application.CLI.Arguments;
using Application.Spotify.Exceptions;
using Application.Spotify.Responses;

using Tests.Factories;

namespace Tests.Handlers;

public class ExportHandlerTests
{
  [Fact]
  public async void Dispatch_WhenNotLoggedIn_Returns1()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(false);

    var argParser = new ArgumentParser(new string[0]);

    var result = await ExportHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(1, result);
  }

  [Fact]
  public async void Dispatch_WhenLoggedInUnsupportedArgument_Returns1()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);

    var argParser = new ArgumentParser(new string[1] { "mock" });

    var result = await ExportHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(1, result);
  }

  [Fact]
  public async void Dispatch_WhenLoggedInPlaylistArgPlaylistNotFound_ThrowsNotFoundException()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);
    mockClient.Setup(mock => mock.GetPlaylists()).ReturnsAsync(new List<PlaylistLite>());

    var argParser = new ArgumentParser(new string[3] { "playlist", "doesn't exist playlist", "somePath" });

    await Assert.ThrowsAsync<PlaylistNotFoundException>(() => ExportHandler.Dispatch(mockClient.Object, argParser));
  }

  [Fact]
  public async void Dispatch_WhenLoggedInPlaylistArgPlaylistExists_ExportsThePlaylist()
  {
    var mockPlaylists = new List<PlaylistLite>()
    {
      PlaylistFactory.MakePlaylistLite("mock playlist 1"),
      PlaylistFactory.MakePlaylistLite("mock playlist 2")
    };

    var mockPlaylistTracks = TrackFactory.MakeTracks("playlist 2 tracks", 2);

    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);
    mockClient.Setup(mock => mock.GetPlaylists()).ReturnsAsync(mockPlaylists);
    mockClient.Setup(mock => mock.GetPlaylistTracks(mockPlaylists[1].Id)).ReturnsAsync(mockPlaylistTracks);

    var mockPath = "mockPath";

    var mockFileWriter = new Mock<IFileWriter>();
    mockFileWriter.Setup(mock => mock.WriteText(mockPath, It.IsAny<string>()));

    var argParser = new ArgumentParser(new string[3] { "playlist", "mock playlist 2", mockPath });

    var result = await ExportHandler.Dispatch(mockClient.Object, argParser, mockFileWriter.Object);

    Assert.Equal(0, result);

    var invocations = mockFileWriter.Invocations;
    Assert.Single(invocations);

    var invocationArgs = invocations[0].Arguments;
    Assert.Equal(mockPath, invocationArgs[0]);

    var csvContent = invocationArgs[1];
    Assert.Equal(
      $@"uri,name,album,artist
""{mockPlaylistTracks[0].URI}"",""{mockPlaylistTracks[0].Name}"",""{mockPlaylistTracks[0].Album.Name}"",""{mockPlaylistTracks[0].Artists[0].Name}""
""{mockPlaylistTracks[1].URI}"",""{mockPlaylistTracks[1].Name}"",""{mockPlaylistTracks[1].Album.Name}"",""{mockPlaylistTracks[1].Artists[0].Name}""
",
      csvContent
    );
  }
}
