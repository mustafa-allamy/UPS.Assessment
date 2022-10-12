using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Interfaces.Services;

namespace UPS.Assessment.Windows
{
    /// <summary>
    /// Interaction logic for AddUserWindows.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private readonly IUserService _userService;

        public AddUserWindow(IUserService userService)
        {
            _userService = userService;
            InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem genderSelectedItem = (ComboBoxItem)GenderComboBox.SelectedItem;
            ComboBoxItem statusSelectedItem = (ComboBoxItem)StatusComboBox.SelectedItem;

            if (!ValidateInput(genderSelectedItem, statusSelectedItem))
                return;

            var createUserForm = new CreateUserForm()
            {
                name = NameTextBox.Text,
                email = EmailTextBox.Text,
                gender = genderSelectedItem.Content.ToString()!,
                status = statusSelectedItem.Content?.ToString()!
            };

            var serviceResponse = await _userService.AddUser(createUserForm);
            if (serviceResponse.Failed)
                MessageBox.Show($"Failed to add user reason:{string.Join(',', serviceResponse.Errors)}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            else
            {
                MessageBox.Show($"User added successfully", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
                this.Close();

            }


        }

        private bool ValidateInput(ComboBoxItem genderSelectedItem, ComboBoxItem statusSelectedItem)
        {
            List<string> Errors = new List<string>();
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || NameTextBox.Text.Length < 2)
                Errors.Add("Please insert a valid name");

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text) || !MailAddress.TryCreate(EmailTextBox.Text, out var email))
                Errors.Add("Please insert a valid Email");

            if (genderSelectedItem?.Content is null)
                Errors.Add("Please select gender");

            if (statusSelectedItem?.Content is null)
                Errors.Add("Please select status");

            if (Errors.Any())
            {
                MessageBox.Show(
                    $"Please fix the following errors before saving{System.Environment.NewLine}{String.Join(System.Environment.NewLine, Errors.ToArray())}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                return false;
            }

            return true;

        }
    }
}
