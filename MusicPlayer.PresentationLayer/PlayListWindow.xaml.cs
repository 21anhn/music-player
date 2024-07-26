using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.IdentityModel.Tokens;
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
        private MusicPlaylistService _msService = new();
        public User CurrentUser { get; set; }



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
            MusicsListView.ItemsSource = _userService.GetAllMusicsByUsername(CurrentUser.Username);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CreateSongButton_Click(object sender, RoutedEventArgs e)
        {
            var playlistName = PlayListTextBox.Text;
            var selectedMusics = MusicsListView.SelectedItems as List<Music>;

            if (string.IsNullOrEmpty(playlistName))
            {
                MessageBox.Show("Please enter a playlist name!");
                return;
            }

            var playlist = _playlistService.GetAllPlaylists().FirstOrDefault(p => p.PlaylistName == playlistName);

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

            if (selectedMusics != null)
            {
                foreach (var selectedMusic in selectedMusics)
                {
                    if (!playlist.PlaylistMusics.Any(pm => pm.MusicId == selectedMusic.MusicId))
                    {
                        var playlistMusic = new PlaylistMusic
                        {
                            MusicId = selectedMusic.MusicId,
                            PlaylistId = playlist.PlaylistId,
                        };
                        _msService.Add(playlistMusic);
                    }
                }
            }

            _playlistService.AddPlaylist(playlist);
            Close();
        }

        private void PlayListTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => PlayListTextBox.Focus();

        private void PlayListTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PlayListTextBox.Text) && PlayListTextBox.Text.Length > 0)
                PlayListTextBlock.Visibility = Visibility.Collapsed;
            else
                PlayListTextBlock.Visibility = Visibility.Visible;
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            LoadSongs();
        }
    }
}
