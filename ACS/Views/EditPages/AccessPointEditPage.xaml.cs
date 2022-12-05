using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Services;
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
    public partial class AccessPointEditPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<AccessPoint> _apAPIPoster;
        private readonly GenericAPIPoster<KeyCard> _kcAPIPoster;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;

        public class CheckableKeyCard
        {
            public bool IsChecked { get; set; }
            public KeyCard KeyCard { get; set; }
            public CheckableKeyCard(KeyCard keyCard, bool isChecked)
            {
                KeyCard = keyCard;
                IsChecked = isChecked;
            }
        }

        public ObservableCollection<CheckableKeyCard> CheckableKeyCards
        {
            get => _checkableKeyCards;
            set
            {
                _checkableKeyCards = value;
                OnPropertyChanged(nameof(CheckableKeyCards));
            }
        }
        private ObservableCollection<CheckableKeyCard> _checkableKeyCards;

        public AccessPoint Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private AccessPoint _item;

        public AccessPoint ItemOrigin
        {
            get => _itemOrigin;
            set
            {
                _itemOrigin = value;
                OnPropertyChanged(nameof(ItemOrigin));
            }
        }
        private AccessPoint _itemOrigin;

        public AccessPointEditPage(GenericAPIPoster<AccessPoint> apAPIPoster, GenericAPIPoster<KeyCard> kcAPIPoster, INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = this;
            Item = new AccessPoint()
            {
                AllowedKeyCards = new List<KeyCard>()
            };
            ItemOrigin = null;
            CheckableKeyCards = new ObservableCollection<CheckableKeyCard>();
            _apAPIPoster = apAPIPoster;
            _kcAPIPoster = kcAPIPoster;
            _navigationService = navigationService;
            _cts = new CancellationTokenSource().Token;
        }

        public async void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                Item = parameter as AccessPoint;
                ItemOrigin = parameter as AccessPoint;
            }
            else
            {
                EditButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                SwitchFieldsIsEnabled();
            }
            CheckableKeyCards = new ObservableCollection<CheckableKeyCard>((await _kcAPIPoster.GetAllAsync(_cts))
                .Select(kc => new CheckableKeyCard(kc, Item.AllowedKeyCards.Any(kci => kci.Id == kc.Id))));
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
                Item = await _apAPIPoster.GetOneAsync(Item.Id, _cts);
            }

            //mb liqwify but DON"T TOUCH
            foreach (var kc in CheckableKeyCards.Where(ckc => !ckc.IsChecked
                     && Item.AllowedKeyCards.Any(kc => ckc.KeyCard.Id == kc.Id))
                 .Select(ckc => ckc.KeyCard))
            {
                Item.AllowedKeyCards.RemoveAll(apkc => apkc.Id == kc.Id);
            }

            foreach (var kc in CheckableKeyCards.Where(ckc => ckc.IsChecked
                    && !Item.AllowedKeyCards.Any(kc => ckc.KeyCard.Id == kc.Id))
                .Select(ckc => ckc.KeyCard))
            {
                //skip already existing relationships
                if (ItemOrigin != null
                    && ItemOrigin.AllowedKeyCards.Any(iokc => iokc.Id == kc.Id))
                {
                    continue;
                }

                kc.AvailableAccessPoints = kc.AvailableAccessPoints.Where(ap => ap.Id == Item.Id).ToList();
                Item.AllowedKeyCards.Add(kc);
            }

            if (ItemOrigin != null)
            {
                //update other fields
                Item.Location = itemChanges.Location;
                Item.AccessPointType = itemChanges.AccessPointType;
                Item.ControllerIP = itemChanges.ControllerIP;
            }
            else
            {
                _apAPIPoster.Attach(Item);
            }
            await _apAPIPoster.SaveChangesAsync(_cts);
            _navigationService.GoBack();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
        }

        private void SwitchFieldsIsEnabled()
        {
            ControllerIPTextBox.IsReadOnly = !ControllerIPTextBox.IsReadOnly;
            LocationTextBox.IsReadOnly = !LocationTextBox.IsReadOnly;
            APTypeCombobox.IsEnabled = !APTypeCombobox.IsEnabled;
            KeyCardListToggle.IsEnabled = !KeyCardListToggle.IsEnabled;
            SaveChangesButton.IsEnabled = !SaveChangesButton.IsEnabled;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You sure you want to delete this?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _apAPIPoster.DeleteAsync(ItemOrigin.Id, _cts);
                await _apAPIPoster.SaveChangesAsync(_cts);
                _navigationService.GoBack();
            }
        }
    }
}
