using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using MusicPlayer.BLL.Services;
using MusicPlayer.DAL;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MusicService _service = new();
        private UserService _userService = new();
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        public User CurrentUser { get; set; }



        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void SongsButton_Click(object sender, RoutedEventArgs e)
        {
            SongsView(Visibility.Visible);
            PlaylistView(Visibility.Collapsed);
            LoadData();
        }

        private void UploadSongButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Music Files (*.mp3;*.wav)|*.mp3;*.wav"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var sourcePath = openFileDialog.FileName;
                var fileName = Path.GetFileName(sourcePath);

                // Đọc metadata từ file MP3
                var tagFile = TagLib.File.Create(sourcePath);
                var musicName = tagFile.Tag.Title ?? "Unknown Title";
                var artistNames = tagFile.Tag.Performers;
                var artistName = artistNames.Length > 0 ? string.Join(", ", artistNames) : "Unknown Artist";

                var storagePath = GetStoragePath();
                if (string.IsNullOrEmpty(storagePath))
                {
                    MessageBox.Show("Storage path is not configured correctly.");
                    return;
                }

                var userFolder = Path.Combine(storagePath, CurrentUser?.Username ?? "default");
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                var destinationPath = Path.Combine(userFolder, fileName);
                try
                {
                    File.Copy(sourcePath, destinationPath);
                }
                catch (Exception)
                {
                    MessageBox.Show("Music is already exist!");
                    return;
                }

                var music = new Music
                {
                    MusicName = musicName,
                    ArtistName = artistName,
                    CreatedDate = DateTime.Now,
                    Status = true,
                    Link = destinationPath,
                    UserId = CurrentUser?.UserId ?? 0 // Gán trực tiếp UserId
                };

                _service.AddMusic(music);
                LoadData();
            }
        }

        private void DeleteSongButton_Click(object sender, RoutedEventArgs e)
        {
            var music = MusicsListView.SelectedItem as Music;
            if (music == null)
            {
                MessageBox.Show("Please select a music to delete!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = MessageBox.Show("Confirm delete!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            _service.DeleteMusic(music);
            LoadData();
        }

        private void SongsView(Visibility visibility)
        {
            if (visibility.Equals(Visibility.Visible))
            {
                SectionTitle.Text = "Songs";
            }
            MusicsListView.Visibility = visibility;
            UploadSongButton.Visibility = visibility;
            DeleteSongButton.Visibility = visibility;
        }

        private void PlaylistView(Visibility visibility)
        {
            if (visibility.Equals(Visibility.Visible))
            {
                SectionTitle.Text = "Playlist";
            }
            PlaylistSection.Visibility = visibility;
            LoadPlaylists();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null)
            {
                return;
            }
            WelcomeNameTextBlock.Text = CurrentUser.FullName;
            SectionTitle.Text = "Home";
            SongsView(Visibility.Collapsed);
            PlaylistView(Visibility.Collapsed);
            LoadData();
            LoadPlaylists();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SectionTitle.Text = "Home";
            SongsView(Visibility.Collapsed);
            PlaylistView(Visibility.Collapsed);
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Close();
            LoginWindow loginWindow = new();
            loginWindow.Show();
            Close();

        }

        private void LoadPlaylists()
        {
            PlaylistsListView.ItemsSource = null;
            PlaylistsListView.ItemsSource = _userService.GettAllPlaylistByUsername(CurrentUser.Username);
        }

        private void LoadData()
        {
            MusicsListView.ItemsSource = null;
            MusicsListView.ItemsSource = _userService.GetAllMusicsByUsername(CurrentUser.Username);
        }

        private string? GetStoragePath()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            return config["StorageSettings:MusicStoragePath"];
        }

        private void PlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlaylistView(Visibility.Visible);
            SongsView(Visibility.Collapsed);
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            PlayListWindow playListWindow = new PlayListWindow();
            playListWindow.CurrentUser = CurrentUser;
            playListWindow.ShowDialog();
            LoadPlaylists();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var music = button.DataContext as Music; // Assuming 'Music' is your model
                if (music != null)
                {
                    SongNameTextBlock.Text = music.MusicName;
                    ArtistNameTextBlock.Text = music.ArtistName;
                    MusicPlayerControls.Visibility = Visibility.Visible;

                    // Additional logic to play the music
                    PlayMusic(music);
                }
            }
        }

        private void PlayMusic(Music music)
        {
            if (File.Exists(music.Link))
            {
                _mediaPlayer.Open(new Uri(music.Link));
                _mediaPlayer.Play();
                MessageBox.Show($"Playing {music.MusicName} by {music.ArtistName}");
            }
            else
            {
                MessageBox.Show("Music file not found!");
            }
        }

        // Event handlers for music controls
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic to play previous song
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (PauseButton.Content.ToString() == "Pause")
            {
                _mediaPlayer.Pause();
                PauseButton.Content = "Play";
            }
            else
            {
                _mediaPlayer.Play();
                PauseButton.Content = "Pause";
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            // Logic to play next song
        }
    }
}