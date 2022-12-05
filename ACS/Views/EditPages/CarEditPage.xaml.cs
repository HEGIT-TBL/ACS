using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Models;
using ACS.Services;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ACS.Views.EditPages
{
    public partial class CarEditPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<User> _userAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public User ItemParent
        {
            get => _itemParent;
            set
            {
                _itemParent = value;
                OnPropertyChanged(nameof(ItemParent));
            }
        }
        private User _itemParent;

        public Car Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private Car _item;

        public Car ItemOrigin
        {
            get => _itemOrigin;
            set
            {
                _itemOrigin = value;
                OnPropertyChanged(nameof(ItemOrigin));
            }
        }
        private Car _itemOrigin;

        public CarEditPage(GenericAPIPoster<User> userAPIPoster, INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = this;
            Item = new Car();
            ItemOrigin = null;

            _userAPIPoster = userAPIPoster;
            _navigationService = navigationService;
            _cts = new CancellationTokenSource().Token;
        }

        public async void OnNavigatedTo(object parameter)
        {
            var tuple = ((User, Car))parameter;
            ItemParent = await _userAPIPoster.GetOneAsync(tuple.Item1.Id, _cts) ?? tuple.Item1;
            if (tuple.Item2 != null)
            {
                Item = tuple.Item2;
                ItemOrigin = tuple.Item2;
            }
            else
            {
                EditButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                SwitchFieldsIsEnabled();
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.GoBack();
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();

            if (!ItemParent.OwnedCars.Any(car => Item.Id == car.Id))
            {
                ItemParent.OwnedCars.Add(Item);
            }
            else
            {
                var editedCar = ItemParent.OwnedCars.Find(car => car.Id == Item.Id);
                editedCar.CarNumberPlate = Item.CarNumberPlate;
                editedCar.Color = Item.Color;
                editedCar.CarModel = Item.CarModel;
            }

            _userAPIPoster.Update(ItemParent);
            await _userAPIPoster.SaveChangesAsync(_cts);
            _navigationService.NavigateDoubleRemoveBackEntryTo(typeof(UserEditPage), (ItemParent, isNavigatedFromCars: true));
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
        }

        private void SwitchFieldsIsEnabled()
        {
            NumberPlateTextBox.IsReadOnly = !NumberPlateTextBox.IsReadOnly;
            ColorTextBox.IsReadOnly = !ColorTextBox.IsReadOnly;
            ModelTextBox.IsReadOnly = !ModelTextBox.IsReadOnly;
            SaveChangesButton.IsEnabled = !SaveChangesButton.IsEnabled;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You sure you want to delete this?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                ItemParent.OwnedCars.RemoveAll(item => item.Id == Item.Id);
                _userAPIPoster.Update(ItemParent);
                await _userAPIPoster.SaveChangesAsync(_cts);
                _navigationService.NavigateDoubleRemoveBackEntryTo(typeof(UserEditPage), (ItemParent, isNavigatedFromCars: true));
            }
        }
    }
}
