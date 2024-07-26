using System;
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

namespace MusicPlayer.PresentationLayer
{
    /// <summary>
    /// Interaction logic for PlayListWindow.xaml
    /// </summary>
    public partial class PlayListWindow : Window
    {
        public PlayListWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateSongButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PlayListTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => PlayListTextBox.Focus();

        private void PlayListTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PlayListTextBox.Text) && PlayListTextBox.Text.Length > 0)
                PlayListTextBlock.Visibility = Visibility.Collapsed;
            else
                PlayListTextBlock.Visibility = Visibility.Visible;
        }
    }
}
