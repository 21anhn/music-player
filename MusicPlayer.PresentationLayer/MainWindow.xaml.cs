using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
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
            SectionTitle.Text = "Songs";
            MusicsListView.Visibility = Visibility.Visible;
            UploadSongButton.Visibility = Visibility.Visible;
            DeleteSongButton.Visibility = Visibility.Visible;
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
                var artistNames = tagFile.Tag.Artists;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentUser == null)
            {
                return;
            }
            WelcomeNameTextBlock.Text = CurrentUser.FullName;
            SectionTitle.Text = "Home";
            MusicsListView.Visibility = Visibility.Collapsed;
            UploadSongButton.Visibility = Visibility.Collapsed;
            DeleteSongButton.Visibility = Visibility.Collapsed;
            LoadData();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            SectionTitle.Text = "Home";
            MusicsListView.Visibility = Visibility.Collapsed;
            UploadSongButton.Visibility = Visibility.Collapsed;
            DeleteSongButton.Visibility = Visibility.Collapsed;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new();
            loginWindow.Show();
            Close();
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
           
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}