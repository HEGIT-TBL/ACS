using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Models;
using ACS.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ACS.Views
{
    public partial class MainPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly IGenericRepositoryAsync<AccessPoint> _apRepository;
        private readonly IGenericRepositoryAsync<KeyCard> _kcRepository;

        public MainPage(IGenericRepositoryAsync<AccessPoint> apRepository, IGenericRepositoryAsync<KeyCard> kcRepository)
        {
            InitializeComponent();
            DataContext = this;
            _apRepository = apRepository;
            _kcRepository = kcRepository;
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

        public async void OnNavigatedTo(object parameter)
        {
            //var ap = await _userRepository.GetOneAsync(new Guid("210c5f8d-b736-4156-ab07-7607cf0ca908"), CancellationToken.None);
            //var e = new KeyCard()
            //{
            //    Key = "1234",
            //    ExpirationDate = DateTime.UtcNow,
            //    AvailableAccessPoints = new List<AccessPoint>()
            //};
            ////e.AvailableAccessPoints.Add(ap);
            //await _camerasRepository.CreateAsync(e, CancellationToken.None);
            //ap.AllowedKeyCards.Add(e);
            //await _userRepository.SaveChangesAsync(CancellationToken.None);

        }

        public void OnNavigatedFrom()
        {
            
        }
    }
}
