using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.IdentityModel.Tokens;
using MusicPlayer.BLL.Services;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.PresentationLayer
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : Window
    {
        private PlaylistService _playlistService = new();
        private MusicPlaylistService _msService = new();
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        private DispatcherTimer _timer;
        private int _currentMusicIndex = -1; // Chỉ số bài hát hiện tại
        private List<Music> _playlist = new List<Music>(); // Danh sách bài hát
        public Playlist CurrentPlaylist { get; set; }

        public ListWindow()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            _playlist = _playlistService.GetMusicsByPlaylistId(CurrentPlaylist.PlaylistId);
            SongsListView.ItemsSource = null;
            SongsListView.ItemsSource = _playlist;
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _mediaPlayer.Close();
            Close();
        }

        private void DeleteSelectedSongButton_Click(object sender, RoutedEventArgs e)
        {
            //Xóa 1 hoặc nhiều nhạc trong playlist
            var selectedMusics = SongsListView.SelectedItems.OfType<Music>().ToList();
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

            foreach (var pm in selectedMusics)
            {
                _msService.DeletePlaylistMusicByMusicId(pm.MusicId);
            }

            //Xóa xong thì load lại list view
            LoadData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PlaylistNameTextBlock.Text = CurrentPlaylist.PlaylistName;
            LoadData();
        }
    }
}
