# spotty-tools-cli
`spotty-tools-cli` is a command line tool that offers a collection of utilities for your Spotify library.

Plz be kind! I am using this application to learn C#.

## Getting started
**Note:** I haven't yet looked into building/packaging the application, so it will need to be run in development mode 😅

### Prerequisites
You will need a Spotify Premium account for these tools to work.
Specifically, you will need to create a "Spotify app" via [Spotify Developer Dashboard](https://developer.spotify.com/dashboard/applications).
After you have done this, you will get a Client ID and a Client Secret for your app.
`spotty-tools-cli` will load these values from the `SPOTIFY_CLIENT_ID` and `SPOTIFY_CLIENT_SECRET` environment variables, so make sure you have set these appropriately when you run the tools.
You will need to edit the settings of your Spotify app to configure an appropriate redirect URI.
`spotty-tools-cli` will have a default redirect URI of `http://localhost:3002`, but you can make it whatever you like (on `localhost`) by setting the `SPOTIFY_REDIRECT_URI` environment variable.
Just make sure it matches what you configured in the Spotify app!

Additionally, you will need a suitable `dotnet` runtime installed on your machine.
On MacOS with `brew` installed, doing `brew install dotnet@6` and `brew install —cask dotnet-sdk` should be sufficient.

### Using the tools
Assuming you have `git clone`'d this repo, and you are in `src/Application`, you will be able to run the app by simply doing `dotnet run <args>`.
(Once I actually build/bundle the application, this will change!)
Typically, the first command you will run is `dotnet run login`, which will initialise the authorization flow where you can grant `spotty-tools-cli` access to your Spotify account.
To see a list of available commands, do `dotnet run help`.

**Note:** At the moment, `spotty-tools-cli` will store your session in a file in `~/.spotty/.session` so that you won't need to log in every time you use the application.
Later I will add an optional flag to disable this behaviour (and let the user manually authorize each time), but at the moment, you can either delete the session folder/file manually, or do `dotnet run logout` which does the same thing.

### Running tests
From `src/Tests`, `dotnet test` will run all of the tests.

## Examples
- `dotnet run login`
- `dotnet run logout`
- `dotnet run export playlist <playlist-name> <destination-file-name>`
- `dotnet run help`

## Features
- [x] Authorization Code flow with state and refresh tokens
- [x] Export a playlist to CSV by name
- [x] Add all songs from liked albums to delegate playlist
- [ ] Export library to CSV
- [ ] Export a list of playlists by comma-separated names
- [ ] View list of most recent _n_ playlists
