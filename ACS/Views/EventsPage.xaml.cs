using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;

using ACS.Contracts.Views;
using ACS.Core.Contracts.Services;
using ACS.Core.Data;
using ACS.Core.Models;
using ACS.Core.Models.Events;

namespace ACS.Views
{
    public partial class EventsPage : Page, INotifyPropertyChanged, INavigationAware
    {
        private readonly IGenericRepositoryAsync<AccessEvent> _aeRepository;
        private readonly IGenericRepositoryAsync<ParkingLotStateChangedEvent> _pleRepository;
        private readonly IGenericRepositoryAsync<FaceRecognizedEvent> _freRepository;
        private readonly CancellationToken _ct;
        private readonly AccessControlDbContext _context;

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

        public EventsPage(IGenericRepositoryAsync<AccessEvent> accessEventRepository,
                          IGenericRepositoryAsync<ParkingLotStateChangedEvent> parkingLotStateChangedRepository,
                          IGenericRepositoryAsync<FaceRecognizedEvent> faceRecognizedEventRepository)
        {
            _aeRepository = accessEventRepository;
            _pleRepository = parkingLotStateChangedRepository;
            _freRepository = faceRecognizedEventRepository;
            _context = _aeRepository.Context;
            _ct = new CancellationTokenSource().Token;
            InitializeComponent();
            DataContext = this;
        }

        public async void OnNavigatedTo(object parameter)
        {
            Source.Clear();
            if (!_context.AccessEvents.Any())
            {
                _aeRepository.Attach(new AccessEvent()
                {
                    AccessTime = DateTime.UtcNow.AddMinutes(5).AddSeconds(10),
                    User = _context.Users.First(),
                    AccessPoint = _context.AccessPoints.First(),
                    IsPermissionGranted = true,
                });
                _aeRepository.Attach(new AccessEvent()
                {
                    AccessTime = DateTime.UtcNow.AddHours(12).AddSeconds(9),
                    User = _context.Users.First(),
                    AccessPoint = _context.AccessPoints.Skip(1).First(),
                    IsPermissionGranted = true,
                });
            }
            if (!_context.FaceRecognizedEvents.Any())
            {
                _freRepository.Attach(new FaceRecognizedEvent()
                {
                    CaptureTime = DateTime.UtcNow.AddMinutes(5),
                    Camera = _context.Cameras.First(),
                    Probability = 0.8,
                    RecognizedUser = _context.Users.First(),
                });
                _freRepository.Attach(new FaceRecognizedEvent()
                {
                    CaptureTime = DateTime.UtcNow.AddHours(12),
                    Camera = _context.Cameras.Skip(1).First(),
                    Probability = 0.8,
                    RecognizedUser = _context.Users.First(),
                });
            }
            if (!_context.ParkingLotStateChangedEvents.Any())
            {
                _pleRepository.Attach(new ParkingLotStateChangedEvent()
                {
                    StateChangeTime = DateTime.UtcNow,
                    ChangedLot = _context.ParkingLots.First(),
                });
                _pleRepository.Attach(new ParkingLotStateChangedEvent()
                {
                    StateChangeTime = DateTime.UtcNow.AddMinutes(5).AddHours(12).AddSeconds(9),
                    ChangedLot = _context.ParkingLots.First(),
                });
            }
            await _context.SaveChangesAsync(_ct);
            var accessEvents = await _aeRepository.GetAllAsync(_ct);
            var parkingLotEvents = await _pleRepository.GetAllAsync(_ct);
            var faceRecognitionEvents = await _freRepository.GetAllAsync(_ct);
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
