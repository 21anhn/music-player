using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicPlayer.DAL.Models;

namespace MusicPlayer.PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public User CurrentUser { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void SongsButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị danh sách bài hát và ẩn các phần khác
            SectionTitle.Text = "Songs";
            SongsListView.Visibility = Visibility.Visible;

            var music = new List<Music>()
            {
                new Music() {MusicName = "Song 1"},
                new Music() {MusicName = "Song 2"}
            };

            SongsListView.ItemsSource = music;  
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(CurrentUser != null)
            {
                WelcomeNameTextBlock.Text = CurrentUser.FullName;
            }
           
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new();
            loginWindow.Show();
            Close();
        }
    }
}