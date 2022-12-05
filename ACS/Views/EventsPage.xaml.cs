using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;
using ACS.Contracts.Views;
using ACS.Core.Data;
using ACS.Core.Models;
using ACS.Core.Models.Events;
using ACS.Services;

namespace ACS.Views
{
    public partial class EventsPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly GenericAPIPoster<AccessEvent> _aeAPIPoster;
        private readonly GenericAPIPoster<AccessPoint> _apAPIPoster;
        private readonly GenericAPIPoster<User> _userAPIPoster;
        private readonly GenericAPIPoster<Camera> _cameraAPIPoster;
        private readonly GenericAPIPoster<ParkingLotStateChangedEvent> _pleAPIPoster;
        private readonly GenericAPIPoster<FaceRecognizedEvent> _freAPIPoster;
        private readonly GenericAPIPoster<ParkingLot> _plAPIPoster;
        private readonly CancellationToken _ct;

        public class EventInfo
        {
            public DateTime Time { get; set; }
            public string Content { get; set; }
            public EventInfo(DateTime time, string content)
            {
                Time = time;
                Content = content;
            }
        }

        public ObservableCollection<EventInfo> Source { get; } = new ObservableCollection<EventInfo>();

        public EventsPage(GenericAPIPoster<AccessEvent> accessEventAPIPoster,
                          GenericAPIPoster<ParkingLotStateChangedEvent> parkingLotStateChangedAPIPoster,
                          GenericAPIPoster<FaceRecognizedEvent> faceRecognizedEventAPIPoster)
        {
            _aeAPIPoster = accessEventAPIPoster;
            _pleAPIPoster = parkingLotStateChangedAPIPoster;
            _freAPIPoster = faceRecognizedEventAPIPoster;
            _ct = new CancellationTokenSource().Token;
            InitializeComponent();
            DataContext = this;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();
            if (!(await _aeAPIPoster.GetAllAsync(_ct)).Any())
            {
                _aeAPIPoster.Attach(new AccessEvent()
                {
                    AccessTime = DateTime.UtcNow.AddMinutes(5).AddSeconds(10),
                    User = (await _userAPIPoster.GetAllAsync(_ct)).First(),
                    AccessPoint = (await _apAPIPoster.GetAllAsync(_ct)).First(),
                    IsPermissionGranted = true,
                });
                _aeAPIPoster.Attach(new AccessEvent()
                {
                    AccessTime = DateTime.UtcNow.AddHours(12).AddSeconds(9),
                    User = (await _userAPIPoster.GetAllAsync(_ct)).First(),
                    AccessPoint = (await _apAPIPoster.GetAllAsync(_ct)).Skip(1).First(),
                    IsPermissionGranted = true,
                });
            }
            if (!(await _freAPIPoster.GetAllAsync(_ct)).Any())
            {
                _freAPIPoster.Attach(new FaceRecognizedEvent()
                {
                    CaptureTime = DateTime.UtcNow.AddMinutes(5),
                    Camera = (await _cameraAPIPoster.GetAllAsync(_ct)).First(),
                    Probability = 0.8,
                    RecognizedUser = (await _userAPIPoster.GetAllAsync(_ct)).First(),
                });
                _freAPIPoster.Attach(new FaceRecognizedEvent()
                {
                    CaptureTime = DateTime.UtcNow.AddHours(12),
                    Camera = (await _cameraAPIPoster.GetAllAsync(_ct)).Skip(1).First(),
                    Probability = 0.8,
                    RecognizedUser = (await _userAPIPoster.GetAllAsync(_ct)).First(),
                });
            }
            if (!(await _pleAPIPoster.GetAllAsync(_ct)).Any())
            {
                _pleAPIPoster.Attach(new ParkingLotStateChangedEvent()
                {
                    StateChangeTime = DateTime.UtcNow,
                    ChangedLot = (await _plAPIPoster.GetAllAsync(_ct)).First(),
                });
                _pleAPIPoster.Attach(new ParkingLotStateChangedEvent()
                {
                    StateChangeTime = DateTime.UtcNow.AddMinutes(5).AddHours(12).AddSeconds(9),
                    ChangedLot = (await _plAPIPoster.GetAllAsync(_ct)).First(),
                });
            }
            await _aeAPIPoster.SaveChangesAsync(_ct);
            var accessEvents = await _aeAPIPoster.GetAllAsync(_ct);
            var parkingLotEvents = await _pleAPIPoster.GetAllAsync(_ct);
            var faceRecognitionEvents = await _freAPIPoster.GetAllAsync(_ct);
            //put to resx mb
            var aes = accessEvents.Select(ae => new EventInfo
                (ae.AccessTime, $"{ae.User.Surname} {ae.User.Name} {ae.User.Patronymic} запросил проход через {ae.AccessPoint.Location}. Допущен? {ae.IsPermissionGranted}"));
            var plsces = parkingLotEvents.Select(plsces => new EventInfo(plsces.StateChangeTime, $"Состояние парковочного места №{plsces.ChangedLot.LotNumber} изменилось на {plsces.ChangedLot.State}"));
            var fres = faceRecognitionEvents.Select(fres => new EventInfo
                (fres.CaptureTime, $"Камера {fres.Camera.Location} распознала {fres.RecognizedUser.Surname} {fres.RecognizedUser.Name} {fres.RecognizedUser.Patronymic} с точностью {fres.Probability}"));

            var data = aes.Union(plsces).Union(fres).OrderBy(d => d.Time);

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
    }
}
