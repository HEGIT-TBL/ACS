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
    public partial class CamerasPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<Camera> _camerasAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public ObservableCollection<Camera> Source { get; } = new ObservableCollection<Camera>();


        public CamerasPage(GenericAPIPoster<Camera> camerasAPIPoster, INavigationService navigationService)
        {
            _cts = new CancellationTokenSource().Token;
            _camerasAPIPoster = camerasAPIPoster;
            _navigationService = navigationService;
            InitializeComponent();
            DataContext = this;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();

            var data = await _camerasAPIPoster.GetAllAsync(_cts);

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
            _navigationService.NavigateTo(typeof(CameraEditPage));
        }

        private void Row_DoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo(typeof(CameraEditPage), (sender as DataGridRow).Item);
        }
    }
}
