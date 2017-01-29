using System;
using Alarm4Rest_Viewer.Services;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq.Expressions;
using Alarm4Rest.Data;
using System.Linq;
using System.Collections.ObjectModel;

namespace Alarm4Rest_Viewer.CustControl
{
    class mainRibbonTapViewModel : PropertyChangeEventBase
    {

        #region Properties
        public static List<SortItem> sortOrderList = new List<SortItem>();
        public static List<string> qSelectedGroupDescription = new List<string>();
        public static List<string> fltSelectedPriority = new List<string>();

        public Expression<Func<RestorationAlarmList, bool>> queryParseDeleg;
        public static List<Item> qFilters = new List<Item>();

        private HashSet<Item> mQCheckedItems;        
        
        //-----------------------------------SortFiel Items----------------------------------//
        private ObservableCollection<Item> mfieldItems1;
        public IEnumerable<Item> fieldItems1 { get { return mfieldItems1; } }

        private ObservableCollection<Item> mfieldItems2;
        public IEnumerable<Item> fieldItems2 { get { return mfieldItems2; } }

        private ObservableCollection<Item> mfieldItems3;
        public IEnumerable<Item> fieldItems3 { get { return mfieldItems3; } }


        //-----------------------------------Category----------------------------------//
        private Item _CatDesc87X;
        private Item _CatDesc56N;
        private TimeCondItem _Today;
        private TimeCondItem _Last2Days;
        private TimeCondItem _ThisWeek;
        private TimeCondItem _Last2Weeks;
        private TimeCondItem _ThisMonth;
        private TimeCondItem _EveryDays;
        private TimeCondItem _UserSel;

        public Item CatDesc87X { get { return _CatDesc87X; } }
        public Item CatDesc56N { get { return _CatDesc56N; } }


        //---------------------------------Time Filtering-----------------------------------//
        public TimeCondItem Today { get { return _Today; } }
        public TimeCondItem Last2Days { get { return _Last2Days; } }
        public TimeCondItem ThisWeek { get { return _ThisWeek; } }
        public TimeCondItem Last2Weeks { get { return _Last2Weeks; } }
        public TimeCondItem ThisMonth { get { return _ThisMonth; } }
        public TimeCondItem EveryDays { get { return _EveryDays; } }
        public TimeCondItem UserSel { get { return _UserSel; } }

        private DateTime _exclusiveEnd = new DateTime();
        private DateTime _exclusiveStart = new DateTime();

        public DateTime ExclusiveEnd
        {
            get
            {
                return _exclusiveEnd;
            }
            set
            {
                if (value != null)
                {
                    RestAlarmsRepo.qDateTimeCondEnd = value.AddDays(1);
                    _exclusiveEnd = value;
                }
            }
        }

        public DateTime ExclusiveStart
        {
            get
            {

                return _exclusiveStart;
            }
            set
            {
                RestAlarmsRepo.qDateTimeCondStart = value;
                _exclusiveStart = value;
            }
        }
        #endregion

        //------------------------------Class Construction--------------------------------------//
        public mainRibbonTapViewModel()
        {
            //To Do ต้องประการที่เดียว
            InitSortOrderTemplate();
            InitCategoryFiltering();
            InitTimeFiltering();
            InitSortOrderField();

            RestAlarmsRepo.qDateTimeCondItem = _Last2Weeks;

            RestAlarmsRepo.orderParseDeleg = sortOrderList.First(i => i.ID == 1);

            RunUserQueryCmd = new RelayCommand(o => onUserQuery(), o => canUserQuery());
        }

        //------------------------------Helper Function--------------------------------------//
        private void InitSortOrderField()
        {
            mfieldItems1 = new ObservableCollection<Item>();
            mfieldItems1.Add(new Item("first", "DateTime"));
            mfieldItems1.Add(new Item("first", "StationName"));
            mfieldItems1.Add(new Item("first", "Priority"));

            mfieldItems2 = new ObservableCollection<Item>();
            mfieldItems2.Add(new Item("second", "DateTime"));
            mfieldItems2.Add(new Item("second", "StationName"));
            mfieldItems2.Add(new Item("second", "Priority"));

            mfieldItems3 = new ObservableCollection<Item>();
            mfieldItems3.Add(new Item("third", "DateTime"));
            mfieldItems3.Add(new Item("third", "StationName"));
            mfieldItems3.Add(new Item("third", "Priority"));

        }
        private void InitSortOrderTemplate()
        {
            sortOrderList.Add(new SortItem(1, "StationName", "DateTime", "Priority"));
            sortOrderList.Add(new SortItem(2, "StationName", "Priority", "DateTime"));
            sortOrderList.Add(new SortItem(3, "DateTime", "StationName", "Priority"));
        }

        private void InitCategoryFiltering()
        {
            //isChecked87X = true;

            _CatDesc87X = new Item("Group87X", "87X", "GroupDescription");
            _CatDesc87X.IsChecked = true;
            if (_CatDesc87X.IsChecked) qFilters.Add(_CatDesc87X);

            _CatDesc56N = new Item("Group56N", "56N", "GroupDescription");
            _CatDesc56N.IsChecked = false;
            if (_CatDesc56N.IsChecked) qFilters.Add(_CatDesc56N);
        }

        private void InitTimeFiltering()
        {
            _Today = new TimeCondItem("Day", 1, false);
            _Last2Days = new TimeCondItem("Day", 2, false);
            _ThisWeek = new TimeCondItem("Week", 1, false);
            _Last2Weeks = new TimeCondItem("Week", 2, true); //Default
            _ThisMonth = new TimeCondItem("Month", 1, false);
            _EveryDays = new TimeCondItem("All", 1, false);
            _UserSel = new TimeCondItem("User", 1, false);

            _exclusiveEnd = DateTime.Now;
            _exclusiveStart = _exclusiveEnd.AddDays((-1) * 5);
    }


        #region Sort Function
        /* WPF call method with 1 parameter*/

        RelayCommand _RunStdSortQuery;
        public ICommand RunStdSortQuery
        {
            get
            {
                if (_RunStdSortQuery == null)
                {
                    _RunStdSortQuery = new RelayCommand(p => onRunStdSort(p), p => true);
                }
                return _RunStdSortQuery;
            }
        }

        private async void onRunStdSort(object txtSortTemplate)
        {
            //CustAlarmViewModel = null;

            int sortTemplate = Convert.ToInt32(txtSortTemplate);
            RestAlarmsRepo.orderParseDeleg = sortOrderList.First(i => i.ID == sortTemplate);

            RestAlarmsRepo.qDateTimeCondEnd = DateTime.Now;

            await RestAlarmsRepo.TGetQueryAlarmAct();

            Console.WriteLine(RestAlarmsRepo.orderParseDeleg.ID);
        }

        /* WPF call method with 2 parameter*/
        RelayCommand _RunQueryTimeCondCmd;
        public ICommand RunQueryTimeCondCmd
        {
            get
            {
                if (_RunQueryTimeCondCmd == null)
                {
                    _RunQueryTimeCondCmd = new RelayCommand(p => RunQueryTimeCond(), p => true);
                }
                return _RunQueryTimeCondCmd;
            }
        }

        // WPF Call with 2 parameter
        private void RunQueryTimeCond()
        {
            TimeCondItem Dummy = new TimeCondItem();

            if (_Today.IsChecked)
            {
                Dummy = _Today.Clone();
            }
            else if (_Last2Days.IsChecked)
            {
                Dummy = _Last2Days.Clone();
            }
            else if(_ThisWeek.IsChecked)
            {
                Dummy = _ThisWeek.Clone();
            }
            else if (_Last2Weeks.IsChecked)
            {
                Dummy = _Last2Weeks.Clone();
            }
            else if (_ThisMonth.IsChecked)
            {
                Dummy = _ThisMonth.Clone();
            }
            else if (_EveryDays.IsChecked)
            {
                Dummy = _EveryDays.Clone();
            }
            else if(_UserSel.IsChecked)
            {
                Dummy = _UserSel.Clone();
            }

            RestAlarmsRepo.qDateTimeCondItem = Dummy;
            //RestAlarmsRepo.qDateTimeCondEnd = DateTime.Now;
            //await RestAlarmsRepo.TGetQueryAlarmAct();

        }

        #endregion Sort Function


        #region User Query Function
        public RelayCommand RunUserQueryCmd { get; private set; }

        public bool canUserQuery()
        {
            return qFilters.Count != 0;
        }
        public async void onUserQuery()
        {

            Console.WriteLine("Run Standard Query cmd");

            IEnumerable<IGrouping<string, Item>> groupFields =
                    from item in qFilters
                    group item by item.FieldName;

            // Preparing for New Database
            //queryParseDeleg = FilterExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields);

            RestAlarmsRepo.filterParseDeleg = queryParseDeleg;
            //RestAlarmsRepo.qDateTimeCondEnd = DateTime.Now;
            await RestAlarmsRepo.TGetQueryAlarmAct();

        }

        RelayCommand _CheckCommand;
        public ICommand CheckCommand
        {
            get
            {
                if (_CheckCommand == null)
                {
                    _CheckCommand = new RelayCommand(p => onCheckCommand(p), p => true);
                }
                return _CheckCommand;
            }
        }

        private void onCheckCommand(object Category)
        {

            if ((string)Category == _CatDesc87X.Name)
            {
                AddRemoveProcess(ref _CatDesc87X);
            }
            if ((string)Category == _CatDesc56N.Name)
            {
                AddRemoveProcess(ref _CatDesc56N);
            }
            else
            {

            }

        }
        private void AddRemoveProcess(ref Item _CatDescX)
        {
            Item CatTemp = _CatDescX;

            var qf = qFilters
                    .Where(i => i.Value == CatTemp.Value && i.FieldName == CatTemp.FieldName).ToList();

            if (CatTemp.IsChecked)
            {
                if (qf.Count == 0) qFilters.Add(CatTemp);
            }
            else
            {
                if (qf.Count != 0)
                {
                    foreach (var item in qf)
                        qFilters.Remove(item);
                }
            }
        }

        ///* WPF call method with 1 parameter*/
        //private bool isChecked87X;
        //public bool IsChecked87X
        //{
        //    get { return isChecked87X; }
        //    set
        //    {
        //        isChecked87X = value;
        //       // OnPropertyChanged("IsChecked87X");
        //    }
        //}


        #endregion User Query Function

    }
}