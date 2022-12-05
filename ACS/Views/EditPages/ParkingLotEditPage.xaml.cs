using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Core.Models.Enums;
using ACS.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ACS.Views.EditPages
{
    public partial class ParkingLotEditPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<ParkingLot> _plAPIPoster;
        private readonly GenericAPIPoster<Car> _carAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public class CheckableCar
        {
            public bool IsChecked { get; set; }
            public Car Car { get; set; }
            public CheckableCar(Car car, bool isChecked)
            {
                Car = car;
                IsChecked = isChecked;
            }
        }

        public ObservableCollection<CheckableCar> AvailableCars
        {
            get => _availableCars;
            set
            {
                _availableCars = value;
                OnPropertyChanged(nameof(AvailableCars));
            }
        }
        private ObservableCollection<CheckableCar> _availableCars;

        public ParkingLot Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private ParkingLot _item;

        public ParkingLot ItemOrigin
        {
            get => _itemOrigin;
            set
            {
                _itemOrigin = value;
                OnPropertyChanged(nameof(ItemOrigin));
            }
        }
        private ParkingLot _itemOrigin;

        public ParkingLotEditPage(GenericAPIPoster<ParkingLot> plAPIPoster, GenericAPIPoster<Car> carAPIPoster, INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = this;
            Item = new ParkingLot()
            {
                PlacedCar = new Car()
            };
            ItemOrigin = null;
            AvailableCars = new ObservableCollection<CheckableCar>();
            _plAPIPoster = plAPIPoster;
            _carAPIPoster = carAPIPoster;
            _navigationService = navigationService;
            _cts = new CancellationTokenSource().Token;
        }

        public async void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                Item = parameter as ParkingLot;
                ItemOrigin = parameter as ParkingLot;
            }
            else
            {
                EditButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                SwitchFieldsIsEnabled();
            }
            var pls = await _plAPIPoster.GetAllAsync(_cts);
            var cars = await _carAPIPoster.GetAllAsync(_cts);
            AvailableCars = new ObservableCollection<CheckableCar>(cars
                .Where(c => pls
                    .Select(pl => pl?.PlacedCar)
                    .Where(plc => plc != null)
                    .All(plc => plc.Id != c.Id))
                .Select(c => new CheckableCar(c, Item.PlacedCar != null && Item.PlacedCar.Id == c.Id)));
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
                Item = await _plAPIPoster.GetOneAsync(Item.Id, _cts);
            }

            if (itemChanges.State == ParkingLotState.Empty || itemChanges.State == ParkingLotState.Maintainance)
            {
                Item.PlacedCar = null;
            }
            else
            {
                var car = AvailableCars.FirstOrDefault(ap => ap.IsChecked)?.Car;
                if (car != null
                    && car?.Id != ItemOrigin?.PlacedCar?.Id)
                {
                    Item.PlacedCar = car;
                }
            }

            if (ItemOrigin != null)
            {
                //update other fields
                Item.LotNumber = itemChanges.LotNumber;
                Item.State = itemChanges.State;
                _plAPIPoster.Update(Item);
            }
            else
            {
                _plAPIPoster.Attach(Item);
            }
            await _plAPIPoster.SaveChangesAsync(_cts);
            _navigationService.GoBack();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
        }

        private void SwitchFieldsIsEnabled()
        {
            LotNumberTextBox.IsReadOnly = !LotNumberTextBox.IsReadOnly;
            StateCombobox.IsEnabled = !StateCombobox.IsEnabled;
            CarsListRadio.IsEnabled = !CarsListRadio.IsEnabled;
            SaveChangesButton.IsEnabled = !SaveChangesButton.IsEnabled;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You sure you want to delete this?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _plAPIPoster.DeleteAsync(ItemOrigin.Id, _cts);
                await _plAPIPoster.SaveChangesAsync(_cts);
                _navigationService.GoBack();
            }
        }
    }
}
