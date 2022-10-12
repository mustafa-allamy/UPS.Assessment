using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UPS.Assessment.Common.Forms;
using UPS.Assessment.Infrastructure.Helpers;
using UPS.Assessment.Infrastructure.Interfaces.Services;
using UPS.Assessment.Models;
using UPS.Assessment.Windows;


namespace UPS.Assessment
{

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IUserService _userService;
        private readonly IAbstractFactory<AddUserWindow> _userWindow;
        public ObservableCollection<User> users;
        private int _pageNumber;
        private int _maxPage;
        public int PageNumber
        {
            get { return _pageNumber; }
            set { _pageNumber = value; NotifyPropertyChanged(nameof(PageNumber)); }
        }

        public MainWindow(IUserService userService, IAbstractFactory<AddUserWindow> userWindow)
        {
            _userService = userService;
            _userWindow = userWindow;
            InitializeComponent();
            _pageNumber = 1;
            DataContext = this;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var serviceResponse = await GetUsersServiceResponse(1);

            users = new ObservableCollection<User>(serviceResponse.Data);
            _maxPage = serviceResponse.ItemsCount;
            UsersDataGrid.ItemsSource = users;
        }

        private async void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as User;
            if (MessageBox.Show($"Delete User: {obj.Name} ?",
                    "Are you sure?", MessageBoxButton.YesNo,
                    MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                var serviceResponse = await _userService.DeleteUser(obj.Id);
                if (serviceResponse.Failed)
                    MessageBox.Show($"Could not delete user, reason:{string.Join(',', serviceResponse.Errors)}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);

                users.Remove(obj);
            }


        }
        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PageNumber = 1;
            var serviceResponse = await GetUsersServiceResponse(_pageNumber);
            users = new ObservableCollection<User>(serviceResponse.Data);
            UsersDataGrid.ItemsSource = users;
        }



        private async void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            NameTextBox.Text = null;
            EmailTextBox.Text = null;
            GenderComboBox.SelectedItem = null;
            StatusComboBox.SelectedItem = null;

            var serviceResponse = await GetUsersServiceResponse(1);
            users = new ObservableCollection<User>(serviceResponse.Data);
            UsersDataGrid.ItemsSource = users;
        }

        private void AddUser_OnClick(object sender, RoutedEventArgs e)
        {
            var addUserWindow = _userWindow.Create();
            addUserWindow.Show();

        }
        private async void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pageNumber <= 1)
                PageNumber = 1;
            else
            {
                PageNumber--;
            }
            var serviceResponse = await GetUsersServiceResponse(_pageNumber);
            users = new ObservableCollection<User>(serviceResponse.Data);
            UsersDataGrid.ItemsSource = users;
        }
        private async void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pageNumber >= _maxPage)
                PageNumber = _maxPage;
            else
            {
                PageNumber++;
            }
            var serviceResponse = await GetUsersServiceResponse(_pageNumber);
            users = new ObservableCollection<User>(serviceResponse.Data);
            _maxPage = serviceResponse.ItemsCount;
            UsersDataGrid.ItemsSource = users;

        }
        private async Task<ServiceResponse<List<User>>> GetUsersServiceResponse(int pageNumber)
        {
            ComboBoxItem genderSelectedItem = (ComboBoxItem)GenderComboBox.SelectedItem;
            ComboBoxItem statusSelectedItem = (ComboBoxItem)StatusComboBox.SelectedItem;

            var userSearchForm = new GetUsersForm()
            {
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                Gender = genderSelectedItem?.Content?.ToString(),
                Status = statusSelectedItem?.Content?.ToString(),
                PageNumber = pageNumber,
            };
            var serviceResponse = await _userService.GetUsers(userSearchForm);
            if (serviceResponse.Failed)
                MessageBox.Show("Could not load data", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            _maxPage = serviceResponse.ItemsCount;
            return serviceResponse;

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        internal void NotifyPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



    }
}
