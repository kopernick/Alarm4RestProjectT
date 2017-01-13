using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alarm4Rest_Viewer.Services;
using Alarm4Rest_Viewer.CustomAlarmLists;
using Alarm4Rest_Viewer.CustControl;
using Alarm4Rest_Viewer.RestorationAlarmLists;

namespace Alarm4Rest_Viewer
{
    public class MainWindowViewModel : PropertyChangeEventBase
    {

        private filterToolBarViewModel _filterToolViewModel = new filterToolBarViewModel();
        private searchToolBarViewModel _searchToolViewModel = new searchToolBarViewModel();
        private CustomAlarmListViewModel _custAlarmViewModel = new CustomAlarmListViewModel();

        private PropertyChangeEventBase _CurrentToolViewModel;
        private PropertyChangeEventBase _CustAlarmViewModel;
        public RelayCommand EnableSearchCmd { get; private set; }
        public RelayCommand EnableFilterCmd { get; private set; }
        public RelayCommand EnableCustView { get; private set; }

        public MainWindowViewModel()
        {
            //_filterToolViewModel = new filterToolBarViewModel();
            //_searchToolViewModel = new searchToolBarViewModel();
            RestAlarmsRepo.InitializeRepository();

            EnableSearchCmd = new RelayCommand(o => onSearchAlarms(), o => canSearch());
            EnableFilterCmd = new RelayCommand(o => onFilterAlarms(), o => canFilter());
            EnableCustView = new RelayCommand(o => onCustView(), o => canViewMain());

        }

        public PropertyChangeEventBase  CurrentToolViewModel
        {
            get { return _CurrentToolViewModel; }
            set { SetProperty(ref _CurrentToolViewModel, value); }
        }

        public PropertyChangeEventBase CustAlarmViewModel
        {
            get { return _CustAlarmViewModel; }
            set { SetProperty(ref _CustAlarmViewModel, value); }
        }

        public void onSearchAlarms()
        {

            //ExpressGen();
            Console.WriteLine("Run Search cmd");
            //CurrentToolViewModel = null;
            CurrentToolViewModel = _searchToolViewModel;
            CustAlarmViewModel = _custAlarmViewModel;

        }

        public void onFilterAlarms()
        {
            //ExpressGen();
            Console.WriteLine("Run Filter cmd");
            CurrentToolViewModel = _filterToolViewModel;
            CustAlarmViewModel = _custAlarmViewModel;
            //RestorationAlarmLists.RestAlarmsListViewModel.LoadRestorationAlarmsAsync();

        }
        public void onCustView()
        {
            //ExpressGen();
            Console.WriteLine("Run MainView cmd");
            CurrentToolViewModel = null;
            CustAlarmViewModel = null;
            //RestorationAlarmLists.RestAlarmsListViewModel.LoadRestorationAlarmsAsync();

        }
        public bool canSearch()
        {
            return true;
        }

        public bool canFilter()
        {
            return true;
        }
        public bool canViewMain()
        {
            return true;
        }

    }
}
