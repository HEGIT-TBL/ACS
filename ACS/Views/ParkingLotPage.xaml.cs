using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Services;
using ACS.Views.EditPages;

namespace ACS.Views
{
    public partial class ParkingLotPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<ParkingLot> _plAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public ObservableCollection<ParkingLot> Source { get; } = new ObservableCollection<ParkingLot>();

        public ParkingLotPage(INavigationService navigationService, GenericAPIPoster<ParkingLot> plAPIPoster)
        {
            InitializeComponent();
            DataContext = this;
            _cts = new CancellationTokenSource().Token;
            _navigationService = navigationService;
            _plAPIPoster = plAPIPoster;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();

            // Replace this with your actual data
            var data = await _plAPIPoster.GetAllAsync(_cts);

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
            _navigationService.NavigateTo(typeof(ParkingLotEditPage));
        }

        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo(typeof(ParkingLotEditPage), (sender as DataGridRow).Item);
        }
    }
}
