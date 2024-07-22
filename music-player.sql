CREATE DATABASE MusicPlayer

USE MusicPlayer


CREATE TABLE [User] (
	userId INT IDENTITY(1, 1) PRIMARY KEY,
	username NVARCHAR(MAX),
	fullName NVARCHAR(MAX),
	[password] VARCHAR(30)
)

CREATE TABLE Playlist (
	playlistId INT IDENTITY(1, 1) PRIMARY KEY,
	playlistName NVARCHAR(MAX),
	createdDate DATETIME,
	[status] bit
)

CREATE TABLE Music (
	musicId INT IDENTITY(1, 1) PRIMARY KEY,
	musicName NVARCHAR(MAX),
	link VARCHAR(MAX),
	createdDate DATETIME,
	[status] bit
)

CREATE TABLE PlaylistMusic(
	id INT IDENTITY(1, 1) PRIMARY KEY,
	musicId INT REFERENCES Music(musicId),
	playlistId INT REFERENCES Playlist(playlistId)
)