using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ACS.Views.EditPages
{
    public partial class UserEditPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly IGenericRepositoryAsync<User> _userRepository;
        private readonly IGenericRepositoryAsync<KeyCard> _kcRepository;
        private readonly INavigationService _navigationService;
        private readonly CancellationToken _cts;
        private readonly List<BitmapSource> _identifierPhotosToAdd;

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

        public User Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged(nameof(Item));
            }
        }
        private User _item;

        public BitmapSource PFP
        {
            get => _pfp;
            set
            {
                _pfp = value;
                HandlePFPChange();
                OnPropertyChanged(nameof(PFP));
            }
        }
        private BitmapSource _pfp;

        public User ItemOrigin
        {
            get => _itemOrigin;
            set
            {
                _itemOrigin = value;
                OnPropertyChanged(nameof(ItemOrigin));
            }
        }
        private User _itemOrigin;

        public UserEditPage(
            IGenericRepositoryAsync<User> userRepository,
            IGenericRepositoryAsync<KeyCard> kcRepository,
            INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = this;

            Item = new User()
            {
                KeyCards = new List<KeyCard>(),
                OwnedCars = new List<Car>(),
                Identifiers = new List<Identifier>(),
            };
            ItemOrigin = null;
            CheckableKeyCards = new ObservableCollection<CheckableKeyCard>();
            _identifierPhotosToAdd = new List<BitmapSource>();
            _userRepository = userRepository;
            _kcRepository = kcRepository;
            _navigationService = navigationService;
            _cts = new CancellationTokenSource().Token;
        }

        public async void OnNavigatedTo(object parameter)
        {
            var tuple = ((User, bool))parameter;
            if (tuple.Item1 != null)
            {
                Item = tuple.Item1;
                ItemOrigin = tuple.Item1;
            }
            if (tuple.Item1 == null || tuple.Item2)
            {
                EditButton.Visibility = Visibility.Hidden;
                DeleteButton.Visibility = Visibility.Hidden;
                SwitchFieldsIsEnabled();
            }

            if (ItemOrigin?.ProfilePicture == null)
            {
                PFP = Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.DefaultPFP.GetHbitmap(),
                    IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            else
            {
                using var ms = new MemoryStream(ItemOrigin.ProfilePicture);
                var bmp = new Bitmap(ms);
                PFP = Imaging.CreateBitmapSourceFromHBitmap(bmp.GetHbitmap(), IntPtr.Zero,
                    Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                ms.Close();
            }
            CheckableKeyCards = new ObservableCollection<CheckableKeyCard>((await _kcRepository.GetAllAsync(_cts))
                .Where(kc => kc.Owner == null || kc.Owner.Id == Item.Id)
                .Select(kc => new CheckableKeyCard(kc, Item.KeyCards.Any(kci => kci.Id == kc.Id))));
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
                Item = await _userRepository.GetOneAsync(Item.Id, _cts);
            }

            HandleKeyCardChange();
            //HandlePFPChange();
            HandleIdentifierAddition();

            if (ItemOrigin != null)
            {
                //update other fields
                Item.Surname = itemChanges.Surname;
                Item.Name = itemChanges.Name;
                Item.Patronymic = itemChanges.Patronymic;

                _userRepository.Update(Item);
            }
            else
            {
                _userRepository.Attach(Item);
            }
            await _userRepository.SaveChangesAsync(_cts);
            _navigationService.GoBack();
        }

        private void HandleIdentifierAddition()
        {
            foreach (var photo in _identifierPhotosToAdd)
            {
                var encoder = new JpegBitmapEncoder()
                {
                    QualityLevel = 100,
                };
                using var ms = new MemoryStream();
                encoder.Frames.Add(BitmapFrame.Create(photo));
                encoder.Save(ms);
                Item.Identifiers.Add(new Identifier()
                {
                    Photo = ms.ToArray(),
                    FacePoints = new float[256]     //TODO: add recognition service call here
                });
                ms.Close();
            }
        }

        private void HandlePFPChange()
        {
            var encoder = new JpegBitmapEncoder()
            {
                QualityLevel = 100,
            };
            using var ms = new MemoryStream();
            encoder.Frames.Add(BitmapFrame.Create(PFP));
            encoder.Save(ms);
            Item.ProfilePicture = ms.ToArray();
            ms.Close();
        }

        private void HandleKeyCardChange()
        {
            //mb liqwify but DON"T TOUCH
            foreach (var kc in CheckableKeyCards.Where(ckc => !ckc.IsChecked
                     && Item.KeyCards.Any(kc => ckc.KeyCard.Id == kc.Id))
                 .Select(ckc => ckc.KeyCard))
            {
                Item.KeyCards.RemoveAll(apkc => apkc.Id == kc.Id);
            }

            foreach (var kc in CheckableKeyCards.Where(ckc => ckc.IsChecked
                    && !Item.KeyCards.Any(kc => ckc.KeyCard.Id == kc.Id))
                .Select(ckc => ckc.KeyCard))
            {
                kc.AvailableAccessPoints = new List<AccessPoint>();
                Item.KeyCards.Add(kc);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchFieldsIsEnabled();
        }

        private void SwitchFieldsIsEnabled()
        {
            PFPButton.IsHitTestVisible = !PFPButton.IsHitTestVisible;
            SurnameTextBox.IsReadOnly = !SurnameTextBox.IsReadOnly;
            NameTextBox.IsReadOnly = !NameTextBox.IsReadOnly;
            PatronymicTextBox.IsReadOnly = !PatronymicTextBox.IsReadOnly;
            if (ItemOrigin != null)
            {
                KeyCardListToggle.IsEnabled = !KeyCardListToggle.IsEnabled;
                CarsDataGridExpander.IsEnabled = !CarsDataGridExpander.IsEnabled;
            }
            AddPhotosButton.IsEnabled = !AddPhotosButton.IsEnabled;
            SaveChangesButton.IsEnabled = !SaveChangesButton.IsEnabled;
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You sure you want to delete this?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                await _userRepository.DeleteAsync(ItemOrigin.Id, _cts);
                await _userRepository.SaveChangesAsync(_cts);
                _navigationService.GoBack();
            }
        }

        private void PFPButton_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog()
            {
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png",
            };
            if (op.ShowDialog() == true)
            {
                PFP = new BitmapImage(new Uri(op.FileName));
            }
            var scope = FocusManager.GetFocusScope(PFPButton);
            FocusManager.SetFocusedElement(scope, null);
            Keyboard.ClearFocus();
        }

        private void AddPhotosButton_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog()
            {
                Multiselect = true,
                Title = "Select a picture",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|Portable Network Graphic (*.png)|*.png",
            };
            if (op.ShowDialog() == true)
            {
                foreach (var filename in op.FileNames)
                {
                    _identifierPhotosToAdd.Add(new BitmapImage(new Uri(filename)));
                }
            }
            var scope = FocusManager.GetFocusScope(PFPButton);
            FocusManager.SetFocusedElement(scope, null);
            Keyboard.ClearFocus();
        }

        //cars
        private void AddCarButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateTo(typeof(CarEditPage), (Item, (Car)null));
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            _navigationService.NavigateTo(typeof(CarEditPage), (Item, (Car)(sender as DataGridRow).Item));
        }
    }
}
