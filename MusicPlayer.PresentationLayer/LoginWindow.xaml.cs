using Microsoft.IdentityModel.Tokens;
using MusicPlayer.BLL.Services;
using MusicPlayer.DAL.Models;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private UserService _service = new();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void UsernameTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => UserNameTextBox.Focus();

        private void UserNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UserNameTextBox.Text) && UserNameTextBox.Text.Length > 0)
                UsernameTextBlock.Visibility = Visibility.Collapsed;
            else
                UsernameTextBlock.Visibility = Visibility.Visible;

        }

        private void PasswordTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => PasswordBox.Focus();

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PasswordBox.Password) && PasswordBox.Password.Length > 0)
                PasswordTextBlock.Visibility = Visibility.Collapsed;
            else
                PasswordTextBlock.Visibility = Visibility.Visible;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserNameTextBox.Text.IsNullOrEmpty() || PasswordBox.Password.IsNullOrEmpty())
            {
                MessageBox.Show("Both email and password are required", "Required fields", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User? account = _service.Authenticate(UserNameTextBox.Text, PasswordBox.Password);

            if (account == null)
            {
                MessageBox.Show("Invalid email address or wrong password", "Wrong credentials", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MainWindow m = new();
            //m.CurrentAccount = account;
            m.Show();
            this.Close();
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow re = new();
            re.Show();
            Close();
        }
    }

}
