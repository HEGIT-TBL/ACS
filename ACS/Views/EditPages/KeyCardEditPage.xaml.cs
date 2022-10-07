using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using System;
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
    public partial class KeyCardEditPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly IGenericRepositoryAsync<KeyCard> _kcRepository;
        private readonly IGenericRepositoryAsync<AccessPoint> _apRepository;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public KeyCard Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private KeyCard _item;

        public KeyCard ItemOrigin
        {
            get => _itemOrigin;
            set
            {
                _itemOrigin = value;
                OnPropertyChanged(nameof(ItemOrigin));
            }
        }
        private KeyCard _itemOrigin;

        public KeyCardEditPage(IGenericRepositoryAsync<KeyCard> kcRepository, IGenericRepositoryAsync<AccessPoint> apRepository, INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = this;
            Item = new KeyCard()
            {
                AvailableAccessPoints = new List<AccessPoint>(),
                ExpirationDate = DateTime.UtcNow.AddDays(1),
            };
            ItemOrigin = null;
            _kcRepository = kcRepository;
            _apRepository = apRepository;
            _navigationService = navigationService;
            _cts = new CancellationTokenSource().Token;
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                Item = parameter as KeyCard;
                ItemOrigin = parameter as KeyCard;
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
            Item.ExpirationDate = Item.ExpirationDate.ToUniversalTime();
            if (Item.ExpirationDate < System.DateTime.UtcNow)
            {
                MessageBox.Show("Current expiration date is not valid!", "Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var itemChanges = Item;

            //track current entity
            if (ItemOrigin != null)
            {
                Item = await _kcRepository.GetOneAsync(Item.Id, _cts);
                Item.Key = itemChanges.Key;
                Item.ExpirationDate = itemChanges.ExpirationDate;
            }
            else
            {
                 _kcRepository.Attach(Item);
            }
            await _kcRepository.SaveChangesAsync(_cts);
            _navigationService.GoBack();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
        }

        private void SwitchFieldsIsEnabled()
        {
            KeyTextBox.IsReadOnly = !KeyTextBox.IsReadOnly;
            ExpirationDateDatePicker.IsEnabled = !ExpirationDateDatePicker.IsEnabled;
            SaveChangesButton.IsEnabled = !SaveChangesButton.IsEnabled;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You sure you want to delete this?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _kcRepository.DeleteAsync(ItemOrigin.Id, _cts);
                await _kcRepository.SaveChangesAsync(_cts);
                _navigationService.GoBack();
            }
        }
    }
}
