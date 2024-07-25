using System;
using System.Windows;
using System.Windows.Controls;
using MusicPlayer.DAL;
using MusicPlayer.DAL.Models;
using MusicPlayer.DAL.Repositories;

namespace MusicPlayer.PresentationLayer
{
    public partial class RegisterWindow : Window
    {
        private readonly UserRepository _userRepository;

        public RegisterWindow()
        {
            InitializeComponent();
            _userRepository = new UserRepository(new MusicPlayerContext());
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
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

            var user = new User
            {
                FullName = fullName,
                Username = username,
                Password = password
            };

            try
            {
                bool success = await _userRepository.RegisterUserAsync(user);

                if (success)
                {
                    MessageBox.Show("Registration successful!");
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
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
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
