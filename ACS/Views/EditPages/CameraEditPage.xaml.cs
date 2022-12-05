using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ACS.Views.EditPages
{
    public partial class CameraEditPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<AccessPoint> _apAPIPoster;
        private readonly GenericAPIPoster<Camera> _cameracAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public class CheckableAccessPoint
        {
            public bool IsChecked { get; set; }
            public AccessPoint AccessPoint { get; set; }
            public CheckableAccessPoint(AccessPoint accessPoint, bool isChecked)
            {
                AccessPoint = accessPoint;
                IsChecked = isChecked;
            }
        }

        public ObservableCollection<CheckableAccessPoint> AvailableAccessPoints
        {
            get => _availableAccessPoints;
            set
            {
                _availableAccessPoints = value;
                OnPropertyChanged(nameof(AvailableAccessPoints));
            }
        }
        private ObservableCollection<CheckableAccessPoint> _availableAccessPoints;

        public Camera Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private Camera _item;

        public Camera ItemOrigin
        {
            get => _itemOrigin;
            set
            {
                _itemOrigin = value;
                OnPropertyChanged(nameof(ItemOrigin));
            }
        }
        private Camera _itemOrigin;

        public CameraEditPage(GenericAPIPoster<AccessPoint> apAPIPoster, GenericAPIPoster<Camera> cameraAPIPoster, INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = this;
            Item = new Camera();
            ItemOrigin = null;
            AvailableAccessPoints = new ObservableCollection<CheckableAccessPoint>();
            _apAPIPoster = apAPIPoster;
            _cameracAPIPoster = cameraAPIPoster;
            _navigationService = navigationService;
            _cts = new CancellationTokenSource().Token;
        }

        public async void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                Item = parameter as Camera;
                ItemOrigin = parameter as Camera;
            }
            else
            {
                EditButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                SwitchFieldsIsEnabled();
            }
            AvailableAccessPoints = new ObservableCollection<CheckableAccessPoint>((await _apAPIPoster.GetAllAsync(_cts))
                .Select(ap => new CheckableAccessPoint(ap, Item.AccessPoint != null && Item.AccessPoint.Id == ap.Id)));
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.GoBack();
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
            var itemChanges = Item;

            //track current entity
            if (ItemOrigin != null)
            {
                Item = await _cameracAPIPoster.GetOneAsync(Item.Id, _cts);
            }
            var ap = AvailableAccessPoints.FirstOrDefault(ap => ap.IsChecked)?.AccessPoint;
            if (ap != null
                && ap?.Id != ItemOrigin?.AccessPoint?.Id)
            {
                ap.AllowedKeyCards = new List<KeyCard>();
                Item.AccessPoint = ap;
            }

            if (ItemOrigin != null)
            {
                Item.Location = itemChanges.Location;
                Item.StreamLink = itemChanges.StreamLink;
            }
            else
            {
                _cameracAPIPoster.Attach(Item);
            }
            await _cameracAPIPoster.SaveChangesAsync(_cts);
            _navigationService.GoBack();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
        }

        private void SwitchFieldsIsEnabled()
        {
            LocationTextBox.IsReadOnly = !LocationTextBox.IsReadOnly;
            StreamLinkTextBox.IsReadOnly = !StreamLinkTextBox.IsReadOnly;
            AccessPointListToggle.IsEnabled = !AccessPointListToggle.IsEnabled;
            SaveChangesButton.IsEnabled = !SaveChangesButton.IsEnabled;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You sure you want to delete this?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _cameracAPIPoster.DeleteAsync(ItemOrigin.Id, _cts);
                await _cameracAPIPoster.SaveChangesAsync(_cts);
                _navigationService.GoBack();
            }
        }
    }
}
