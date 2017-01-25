﻿using System;
using Alarm4Rest_Viewer.Services;
using Alarm4Rest.Data;
using System.Collections.ObjectModel;
using System.Timers;
using System.Collections.Generic;
using System.Windows.Input;

namespace Alarm4Rest_Viewer.QueryAlarmLists
{
    class QueryAlarmsListViewModel: PropertyChangeEventBase
    {

        //private IRestAlarmsRepository _repository = new RestAlarmsRepository();
        public static List<RestorationAlarmList> QueryAlarmListDump { get; private set; }

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

        private int _restAlarmCount;
        public int restAlarmCount
        {
            get { return _restAlarmCount; }
            set
            {
                _restAlarmCount = value;
                OnPropertyChanged("restAlarmCount");
            }

        }

        public static List<string> StationName { get; private set; }
        public QueryAlarmsListViewModel()
        {
            QueryAlarmListDump = new List<RestorationAlarmList>();
            QueryDetailCommand = new RelayCommand(O=>OnQueryDetailCommand(),O=>canShowDetail());

            FirstPageCommand = new RelayCommand(O => onFirstPageCommand(), O => canPrePageCommand());
            PrePageCommand = new RelayCommand(O => onPrePageCommand(), O => canPrePageCommand());
            NextPageCommand = new RelayCommand(O => onNextPageCommand(), O => canNextPageCommand());
            LastPageCommand = new RelayCommand(O => onLastPageCommand(), O => canNextPageCommand());
            EnterPageCommand = new RelayCommand(O => onEnterPageCommand(), O => canEnterPageCommand());

            RestAlarmsRepo.RestAlarmChanged += OnQueryRepoChanged;
            pageIndex = 1;
            restAlarmCount = 0;
            //RestAlarmsRepo.pageIndex = pageIndex;

            //_timer.Elapsed += (s, e) => NotificationMessage = "This is Alarm bar 2 : " + DateTime.Now.ToLocalTime();
            //_timer.Start();
        }

        //OnLoad Control
        /*
        public async void LoadRestorationAlarmsAsync()
        {
            try
            {
                RestEventArgs arg = new RestEventArgs();
                await RestAlarmsRepo.GetInitDataRepositoryAsync();
                RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmsRepo.RestAlarmListDump);
                pageCount = RestAlarmsRepo.pageCount;
                restAlarmCount = RestAlarmsRepo.restAlarmCount;
                arg.message = "hasLoaded";
                onRestAlarmChanged(arg);
                Console.WriteLine("Load Success");
            }
            catch
            {
                Console.WriteLine("Load Fail");
                NotificationMessage = "Can't connect to Database : " + DateTime.Now.ToLocalTime();
            }

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
            await RestAlarmsRepo.GetRestAlarmAct();

        }
        public RelayCommand EnterPageCommand { get; private set; }
        private bool canEnterPageCommand()
        {
            return (true);
        }

        private async void onEnterPageCommand()
        {
            if (_pageIndex <= 0) pageIndex = 1;
            if (_pageIndex > RestAlarmsRepo.pageCount) pageIndex = RestAlarmsRepo.pageCount;

            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            await RestAlarmsRepo.GetRestAlarmAct();
        }

        public RelayCommand PrePageCommand { get; private set; }

        private async void onPrePageCommand()
        {
            pageIndex -= 1;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            await RestAlarmsRepo.GetRestAlarmAct();

            //To Do function Update RestAlarmsRepo.RestAlarmListDump 
            //RestAlarmListDump = await RestAlarmsRepo.GetRestAlarmsAsync(pageIndex, pageSize);
            //RestorationAlarms = new ObservableCollection<RestorationAlarmList>(RestAlarmListDump);
        }
        public RelayCommand NextPageCommand { get; private set; }

        private async void onNextPageCommand()
        {
            pageIndex += 1;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            await RestAlarmsRepo.GetRestAlarmAct();
        }

        public RelayCommand LastPageCommand { get; private set; }
        private bool canNextPageCommand()
        {
            return (_pageIndex < RestAlarmsRepo.pageCount);
        }

        private async void onLastPageCommand()
        {
            pageIndex = RestAlarmsRepo.pageCount;
            Console.WriteLine(DateTime.Now.ToString() + " : goto page : " + _pageIndex);
            RestAlarmsRepo.pageIndex = pageIndex;
            await RestAlarmsRepo.GetRestAlarmAct();
        }

        
        //New alarm Process
        private void OnQueryRepoChanged(object source, RestEventArgs arg)
        {

            switch (arg.message)
            {
                case "hasNewAlarm":
                    Console.WriteLine(DateTime.Now.ToString() + " : Main Alarm List Recieved New Alarm");
                    if (RestAlarmsRepo.PreviousAlarmRecIndex < 0) break;
                    for (int i = RestAlarmsRepo.startNewRestItemArray; i >= 0; i--)
                    {
                        RestorationAlarms.Insert(0,RestAlarmsRepo.RestAlarmListDump[i]);
                        if (RestorationAlarms.Count > RestAlarmsRepo.pageSize) RestorationAlarms.RemoveAt(RestAlarmsRepo.pageSize);
                    }
                    pageCount = RestAlarmsRepo.pageCount;
                    restAlarmCount = RestAlarmsRepo.restAlarmCount;
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
                    restAlarmCount = RestAlarmsRepo.restAlarmCount;
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
                    restAlarmCount = RestAlarmsRepo.restAlarmCount;

                    break;
                case "GetRestAlarmNoResult":
                    Console.WriteLine(DateTime.Now.ToString() + " :  Main Alarm List has been filtered but no data");
                    RestorationAlarms.Clear();
                    pageCount = RestAlarmsRepo.pageCount;
                    restAlarmCount = RestAlarmsRepo.restAlarmCount;
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

        public RelayCommand QueryDetailCommand { get; private set; }

        public event Action<RestorationAlarmList> RestorationDetailRequested = delegate { };

        private bool canShowDetail()
        {
            return (_selectedEvent != null);
        }
        private void OnQueryDetailCommand()
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