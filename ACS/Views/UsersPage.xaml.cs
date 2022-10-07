using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Views.EditPages;

namespace ACS.Views
{
    public partial class UsersPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly IGenericRepositoryAsync<User> _userRepository;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _ct;

        public ObservableCollection<User> Source { get; } = new ObservableCollection<User>();

        public UsersPage(IGenericRepositoryAsync<User> userRepository, INavigationService navigationService)
        {
            _userRepository = userRepository;
            _ct = new CancellationTokenSource().Token;
            _navigationService = navigationService;
            InitializeComponent();
            DataContext = this;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();

            // Replace this with your actual data
            var data = await _userRepository.GetAllAsync(_ct);

            foreach (var item in data)
            {
                Source.Add(item);
            }
        }

        public void OnNavigatedFrom()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _navigationService.NavigateTo(typeof(UserEditPage), ((User)null, isNavigatedFromCars: false));
        }

        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo(typeof(UserEditPage), ((User)(sender as DataGridRow).Item, isNavigatedFromCars: false));
        }
    }
}
