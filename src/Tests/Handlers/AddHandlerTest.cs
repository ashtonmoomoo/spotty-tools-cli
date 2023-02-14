using Xunit;
using Moq;

using Application.Handlers;
using Application.CLI.Arguments;
using Application.Interfaces;

using Tests.Factories;

namespace Tests.Handlers;

public class AddHandlerTests
{
  [Fact]
  public async void Dispatch_EmptyArgument_Returns1()
  {
    var mockClient = new Mock<IClient>();
    var argParser = new ArgumentParser(new string[1] { "" });

    var result = await AddHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(1, result);
  }

  [Fact]
  public async void Dispatch_InvalidArgument_Returns1()
  {
    var mockClient = new Mock<IClient>();
    var argParser = new ArgumentParser(new string[1] { "invalid resource" });

    var result = await AddHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(1, result);
  }

  [Fact]
  public async void Dispatch_InvalidDestination_Returns1()
  {
    var mockClient = new Mock<IClient>();
    var argParser = new ArgumentParser(new string[2] { "albums", "" });

    var result = await AddHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(1, result);
  }

  [Fact]
  public async void Dispatch_LessThanOneBatch_AddsAlbumsToPlaylist()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);

    var mockCreatedPlaylistId = "playlistId";

    var albums = AlbumFactory.MakeAlbums("mockAlbum", 3, 10);
    mockClient.Setup(mock => mock.GetAlbums()).ReturnsAsync(albums);
    mockClient.Setup(mock => mock.CreatePlaylist(It.IsAny<string>())).ReturnsAsync(mockCreatedPlaylistId);

    var argParser = new ArgumentParser(new string[2] { "albums", "mock playlist" });

    var result = await AddHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(0, result);

    var invocations = mockClient.Invocations;
    var addToPlaylistInvocations = invocations.Where(i => i.Method.Name == "AddSongsToPlaylist");

    Assert.Single(addToPlaylistInvocations);

    var theInvocation = addToPlaylistInvocations.First();
    var args = theInvocation.Arguments;

    Assert.Equal(2, args.Count());

    var trackIds = albums.SelectMany(a => a.TracksPage.Items).Select(i => i.Id).ToList();

    Assert.Equal(trackIds, args[0]);
    Assert.Equal(mockCreatedPlaylistId, args[1]);
  }

  [Fact]
  public async void Dispatch_MoreThanOneBatch_AddsAlbumsToPlaylist()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);

    var mockCreatedPlaylistId = "playlistId";

    // 180 songs total - requires 2 batches
    var albums = AlbumFactory.MakeAlbums("mockAlbum", 18, 10);
    mockClient.Setup(mock => mock.GetAlbums()).ReturnsAsync(albums);
    mockClient.Setup(mock => mock.CreatePlaylist(It.IsAny<string>())).ReturnsAsync(mockCreatedPlaylistId);

    var argParser = new ArgumentParser(new string[2] { "albums", "mock playlist" });

    var result = await AddHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(0, result);

    var invocations = mockClient.Invocations;
    var addToPlaylistInvocations = invocations.Where(i => i.Method.Name == "AddSongsToPlaylist").ToList();

    Assert.Equal(2, addToPlaylistInvocations.Count());

    var firstInvocation = addToPlaylistInvocations[0];
    var secondInvocation = addToPlaylistInvocations[1];

    var firstArgs = firstInvocation.Arguments;
    var secondArgs = secondInvocation.Arguments;

    var allTrackIds = albums.SelectMany(a => a.TracksPage.Items).Select(i => i.URI).ToList();

    var batchSize = Application.Spotify.Constants.Playlist.MAX_SONGS_TO_ADD;

    Assert.Equal(allTrackIds.Take(batchSize).ToList(), firstArgs[0]);
    Assert.Equal(mockCreatedPlaylistId, firstArgs[1]);

    Assert.Equal(allTrackIds.Skip(batchSize).Take(batchSize).ToList(), secondArgs[0]);
    Assert.Equal(mockCreatedPlaylistId, secondArgs[1]);
  }

  [Fact]
  public async void Dispatch_MoreThanOneSuperBatch_AddsAlbumsToPlaylists()
  {
    var mockClient = new Mock<IClient>();
    mockClient.Setup(mock => mock.IsLoggedIn()).Returns(true);

    var mockCreatedPlaylistId1 = "playlistId1";
    var mockCreatedPlaylistId2 = "playlistId2";

    // 11200 songs total - requires 2 super batches + 2 more normal batches
    var albums = AlbumFactory.MakeAlbums("mockAlbum", 1120, 10);
    mockClient.Setup(mock => mock.GetAlbums()).ReturnsAsync(albums);
    mockClient.SetupSequence(mock => mock.CreatePlaylist(It.IsAny<string>()))
      .ReturnsAsync(mockCreatedPlaylistId1)
      .ReturnsAsync(mockCreatedPlaylistId2);

    var argParser = new ArgumentParser(new string[2] { "albums", "mock playlist" });

    var result = await AddHandler.Dispatch(mockClient.Object, argParser);

    Assert.Equal(0, result);

    var invocations = mockClient.Invocations;
    var addToPlaylistInvocations = invocations.Where(i => i.Method.Name == "AddSongsToPlaylist").ToList();

    Assert.Equal(112, addToPlaylistInvocations.Count());

    var createPlaylistInvocations = invocations.Where(i => i.Method.Name == "CreatePlaylist").ToList();
    Assert.Equal(2, createPlaylistInvocations.Count());

    Assert.Equal("mock playlist Part 1", createPlaylistInvocations[0].Arguments[0]);
    Assert.Equal("mock playlist Part 2", createPlaylistInvocations[1].Arguments[0]);
  }
}
