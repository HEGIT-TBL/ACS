using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Helpers;
using ACS.Services;
using ACS.Views.EditPages;

namespace ACS.Views
{
    public partial class AccessPointsPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<AccessPoint> _accessPointAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _ct;
        public ObservableCollection<AccessPoint> Source { get; } = new ObservableCollection<AccessPoint>();
        
        public AccessPointsPage(GenericAPIPoster<AccessPoint> accessPointAPIPoster,INavigationService navigationService)
        {
            _accessPointAPIPoster = accessPointAPIPoster;
            _navigationService = navigationService;
            _ct = new CancellationTokenSource().Token;
            InitializeComponent();
            DataContext = this;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();

            var data = await _accessPointAPIPoster.GetAllAsync(_ct);

            foreach (var item in data)
            {
                item.AccessPointType.GetDescription();
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
            _navigationService.NavigateTo(typeof(AccessPointEditPage));
        }

        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _navigationService.NavigateDoubleRemoveBackEntryTo(typeof(AccessPointEditPage), (sender as DataGridRow).Item);
        }
    }
}
