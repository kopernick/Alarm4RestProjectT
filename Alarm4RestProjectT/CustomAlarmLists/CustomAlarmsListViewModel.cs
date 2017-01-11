using System;
using Alarm4Rest_Viewer.Services;
using Alarm4Rest.Data;
using System.Collections.ObjectModel;
using System.Timers;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Alarm4Rest_Viewer.CustomAlarmLists
{
    class CustomAlarmListViewModel: PropertyChangeEventBase
    {

        //private IRestAlarmsRepository _repository = new RestAlarmsRepository();
        public int pageIndex { get; private set; }
        public int pageSize { get; private set; }

        private Timer _timer = new Timer(5000);

        public CustomAlarmListViewModel()
        {
            RestorationDetailCommand = new RelayCommand(o=>OnRestorationDetailCommand(),o=>canShowDetail());
            //LoadDataCmd = new RelayCommand(LoadCustomAlarms);
            //RunFilterCmd = new RelayCommand (o=>onFilterAlarms(),o=>canFilter());
            CustomAlarms = new ObservableCollection<RestorationAlarmList>();

           RestAlarmsRepo.RestAlarmChanged += OnRestAlarmChanged;
            pageIndex = 1;
            pageSize = 40;

            //_timer.Elapsed += (s, e) => NotificationMessage = "This is Alarm bar 2 : " + DateTime.Now.ToLocalTime();
            //_timer.Start();
        }


        //New alarm Process
        private void OnRestAlarmChanged(object source, RestEventArgs arg)
        {

            switch (arg.message)
            {
                case "hasNewAlarmCust":
                    Console.WriteLine(DateTime.Now.ToString() + " : Custom Alarm List Recieved New Alarm");
                    if (RestAlarmsRepo.PreviousAlarmRecIndex < 0) break;
                    for (int i = RestAlarmsRepo.startNewCustItemArray; i >= 0; i--)
                    {
                        CustomAlarms.Insert(0, RestAlarmsRepo.RestAlarmListDump[i]);
                        if (CustomAlarms.Count > pageSize) CustomAlarms.RemoveAt(pageSize);
                    }

                    NotificationMessage = "Has New Alarm : " + DateTime.Now.ToLocalTime();
                    break;

                case "hasAllNewAlarmCust":
                    Console.WriteLine(DateTime.Now.ToString() + " :  Custom Alarm List Recieved All new or Reset");
                    CustomAlarms.Clear();
                    for (int i = RestAlarmsRepo.startNewCustItemArray; i >= 0; i--)
                        CustomAlarms.Insert(0,RestAlarmsRepo.CustAlarmListDump[i]);

                    NotificationMessage = "Database has been reset : " + DateTime.Now.ToLocalTime();
                    break;

                case "filterAlarmCust":
                    Console.WriteLine(DateTime.Now.ToString() + " :  Custom Alarm List has been filtered");
                    CustomAlarms.Clear();
                    for (int i = RestAlarmsRepo.startNewCustItemArray; i >= 0; i--)
                        CustomAlarms.Insert(0, RestAlarmsRepo.CustAlarmListDump[i]);

                    NotificationMessage = "Filtering : " + DateTime.Now.ToLocalTime();
                    break;

                case "filterAlarmCustNoResult":
                    Console.WriteLine(DateTime.Now.ToString() + " :  Custom Alarm List has been filtered but no data");
                    CustomAlarms.Clear();
                    NotificationMessage = "No result Filtering : " + DateTime.Now.ToLocalTime();
                    break;
                    

                default:
                    Console.WriteLine(DateTime.Now.ToString() + " :  Custom Alarm List Default");
                    break;
            }
        }
        //public bool canFilter()
        //{
        //    return RestAlarmsRepo.filter_Parse.Count != 0; ;
        //}
        //public async void onFilterAlarms()
        //{

        //    CustomAlarms = await RestAlarmsRepo.GetCustomRestAlarmsAsync(RestAlarmsRepo.filter_Parse, pageIndex, pageSize);
        //    RestAlarmsRepo.LastCustomAlarmRecIndex = CustomAlarms[0].PkAlarmListID; //Get Last PkAlarmList initializing

        //    //CustomAlarms = new ObservableCollection<RestorationAlarmList>(await RestAlarmsRepo.GetRestAlarmsAsync());
        //}

        private ObservableCollection<RestorationAlarmList> _restorationAlarms;
        public ObservableCollection<RestorationAlarmList> CustomAlarms
        {
            get { return _restorationAlarms; }
            set
            {
                _restorationAlarms = value;
                OnPropertyChanged("CustomAlarms");
            }
        }

        public RelayCommand RestorationDetailCommand { get; private set; }
        public ICommand RunFilterCmd { get; private set; }

        public event Action<RestorationAlarmList> RestorationDetailRequested = delegate { };

        private bool canLoadData()
        {
            return (RestAlarmsRepo.RestAlarmListDump.Count != 0);
        }
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



    }
}
