using System;
using Alarm4Rest_Viewer.Services;
using Alarm4Rest.Data;
using System.Collections.ObjectModel;
using System.Timers;
using System.Collections.Generic;

namespace Alarm4Rest_Viewer.RestorationAlarmLists
{
    class RestAlarmsListViewModel: PropertyChangeEventBase
    {

        //private IRestAlarmsRepository _repository = new RestAlarmsRepository();

        private Timer _timer = new Timer(5000);

        public int pageIndex { get; private set; }
        public int pageSize { get; private set; }

        public static List<string> StationName { get; private set; }
        public RestAlarmsListViewModel()
        {
            RestorationDetailCommand = new RelayCommand(O=>OnRestorationDetailCommand(),O=>canShowDetail());

            RestAlarmsRepo.RestAlarmChanged += OnRestAlarmRepoChanged;
            pageIndex = 1;
            pageSize = 40;
            RestAlarmsRepo.pageIndex = pageSize;
            RestAlarmsRepo.pageIndex = pageIndex;

            //_timer.Elapsed += (s, e) => NotificationMessage = "This is Alarm bar 2 : " + DateTime.Now.ToLocalTime();
            //_timer.Start();
        }

        //OnLoad Control
        public async void LoadRestorationAlarmsAsync()
        {
            RestEventArgs arg = new RestEventArgs();
            await RestAlarmsRepo.GetInitDataRepositoryAsync();
            RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmsRepo.RestAlarmListDump);
            arg.message = "hasLoaded";
            onRestAlarmChanged(arg);
            Console.WriteLine("Load Success");
        }

        //New alarm Process
        private void OnRestAlarmRepoChanged(object source, RestEventArgs arg)
        {

            switch (arg.message)
            {
                case "hasNewAlarm":
                    Console.WriteLine(DateTime.Now.ToString() + " : Main Alarm List Recieved New Alarm");
                    if (RestAlarmsRepo.PreviousAlarmRecIndex < 0) break;
                    for (int i = RestAlarmsRepo.startNewRestItemArray; i >= 0; i--)
                    {
                        RestorationAlarms.Insert(0,RestAlarmsRepo.RestAlarmListDump[i]);
                        if(RestorationAlarms.Count>pageSize) RestorationAlarms.RemoveAt(pageSize);
                    }

                    NotificationMessage = "Has New Alarm : " + DateTime.Now.ToLocalTime();
                    break;

                case "Reset":
                    Console.WriteLine(DateTime.Now.ToString() + " : Main Alarm List Recieved Reset");
                    RestorationAlarms.Clear();
                    for (int i = RestAlarmsRepo.startNewRestItemArray; i >= 0; i--)
                    {
                        RestorationAlarms.Insert(0,RestAlarmsRepo.RestAlarmListDump[i]);
                    }

                    NotificationMessage = "Database has been reset : " + DateTime.Now.ToLocalTime();
                    break;

                default:
                    
                    break;
            }
        }

        private ObservableCollection<RestorationAlarmList> _restorationAlarms;
        public ObservableCollection<RestorationAlarmList> RestorationAlarms
        {
            get { return _restorationAlarms; }
            set
            {
                _restorationAlarms = value;
                OnPropertyChanged("RestorationAlarms");
            }
        }

        public RelayCommand RestorationDetailCommand { get; private set; }

        public event Action<RestorationAlarmList> RestorationDetailRequested = delegate { };

        private bool canShowDetail()
        {
            return (_selectedEvent != null);
        }
        private void OnRestorationDetailCommand()
        {
            RestorationDetailRequested(_selectedEvent);
        }


        private RestorationAlarmList _selectedEvent;

        public RestorationAlarmList SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                if (_selectedEvent != value)
                {
                    _selectedEvent = value;
                    OnPropertyChanged("SelectedEvent");

                }
            }
        }
        private string _NotificationMessage;
        public string NotificationMessage
        {
            get { return _NotificationMessage; }
            set
            {
                if (value != _NotificationMessage)
                {
                    _NotificationMessage = value;
                    OnPropertyChanged("NotificationMessage");
                }
            }
        }

        public static event EventHandler<RestEventArgs> RestAlarmChanged;
        private static void onRestAlarmChanged(RestEventArgs arg)
        {
            if (RestAlarmChanged != null)
                RestAlarmChanged(null, arg);
        }


    }
}
