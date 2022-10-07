using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

using ACS.Contracts.Services;
using ACS.Contracts.Views;
using ACS.Models;
using ACS.Properties;
using Microsoft.Extensions.Options;

namespace ACS.Views
{
    public partial class SettingsPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly AppConfig _appConfig;
        private readonly IThemeSelectorService _themeSelectorService;
        private readonly ISystemService _systemService;
        private readonly IApplicationInfoService _applicationInfoService;
        private bool _isInitialized;
        private AppTheme _theme;
        private string _versionDescription;

        public AppTheme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

        public string VersionDescription
        {
            get { return $"{Properties.Resources.AppDisplayName} - {_applicationInfoService.GetVersion()}";  }
            set { Set(ref _versionDescription, value); }
        }

        public SettingsPage(IOptions<AppConfig> appConfig, IThemeSelectorService themeSelectorService, ISystemService systemService, IApplicationInfoService applicationInfoService)
        {
            _appConfig = appConfig.Value;
            _themeSelectorService = themeSelectorService;
            _systemService = systemService;
            _applicationInfoService = applicationInfoService;
            CultureResources.ChangeCulture(CultureInfo.CurrentCulture);
            InitializeComponent();
            LanguageComboBox.SelectionChanged += new SelectionChangedEventHandler(LanguageComboBox_SelectionChanged);
            LanguageComboBox.SelectedItem = CultureInfo.DefaultThreadCurrentCulture?? CultureInfo.CurrentCulture;

            DataContext = this;
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCulture = LanguageComboBox.SelectedItem as CultureInfo;

            //if not current language
            if (Properties.Resources.Culture != null && !Properties.Resources.Culture.Equals(selectedCulture))
            {
                Debug.WriteLine(string.Format("Change Current Culture to [{0}]", selectedCulture));

                CultureInfo.DefaultThreadCurrentCulture = selectedCulture;
                //change resources to new culture
                CultureResources.ChangeCulture(selectedCulture);
                OnPropertyChanged(nameof(VersionDescription));
            }
        }

        public void OnNavigatedTo(object parameter)
        {
            Theme = _themeSelectorService.GetCurrentTheme();
            _isInitialized = true;
        }

        public void OnNavigatedFrom()
        {
        }

        private void OnLightChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _themeSelectorService.SetTheme(AppTheme.Light);
            }
        }

        private void OnDarkChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _themeSelectorService.SetTheme(AppTheme.Dark);
            }
        }

        private void OnDefaultChecked(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
            {
                _themeSelectorService.SetTheme(AppTheme.Default);
            }
        }

        private void OnPrivacyStatementClick(object sender, RoutedEventArgs e)
            => _systemService.OpenInWebBrowser(_appConfig.PrivacyStatement);

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
    }
}
