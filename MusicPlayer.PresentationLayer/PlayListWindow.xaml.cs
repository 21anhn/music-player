using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicPlayer.BLL.Services;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.PresentationLayer
{
    /// <summary>
    /// Interaction logic for PlayListWindow.xaml
    /// </summary>
    public partial class PlayListWindow : Window
    {
        private UserService _userService = new();
        private PlaylistService _playlistService = new();
        private MusicService _musicService = new();
        private MusicPlaylistService _msService = new();
        public User CurrentUser { get; set; }
        public Playlist CurrentPlaylist { get; set; }


        public PlayListWindow()
        {
            InitializeComponent();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void LoadSongs()
        {
            MusicsListView.ItemsSource = null;
            var musicsOfUser = _userService.GetAllMusicsByUsername(CurrentUser.Username);
            //Khi tạo mới playlist
            if (CurrentPlaylist == null)
            {
                MusicsListView.ItemsSource = musicsOfUser;
            }
            else //Update playlist có sẵn
            {
                MusicsListView.ItemsSource = _musicService.GetMusicsNotInPlaylist(musicsOfUser, CurrentPlaylist.PlaylistId);
            }
            
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CreateSongButton_Click(object sender, RoutedEventArgs e)
        {
            var playlistName = PlayListNameTextBox.Text;
            var selectedMusics = MusicsListView.SelectedItems.OfType<Music>().ToList();

            
            if (string.IsNullOrEmpty(playlistName))
            {
                MessageBox.Show("Please enter a playlist name!");
                return;
            }

            Playlist playlist;

            if(CurrentPlaylist == null)
            {
                playlist = _playlistService.GetAllPlaylists().FirstOrDefault(p => p.PlaylistName == playlistName);

                if (playlist != null)
                {
                    MessageBox.Show("Playlist name already exist!");
                    return;
                }

                playlist = new()
                {
                    PlaylistName = playlistName,
                    CreatedDate = DateTime.Now,
                    Status = true,
                    UserId = CurrentUser?.UserId ?? 0 // Gán trực tiếp UserId
                };

                _playlistService.AddPlaylist(playlist);
            }
            else
            {
                playlist = CurrentPlaylist;
            }

            if (selectedMusics != null)
            {
                foreach (var selectedMusic in selectedMusics)
                {
                    if (!playlist.PlaylistMusics.Any(pm => pm.MusicId == selectedMusic.MusicId))
                    {
                        var playlistMusic = new PlaylistMusic
                        {
                            Id = 0,
                            MusicId = selectedMusic.MusicId,
                            PlaylistId = playlist.PlaylistId,
                        };
                        _msService.Add(playlistMusic);
                    }
                }
            }

            //Update lại playlist có sẵn
            if(CurrentPlaylist != null)
            {
                _playlistService.UpdatePlaylist(playlist);
            }
            Close();
        }

        private void PlayListTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => PlayListNameTextBox.Focus();

        private void PlayListTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PlayListNameTextBox.Text) && PlayListNameTextBox.Text.Length > 0)
                PlayListTextBlock.Visibility = Visibility.Collapsed;
            else
                PlayListTextBlock.Visibility = Visibility.Visible;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            if(CurrentPlaylist != null)
            {
                PlayListNameTextBox.Text = CurrentPlaylist.PlaylistName;
            }
            LoadSongs();
        }
    }
}
