using System;
using Alarm4Rest_Viewer.Services;
using Alarm4Rest.Data;
using System.Collections.ObjectModel;
using System.Timers;
using System.Collections.Generic;
using System.Windows.Input;

namespace Alarm4Rest_Viewer.RestorationAlarmLists
{
    class RestAlarmsListViewModel: PropertyChangeEventBase
    {

        //private IRestAlarmsRepository _repository = new RestAlarmsRepository();
        public static List<RestorationAlarmList> RestAlarmListDump { get; private set; }

        private Timer _timer = new Timer(5000);

        private int _pageIndex;
        public int pageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                OnPropertyChanged("pageIndex");
            }
        }

        private int _pageCount;
        public int pageCount
        {
            get { return _pageCount; }
            set
            {
                _pageCount = value;
                OnPropertyChanged("pageCount");
            }
        }
        public int pageSize { get; private set; }

        public static List<string> StationName { get; private set; }
        public RestAlarmsListViewModel()
        {
            RestAlarmListDump = new List<RestorationAlarmList>();
            RestorationDetailCommand = new RelayCommand(O=>OnRestorationDetailCommand(),O=>canShowDetail());

            FirstPageCommand = new RelayCommand(O => onFirstPageCommand(), O => canPrePageCommand());
            PrePageCommand = new RelayCommand(O => onPrePageCommand(), O => canPrePageCommand());
            NextPageCommand = new RelayCommand(O => onNextPageCommand(), O => canNextPageCommand());
            LastPageCommand = new RelayCommand(O => onLastPageCommand(), O => canNextPageCommand());
            EnterPageCommand = new RelayCommand(O => onEnterPageCommand(), O => canEnterPageCommand());

            RestAlarmsRepo.RestAlarmChanged += OnRestAlarmRepoChanged;
            pageIndex = 1;
            pageSize = 40;
            RestAlarmsRepo.pageSize = pageSize;
            RestAlarmsRepo.pageIndex = pageIndex;

            //_timer.Elapsed += (s, e) => NotificationMessage = "This is Alarm bar 2 : " + DateTime.Now.ToLocalTime();
            //_timer.Start();
        }

        /* WPF call method with parameter
        RelayCommand _EnterPageCommand;
        public ICommand EnterPageCommand
        {
            get
            {
                if (_EnterPageCommand == null)
                {
                    _EnterPageCommand = new RelayCommand(p => EnterPage(p),
                        p => true);
                }
                return _EnterPageCommand;
            }
        }

        private async void EnterPage(object txtPage)
        {

            pageIndex = Convert.ToInt32(txtPage);
            if (_pageIndex <= 0) pageIndex = 1;
            if (_pageIndex > RestAlarmsRepo.pageCount) pageIndex = RestAlarmsRepo.pageCount;

            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;

            //To Do function Update RestAlarmsRepo.RestAlarmListDump 
            RestAlarmListDump = await RestAlarmsRepo.GetRestAlarmsAsync(pageIndex, pageSize);
            RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmListDump);
        }
        */

        public RelayCommand FirstPageCommand { get; private set; }
        private bool canPrePageCommand()
        {
            return (_pageIndex > 1);
        }

        private async void onFirstPageCommand()
        {
            pageIndex = 1;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;

            //To Do function Update RestAlarmsRepo.RestAlarmListDump 
            RestAlarmListDump = await RestAlarmsRepo.GetRestAlarmsAsync(pageIndex, pageSize);
            RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmListDump);

        }
        public RelayCommand EnterPageCommand { get; private set; }
        private bool canEnterPageCommand()
        {
            return (true);
        }

        private void onEnterPageCommand()
        {
            if (_pageIndex <= 0) pageIndex = 1;
            if (_pageIndex > RestAlarmsRepo.pageCount) pageIndex = RestAlarmsRepo.pageCount;

            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            RestAlarmsRepo.GetRestAlarmAct();
        }

        public RelayCommand PrePageCommand { get; private set; }

        private void onPrePageCommand()
        {
            pageIndex -= 1;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            RestAlarmsRepo.GetRestAlarmAct();

            //To Do function Update RestAlarmsRepo.RestAlarmListDump 
            //RestAlarmListDump = await RestAlarmsRepo.GetRestAlarmsAsync(pageIndex, pageSize);
            //RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmListDump);
        }
        public RelayCommand NextPageCommand { get; private set; }

        private void onNextPageCommand()
        {
            pageIndex += 1;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            RestAlarmsRepo.GetRestAlarmAct();
        }

        public RelayCommand LastPageCommand { get; private set; }
        private bool canNextPageCommand()
        {
            return (_pageIndex < RestAlarmsRepo.pageCount);
        }

        private void onLastPageCommand()
        {
            pageIndex = RestAlarmsRepo.pageCount;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            RestAlarmsRepo.GetRestAlarmAct();
        }

        //OnLoad Control
        public async void LoadRestorationAlarmsAsync()
        {
            RestEventArgs arg = new RestEventArgs();
            await RestAlarmsRepo.GetInitDataRepositoryAsync();
            RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmsRepo.RestAlarmListDump);
            pageCount = RestAlarmsRepo.pageCount;
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
                        if (RestorationAlarms.Count>pageSize) RestorationAlarms.RemoveAt(pageSize);
                        RestorationAlarms.Insert(0,RestAlarmsRepo.RestAlarmListDump[i]);
                        
                    }
                    pageCount = RestAlarmsRepo.pageCount;
                    NotificationMessage = "Has New Alarm : " + DateTime.Now.ToLocalTime();
                    break;

                case "Reset":
                    Console.WriteLine(DateTime.Now.ToString() + " : Main Alarm List Recieved Reset");
                    RestorationAlarms.Clear();
                    for (int i = RestAlarmsRepo.startNewRestItemArray; i >= 0; i--)
                    {
                        RestorationAlarms.Insert(0,RestAlarmsRepo.RestAlarmListDump[i]);
                    }
                    pageCount = RestAlarmsRepo.pageCount;
                    NotificationMessage = "Database has been reset : " + DateTime.Now.ToLocalTime();
                    break;

                case "GetRestAlarm":
                    Console.WriteLine(DateTime.Now.ToString() + " : Main Alarm List Execute Navigation");
                    RestorationAlarms.Clear();
                    for (int i = RestAlarmsRepo.startNewRestItemArray; i >= 0; i--)
                    {
                        RestorationAlarms.Insert(0, RestAlarmsRepo.RestAlarmListDump[i]);
                    }
                    pageCount = RestAlarmsRepo.pageCount;

                    break;
                case "GetRestAlarmNoResult":
                    Console.WriteLine(DateTime.Now.ToString() + " :  Main Alarm List has been filtered but no data");
                    RestorationAlarms.Clear();
                    pageCount = RestAlarmsRepo.pageCount;
                    NotificationMessage = "Main Alarm List No : " + DateTime.Now.ToLocalTime();
                    break;

                default:
                    Console.WriteLine(DateTime.Now.ToString() + " :  Main Alarm List Default");

                    break;
            }
        }

        private ObservableCollection<RestorationAlarmList> _restorationAlarms;
        //public static List<RestorationAlarmList> CustAlarmListDump { get; private set; }
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
