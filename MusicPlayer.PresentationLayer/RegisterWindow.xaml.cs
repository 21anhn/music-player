using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicPlayer.BLL.Services;
using MusicPlayer.DAL;
using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;

namespace MusicPlayer.PresentationLayer
{
    public partial class RegisterWindow : Window
    {
        private UserService _service = new();

        public RegisterWindow()
        {
            InitializeComponent();
            WindowStyle = WindowStyle.None;
        }

        private void FullNameTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => FullNameTextBox.Focus();

        private void FullNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FullNameTextBox.Text) && FullNameTextBox.Text.Length > 0)
                FullNameTextBlock.Visibility = Visibility.Collapsed;
            else
                FullNameTextBlock.Visibility = Visibility.Visible;
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

        private void ConfirmPasswordTextBlock_MouseDown(object sender, MouseButtonEventArgs e) => ConfirmPasswordBox.Focus();

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ConfirmPasswordBox.Password) && ConfirmPasswordBox.Password.Length > 0)
                ConfirmPasswordTextBlock.Visibility = Visibility.Collapsed;
            else
                ConfirmPasswordTextBlock.Visibility = Visibility.Visible;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var fullName = FullNameTextBox.Text;
            var username = UserNameTextBox.Text;
            var password = PasswordBox.Password;
            var confirmPassword = ConfirmPasswordBox.Password;

         
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            try
            {
                bool success = _service.Register(fullName, username, password);

                if (success)
                {
                    MessageBox.Show("Registration successful!");
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.ShowDialog();
                    this.Close();
                }
                else
                {
                    UserNameTextBox.Text = "";
                    MessageBox.Show("Registration failed. The username might be taken.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();       
        }

        private void LoginHyperlink_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LoginWindow l = new();
            l.Show();
        }
    }
}
