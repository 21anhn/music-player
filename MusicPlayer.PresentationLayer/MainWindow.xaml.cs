﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using MusicPlayer.BLL.Services;
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
        private PlaylistService _playlistService = new();
        private MusicPlaylistService _msService = new();
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        private DispatcherTimer _timer;
        private int _currentMusicIndex = -1; // Chỉ số bài hát hiện tại
        private List<Music> _playlist = new List<Music>(); // Danh sách bài hát
        public User CurrentUser { get; set; }



        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
            VolumeSlider.ValueChanged += VolumeSlider_ValueChanged;
            _mediaPlayer.Volume = VolumeSlider.Value;
        }

        //search control
        private void SearchTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => SearchTextBox.Focus();

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchTextBox.Text) && SearchTextBox.Text.Length > 0)
                SearchTextBlock.Visibility = Visibility.Collapsed;
            else
                SearchTextBlock.Visibility = Visibility.Visible;
        }
        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Optional: Prevent the "ding" sound when Enter is pressed
                SearchMusic();
            }
        }

        private void SearchMusic()
        {
            var searchText = SearchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                MusicsListView.ItemsSource = null;
                MusicsListView.ItemsSource = _service.SearchMusicsBySongNameOrArtistName(CurrentUser.Username, searchText);
                SongsView(Visibility.Visible);
                PlaylistView(Visibility.Collapsed);
                HomeView(Visibility.Collapsed);
            }
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
            HomeView(Visibility.Collapsed);
            LoadData();
        }

        private void UploadSongButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Music Files (*.mp3;*.wav)|*.mp3;*.wav",
                Multiselect = true // Cho phép chọn nhiều tệp
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var sourceFiles = openFileDialog.FileNames; // Danh sách các tệp được chọn
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

                foreach (var sourcePath in sourceFiles)
                {
                    var fileName = Path.GetFileName(sourcePath);
                    var tagFile = TagLib.File.Create(sourcePath);
                    var musicName = tagFile.Tag.Title ?? "Unknown Title";
                    var artistNames = tagFile.Tag.Performers;
                    var artistName = artistNames.Length > 0 ? string.Join(", ", artistNames) : "Unknown Artist";

                    var destinationPath = Path.Combine(userFolder, fileName);

                    try
                    {
                        if (File.Exists(destinationPath))
                        {
                            // Tệp đã tồn tại, có thể bỏ qua hoặc thông báo cho người dùng
                            MessageBox.Show($"Music '{fileName}' already exists.");
                            continue;
                        }

                        File.Copy(sourcePath, destinationPath);

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
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error while copying '{fileName}': {ex.Message}");
                        // Xử lý lỗi nếu cần
                    }
                }

                LoadData();
            }
        }

        private void DeleteSongButton_Click(object sender, RoutedEventArgs e)
        {
            //Xóa 1 hoặc nhiều nhạc trong playlist
            var selectedMusics = MusicsListView.SelectedItems.OfType<Music>().ToList();
            if (selectedMusics.IsNullOrEmpty())
            {
                MessageBox.Show("Please selecte a/an music(s) to delete!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Confirm delete!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            foreach (var music in selectedMusics)
            {
                try
                {
                    // Kiểm tra xem bài nhạc có nằm trong playlist nào của user không
                    var isInPlaylist = _msService.GetAll()
                        .Any(pm => pm.MusicId == music.MusicId && _playlistService.GetAllPlaylists().Any(pl => pl.PlaylistId == pm.PlaylistId && pl.UserId == CurrentUser.UserId));

                    if (File.Exists(music.Link))
                    {
                        File.Delete(music.Link);
                    }
                    _service.DeleteMusic(music);
                }
                catch (Exception)
                {
                    MessageBox.Show(music.MusicName + " is in another playlist!", "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error); //Không cho xóa khi music đang nằm trong 1 playlist
                }
            }

            //Xóa xong thì load lại list view
            LoadData();
        }

        private void HomeView(Visibility visibility)
        {
            if (visibility.Equals(Visibility.Visible))
            {
                SectionTitle.Text = "Home";
            }
            HomeImgWrapPanel.Visibility = visibility;
        }

        private void SongsView(Visibility visibility)
        {
            if (visibility.Equals(Visibility.Visible))
            {
                SectionTitle.Text = "Songs";
            }
            MusicsListView.Visibility = visibility;
            SongsVisibileStackPanel.Visibility = visibility;
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
            HomeView(Visibility.Visible);
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
            var musics = _userService.GetAllMusicsByUsername(CurrentUser.Username);
            _playlist = musics.ToList();
            MusicsListView.ItemsSource = null;
            MusicsListView.ItemsSource = _playlist;
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
            HomeView(Visibility.Collapsed);
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

                    //Play music
                    PlayMusic(music);
                }
                else if (_mediaPlayer.CanPause)
                {
                    _mediaPlayer.Play();
                }
                PlayButton.Visibility = Visibility.Collapsed;
                PauseButton.Visibility = Visibility.Visible;
            }

        }

        // Event handlers for music controls
        private void PlayMusic(Music music)
        {
            if (File.Exists(music.Link))
            {
                _mediaPlayer.Open(new Uri(music.Link));
                _mediaPlayer.MediaOpened += (s, e) =>
                {
                    if (_mediaPlayer.NaturalDuration.HasTimeSpan)
                    {
                        var duration = _mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                        PositionSlider.Maximum = duration;
                        UpdateTotalTime();
                    }
                };

                _mediaPlayer.MediaEnded += (s, e) =>
                {
                    if (EnabledReplayButton.Visibility == Visibility)
                    {
                        // Nếu đang bật chế độ phát lại, phát lại bài hát hiện tại
                        PlayMusic(music);
                    }
                    else
                    {
                        // Nếu không, phát bài hát tiếp theo
                        PlayNextSong();
                    }
                };

                _mediaPlayer.Play();

                // Start the timer to update the slider value
                StartPlaybackTimer();
                _currentMusicIndex = _playlist.IndexOf(music); // Cập nhật chỉ số bài hát hiện tại
            }
            else
            {
                MessageBox.Show("Music file not found!");
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (PauseButton.Visibility == Visibility.Visible)
            {
                _mediaPlayer.Pause();

                PauseButton.Visibility = Visibility.Collapsed;
                PlayButton.Visibility = Visibility.Visible;
            }
        }
        private void PlayNextSong()
        {
            if (_playlist.Count == 0) return; // Không có bài hát để phát

            // Tìm bài hát tiếp theo
            _currentMusicIndex = (_currentMusicIndex + 1) % _playlist.Count;
            var nextMusic = _playlist[_currentMusicIndex];
            ChangeNameAndTitleTextBox(nextMusic);
            PlayMusic(nextMusic);
        }

        private void PlayPreviousSong()
        {
            if (_playlist.Count == 0) return; // Không có bài hát để phát

            // Tìm bài hát trước đó
            _currentMusicIndex = (_currentMusicIndex - 1 + _playlist.Count) % _playlist.Count;
            var previousMusic = _playlist[_currentMusicIndex];
            ChangeNameAndTitleTextBox(previousMusic);
            PlayMusic(previousMusic);
        }

        private void PlayRandomSong()
        {
            if (_playlist.Count == 0) return; // Không có bài hát để phát

            // Tạo một chỉ số ngẫu nhiên
            Random random = new Random();
            _currentMusicIndex = random.Next(_playlist.Count);
            var randomMusic = _playlist[_currentMusicIndex];
            ChangeNameAndTitleTextBox(randomMusic);
            PlayMusic(randomMusic);
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            PlayPreviousSong();
            ChangeIconButtonInPlayMusic();
        }


        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            PlayNextSong();
            ChangeIconButtonInPlayMusic();
        }
        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            PlayRandomSong();
            ChangeIconButtonInPlayMusic();
        }

        private void ChangeNameAndTitleTextBox(Music music)
        {
            SongNameTextBlock.Text = music.MusicName;
            ArtistNameTextBlock.Text = music.ArtistName;
        }

        private void ChangeIconButtonInPlayMusic()
        {
            PauseButton.Visibility = Visibility.Visible;
            PlayButton.Visibility = Visibility.Collapsed;
        }

        private void ReplayButton_Click(object sender, RoutedEventArgs e)
        {
            ReplayButton.Visibility = Visibility.Collapsed;
            EnabledReplayButton.Visibility = Visibility.Visible;

        }
        private void EnabledReplayButton_Click(object sender, RoutedEventArgs e)
        {
            ReplayButton.Visibility = Visibility.Visible;
            EnabledReplayButton.Visibility = Visibility.Collapsed;
        }

        //timeline control
        private void StartPlaybackTimer()
        {
            if (_timer == null)
            {
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _timer.Tick += (s, e) =>
                {
                    if (_mediaPlayer.NaturalDuration.HasTimeSpan)
                    {
                        PositionSlider.Value = _mediaPlayer.Position.TotalSeconds;
                        CurrentTimeTextBlock.Text = _mediaPlayer.Position.ToString(@"mm\:ss");
                    }
                };
                _timer.Start();
            }
        }

        // Event handler for slider value change
        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan && !_mediaPlayer.IsBuffering)
            {
                _mediaPlayer.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            }
        }
        private void PositionSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            Point mousePosition = e.GetPosition(PositionSlider);
            double newValue = (mousePosition.X / PositionSlider.ActualWidth) * PositionSlider.Maximum;
            PositionSlider.Value = newValue;

            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                _mediaPlayer.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            }
        }
        private void UpdateTotalTime()
        {
            if (_mediaPlayer.NaturalDuration.HasTimeSpan)
            {
                TotalTimeTextBlock.Text = _mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
        }

        //volume control
        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Volume = 0;
            VolumeSlider.Value = 0;
            VolumeButton.Visibility = Visibility.Collapsed;
            MuteButton.Visibility = Visibility.Visible;
        }

        private void MuteButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Volume = 0.5;
            VolumeSlider.Value = 0.5;
            MuteButton.Visibility = Visibility.Collapsed;
            VolumeButton.Visibility = Visibility.Visible;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mediaPlayer.Volume = VolumeSlider.Value;

            if (VolumeSlider.Value == 0)
            {
                VolumeButton.Visibility = Visibility.Collapsed;
                MuteButton.Visibility = Visibility.Visible;
            }
            else
            {
                VolumeButton.Visibility = Visibility.Visible;
                MuteButton.Visibility = Visibility.Collapsed;
            }

            _mediaPlayer.Volume = VolumeSlider.Value;
        }

        private void OpenPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylists = PlaylistsListView.SelectedItems.OfType<Playlist>().ToList();
            if (selectedPlaylists.IsNullOrEmpty())
            {
                MessageBox.Show("Please select a playlist to open!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ListWindow listWindow = new();
            listWindow.CurrentPlaylist = selectedPlaylists[0]; //Chỉ cho phép open 1 playlist
            Hide(); //Ẩn màn hình hiện tại đi
            _mediaPlayer.Stop();
            listWindow.ShowDialog();
            //Sau khi tắt thì load lại data của PlaylistsListView
            LoadPlaylists();
            Show(); //Show lại màn hình main
        }

        private void UpdatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylists = PlaylistsListView.SelectedItems.OfType<Playlist>().ToList();
            if (selectedPlaylists.IsNullOrEmpty())
            {
                MessageBox.Show("Please select a playlist to update!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PlayListWindow playListWindow = new();
            playListWindow.CurrentPlaylist = selectedPlaylists[0]; //Chỉ cho phép update 1 playlist
            playListWindow.CurrentUser = CurrentUser; //Lấy danh sách nhạc của user đó
            playListWindow.ShowDialog();
            LoadPlaylists(); //Reset lại playlist
        }

        private void DeletePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedPlaylists = PlaylistsListView.SelectedItems.OfType<Playlist>().ToList();
            if (selectedPlaylists.IsNullOrEmpty())
            {
                MessageBox.Show("Please select playlist(s) to delete!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Confirm delete!", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            foreach (var item in selectedPlaylists)
            {
                _playlistService.DeletePlaylist(item);
            }
            LoadPlaylists();
        }
    }
}