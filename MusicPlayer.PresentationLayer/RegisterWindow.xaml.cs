using System;
using System.Windows;
using System.Windows.Controls;
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
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var fullName = FullNameTextBox.Text;
            var username = EmailTextBox.Text;
            var password = PasswordTextBox.Password;
            var confirmPassword = ConfirmPasswordTextBox.Password;

         
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
            this.Close();

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.ShowDialog();
            
        }
    }
}
