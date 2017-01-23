using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alarm4Rest_Viewer.Services;
using Alarm4Rest_Viewer.CustomAlarmLists;
using Alarm4Rest_Viewer.CustControl;
using Alarm4Rest_Viewer.RestorationAlarmLists;
using Alarm4Rest.Data;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace Alarm4Rest_Viewer
{
    public class MainWindowViewModel : PropertyChangeEventBase
    {

        //private filterToolBarViewModel _filterToolViewModel = new filterToolBarViewModel();
        //private searchToolBarViewModel _searchToolViewModel = new searchToolBarViewModel();

        public RelayCommand EnableSearchCmd { get; private set; }
        public RelayCommand EnableFilterCmd { get; private set; }
        public RelayCommand EnableCustView { get; private set; }

        #region Sorting Templat
        public static List<SortItem> sortOrderList = new List<SortItem>();
        #endregion

        #region Filter Properties
        public static List<string> fltSelectedStations = new List<string>();
        public static List<string> fltSelectedPriority = new List<string>();
        public static List<string> fltSelectedMessage = new List<string>();
        public static List<string> fltSelectedGroupDescription = new List<string>();
        public Expression<Func<RestorationAlarmList, bool>> filterParseDeleg;

        public static List<Item> filters = new List<Item>();

        private HashSet<Item> mfltCheckedItems;

        private ObservableCollection<Item> mfltPriorityItems;
        public IEnumerable<Item> fltPriorityItems { get { return mfltPriorityItems; } }


        private ObservableCollection<Item> mfltGroupDescItems;
        public IEnumerable<Item> fltGroupDescItems { get { return mfltGroupDescItems; } }

        private ObservableCollection<Item> mfltMessageItems;
        public IEnumerable<Item> fltMessageItems { get { return mfltMessageItems; } }

        private ObservableCollection<Item> mfltStationItems;
        public IEnumerable<Item> fltStationItems { get { return mfltStationItems; } }

        private ObservableCollection<string> _fltStations;
        public ObservableCollection<string> fltStations
        {
            get { return _fltStations; }
            set
            {
                _fltStations = value;
                OnPropertyChanged("fltStations");
            }
        }
        public string fltSelectedStationsView
        {
            get { return _fltSelectedStationsView; }
            set
            {
                Set(ref _fltSelectedStationsView, value);
                OnPropertyChanged("fltSelectedStationsView");
            }
        }
        public string fltSelectedPriorityView
        {
            get { return _fltSelectedPriorityView; }
            set
            {
                Set(ref _fltSelectedPriorityView, value);
                OnPropertyChanged("fltSelectedPriorityView");
            }
        }
        public string fltSelectedGroupDescriptionView
        {
            get { return _fltSelectedGroupDescriptionView; }
            set
            {
                Set(ref _fltSelectedGroupDescriptionView, value);
                OnPropertyChanged("fltSelectedGroupDescriptionView");
            }
        }

        public string fltSelectedMessageView
        {
            get { return _fltSelectedMessageView; }
            set
            {
                Set(ref _fltSelectedMessageView, value);
                OnPropertyChanged("fltSelectedMessageView");
            }
        }
        
        private string _fltSelectedStationsView;
        private string _fltSelectedPriorityView;
        private string _fltSelectedGroupDescriptionView;
        private string _fltSelectedMessageView;
        public string filterText
        {
            get { return _filterText; }
            set
            {
                Set(ref _filterText, value);
                OnPropertyChanged("Text");
            }

        }

        private int _pageSize;
        public int pageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                OnPropertyChanged("pageSize");
            }
        }

        private string _filterText;
        #endregion

        #region Search Properties
        public static List<string> selectedStations = new List<string>();
        public static List<string> selectedField = new List<string>();
        public static List<string> selectedGroupDescription = new List<string>();
        public Expression<Func<RestorationAlarmList, bool>> searchParseDeleg;

        public static List<Item> searchList = new List<Item>();

        private HashSet<Item> mCheckedItems;

        private ObservableCollection<Item> mfieldItems;
        public IEnumerable<Item> fieldItems { get { return mfieldItems; } }


        private ObservableCollection<Item> mpriorityItems;
        public IEnumerable<Item> priorityItems { get { return mpriorityItems; } }


        private ObservableCollection<Item> mstationItems;
        public IEnumerable<Item> stationItems { get { return mstationItems; } }

        private ObservableCollection<string> _Stations;
        public ObservableCollection<string> Stations
        {
            get { return _Stations; }
            set
            {
                _Stations = value;
                OnPropertyChanged("Stations");
            }
        }
        public string selectedStationsView
        {
            get { return _selectedStationsView; }
            set
            {
                Set(ref _selectedStationsView, value);
                OnPropertyChanged("selectedStationsView");
            }
        }
        public string selectedFieldView
        {
            get { return _selectedFieldView; }
            set
            {
                Set(ref _selectedFieldView, value);
                OnPropertyChanged("selectedFieldView");
            }
        }
        public string selectedPriorityView
        {
            get { return _selectedPriorityView; }
            set
            {
                Set(ref _selectedPriorityView, value);
                OnPropertyChanged("selectedPriorityView");
            }
        }

        public string _selectedStationsView;
        public string _selectedFieldView;
        public string _selectedPriorityView;

        public string searchText
        {
            get { return _searchText; }
            set
            {
                Set(ref _searchText, value);
                OnPropertyChanged("Text");
            }

        }

        private string _searchText;
        //Default Search Keyword
        private string _search_Parse_Pri;
        public string search_Parse_Pri
        {
            get { return _search_Parse_Pri; }
            set
            {
                _search_Parse_Pri = value;
                OnPropertyChanged("search_Parse_Pri");
                // RestAlarmsRepo.filter_Parse = value;
            }
        }

        //Option Search Keyword
        private string _search_Parse_Sec;
        public string search_Parse_Sec
        {
            get { return _search_Parse_Sec; }
            set
            {
                _search_Parse_Sec = value;
                OnPropertyChanged("search_Parse_Sec");
                // RestAlarmsRepo.filter_Parse = value;
            }
        }

        #endregion
        public MainWindowViewModel()
        {
            //_filterToolViewModel = new filterToolBarViewModel();
            //_searchToolViewModel = new searchToolBarViewModel();

            RestAlarmsRepo.InitializeRepository(); // Start define --> DBContext = new Alarm4RestorationContext();
            //CustAlarmViewModel = new CustomAlarmListViewModel();

            EnableSearchCmd = new RelayCommand(o => onSearchAlarms(), o => canSearch());
            EnableFilterCmd = new RelayCommand(o => onFilterAlarms(), o => canFilter());
            EnableCustView = new RelayCommand(o => onCustView(), o => canViewMain());

            RestAlarmsListViewModel.RestAlarmChanged += OnRestAlarmChanged;

            pageSize = RestAlarmsRepo.pageSize;
            SetPageSize = new RelayCommand(o => onSetPageSize(), o => canSetPageSize());

            #region Initialize Sort Template menu
            InitSortOrderTemplate();
            #endregion

            #region Initialize filter menu
            mfltStationItems = new ObservableCollection<Item>();
            mfltPriorityItems = new ObservableCollection<Item>();
            mfltGroupDescItems = new ObservableCollection<Item>();
            mfltMessageItems = new ObservableCollection<Item>();
            mfltCheckedItems = new HashSet<Item>();

            mfltStationItems.CollectionChanged += fltItems_CollectionChanged;
            mfltPriorityItems.CollectionChanged += fltItems_CollectionChanged;
            mfltGroupDescItems.CollectionChanged += fltItems_CollectionChanged;
            mfltMessageItems.CollectionChanged += fltItems_CollectionChanged;

            RunFilterCmd = new RelayCommand(o => onFilterAlarms(), o => canFilter());
       #endregion

      #region Initialize Search menu
            mstationItems = new ObservableCollection<Item>();
            mfieldItems = new ObservableCollection<Item>();
            mpriorityItems = new ObservableCollection<Item>();
            mCheckedItems = new HashSet<Item>();

            mstationItems.CollectionChanged += Items_CollectionChanged;
            mfieldItems.CollectionChanged += Items_CollectionChanged;
            mpriorityItems.CollectionChanged += Items_CollectionChanged;
            search_Parse_Pri = "";
            search_Parse_Sec = "";

            RunSearchCmd = new RelayCommand(o => onSearchAlarms(), o => canSearch());

        #endregion
        }

        #region Helper Function

        public RelayCommand SetPageSize { get; private set; }

        public bool canSetPageSize()
        {
            return (RestAlarmsRepo.pageSize != pageSize && pageSize != 0);
        }
        public async void onSetPageSize()
        {
            RestAlarmsRepo.pageSize = pageSize;
            await RestAlarmsRepo.GetRestAlarmAct();
            await RestAlarmsRepo.GetCustAlarmAct();
        }

        #endregion
        #region Sort Template Helper function
        private void InitSortOrderTemplate()
        {
            sortOrderList.Add(new SortItem("StationName", "DateTime", "Priority"));
            sortOrderList.Add(new SortItem("StationName", "Priority", "DateTime"));
            sortOrderList.Add(new SortItem("DateTime", "StationName", "Priority"));
        }
        #endregion

            #region Filter Helper function
        private void OnRestAlarmChanged(object source, RestEventArgs arg)
        {
            if (arg.message == "hasLoaded")
            {

                // Adding Station ComboBox items
                foreach (var Station in RestAlarmsRepo.StationsName)
                {
                    mfltStationItems.Add(new Item(Station.ToString(), "StationName"));
                    mstationItems.Add(new Item(Station.ToString(), "StationName"));
                }

                // Adding Priority ComboBox items
                foreach (var Priority in RestAlarmsRepo.Priority)
                {
                    mfltPriorityItems.Add(new Item(Priority.ToString(), "Priority"));
                    mpriorityItems.Add(new Item(Priority.ToString(), "Priority"));
                }

                // Adding GoupDescription ComboBox items
                foreach (var GroupDescription in RestAlarmsRepo.GroupDescription)
                {
                    mfltGroupDescItems.Add(new Item(GroupDescription.ToString(), "GroupDescription"));
                }

                // Adding Message ComboBox items
                foreach (var Message in RestAlarmsRepo.Message)
                {
                    mfltMessageItems.Add(new Item(Message.ToString(), "Message"));
                }
                             
                // Adding Search field items
                mfieldItems.Add(new Item("PointName", "FieldName"));
                mfieldItems.Add(new Item("Message", "FieldName"));
                //mpfieldItems.Add(new Item("Priority", "FieldName"));
                mfieldItems.Add(new Item("GroupPointName", "FieldName"));
                mfieldItems.Add(new Item("GroupDescription", "FieldName"));
            }
        }
        private void fltItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item item in e.OldItems)
                {
                    item.PropertyChanged -= fltItem_PropertyChanged;
                    mfltCheckedItems.Remove(item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (Item item in e.NewItems)
                {
                    item.PropertyChanged += fltItem_PropertyChanged;
                    if (item.IsChecked) mfltCheckedItems.Add(item);
                }
            }

        }

        private void fltItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                Item item = (Item)sender;
                if (item.IsChecked)
                {
                    mfltCheckedItems.Add(item);
                    switch (item.FieldName)
                    {
                        case "StationName":
                            fltSelectedStations.Add(item.Value.TrimEnd());
                            break;
                        case "Priority":
                            fltSelectedPriority.Add(item.Value.TrimEnd());
                            break;
                        case "Message":
                            fltSelectedMessage.Add(item.Value.TrimEnd());
                            break;
                        case "GroupDescription":
                            fltSelectedGroupDescription.Add(item.Value.TrimEnd());
                            break;
                    }

                    //selectedStations.Add(item.Value.TrimEnd());
                    filters.Add(item); //Add to filter parse
                }
                else
                {
                    mfltCheckedItems.Remove(item);
                    switch (item.FieldName)
                    {
                        case "StationName":
                            fltSelectedStations.Remove(item.Value.TrimEnd());
                            break;
                        case "Priority":
                            fltSelectedPriority.Remove(item.Value.TrimEnd());
                            break;
                        case "Message":
                            fltSelectedMessage.Remove(item.Value.TrimEnd());
                            break;
                        case "GroupDescription":
                            fltSelectedGroupDescription.Remove(item.Value.TrimEnd());
                            break;
                    }
                    //selectedStations.Remove(item.Value.TrimEnd());
                    filters.Remove(item); //Remove from filter parse
                }
                UpdateFilterParseTxt();
            }
        }
        private void UpdateFilterParseTxt()
        {
            fltSelectedStationsView = string.Join(" | ", fltSelectedStations.ToArray());
            fltSelectedPriorityView = string.Join(" | ", fltSelectedPriority.ToArray());
            fltSelectedGroupDescriptionView = string.Join(" | ", fltSelectedGroupDescription.ToArray());
            fltSelectedMessageView = string.Join(" | ", fltSelectedMessage.ToArray());

            //To Do
            List<string> filterTextList = new List<string>();
            if (fltSelectedStationsView.Count() > 0) filterTextList.Add("(" + fltSelectedStationsView + ")");
            if (fltSelectedPriorityView.Count() > 0) filterTextList.Add("(" + fltSelectedPriorityView + ")");
            if (fltSelectedGroupDescriptionView.Count() > 0) filterTextList.Add("(" + fltSelectedGroupDescriptionView + ")");
            if (fltSelectedMessageView.Count() > 0) filterTextList.Add("(" + fltSelectedMessageView + ")");

            if (filterTextList.Count() > 0)
            {
                filterText = string.Join(" & ", filterTextList.ToArray());
                Console.WriteLine(filterText);
            }
        }

        public RelayCommand RunFilterCmd { get; private set; }

        public bool canFilter()
        {
            return mfltCheckedItems.Count != 0;
        }
        public async void onFilterAlarms()
        {
            //Implement for each query Group by PropertyName : StationName , Priority or Desc.

            //ExpressGen();
            Console.WriteLine("Run Filter cmd");
            //CustAlarmViewModel = _custAlarmViewModel;

            IEnumerable<IGrouping<string, Item>> groupFields =
                    from item in filters
                    group item by item.FieldName;

            filterParseDeleg = FilterExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields);

            RestAlarmsRepo.filterParseDeleg = filterParseDeleg;
            await RestAlarmsRepo.GetCustAlarmAct();

            Console.WriteLine(filterParseDeleg.Body);

        }

        #endregion

        #region Search Helper function
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (Item item in e.OldItems)
                {
                    item.PropertyChanged -= Item_PropertyChanged;
                    mCheckedItems.Remove(item);
                }
            }
            if (e.NewItems != null)
            {
                foreach (Item item in e.NewItems)
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    if (item.IsChecked) mCheckedItems.Add(item);
                }
            }

            //UpdateFilterParseTxt();
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChecked")
            {
                Item item = (Item)sender;
                if (item.IsChecked)
                {
                    mCheckedItems.Add(item);
                    switch (item.FieldName)
                    {
                        case "StationName":
                            selectedStations.Add(item.Value.TrimEnd());
                            break;
                        case "FieldName":
                            selectedField.Add(item.Value.TrimEnd());
                            break;
                        case "Priority":
                            selectedGroupDescription.Add(item.Value.TrimEnd());
                            break;
                    }

                    //selectedStations.Add(item.Value.TrimEnd());
                    searchList.Add(item); //Add to filter parse
                }
                else
                {
                    mCheckedItems.Remove(item);
                    switch (item.FieldName)
                    {
                        case "StationName":
                            selectedStations.Remove(item.Value.TrimEnd());
                            break;
                        case "FieldName":
                            selectedField.Remove(item.Value.TrimEnd());
                            break;
                        case "Priority":
                            selectedGroupDescription.Remove(item.Value.TrimEnd());
                            break;
                    }
                    //selectedStations.Remove(item.Value.TrimEnd());
                    searchList.Remove(item); //Remove from filter parse
                }
                UpdateSeachParseTxt();
            }
        }
        private void UpdateSeachParseTxt()
        {
            selectedStationsView = string.Join(" | ", selectedStations.ToArray());
            selectedFieldView = string.Join(" | ", selectedField.ToArray());
            selectedPriorityView = string.Join(" | ", selectedGroupDescription.ToArray());

            //To Do
            List<string> filterTextList = new List<string>();
            if (selectedFieldView.Count() > 0) filterTextList.Add("Search " + searchText + "in field(s)" + "(" + selectedFieldView + ")");
            if (selectedStationsView.Count() > 0) filterTextList.Add(" @ station :" + "(" + selectedStationsView + ")");
            if (filterTextList.Count() > 0)
            {
                searchText = string.Join(" & ", filterTextList.ToArray());
                Console.WriteLine(searchText);
            }
        }

        public RelayCommand RunSearchCmd { get; private set; }

        public bool canSearch()
        {
            return (_search_Parse_Pri != "" || _search_Parse_Sec != "");
        }
        public async void onSearchAlarms()
        {
            //Implement for each query Group by PropertyName : StationName , Priority or Desc.

            //ExpressGen();
            //CustAlarmViewModel = _custAlarmViewModel;

            IEnumerable<IGrouping<string, Item>> groupFields =
                    from item in searchList
                    group item by item.FieldName;

            string[] search_Parse_Pri_List = search_Parse_Pri.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine(search_Parse_Pri_List.Length);

            //searchParseDeleg = SearchingExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields, search_Parse_Pri);
            searchParseDeleg = SearchingExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields, search_Parse_Pri_List, _search_Parse_Sec);

            if (searchParseDeleg == null)
            {
                Console.WriteLine("Expression Building Error");
            }
            else
            {
                RestAlarmsRepo.custPageIndex = 1;
                RestAlarmsRepo.filterParseDeleg = searchParseDeleg;
                await RestAlarmsRepo.GetCustAlarmAct();
                Console.WriteLine(searchParseDeleg.Body);
            }

        }
        #endregion

        private PropertyChangeEventBase _CustAlarmViewModel;
        public PropertyChangeEventBase CustAlarmViewModel
        {
            get { return _CustAlarmViewModel; }
            set {
                SetProperty(ref _CustAlarmViewModel, value);
            }
        }

       
        public void onCustView()
        {
            //ExpressGen();
            Console.WriteLine("Run MainView cmd");
            CustAlarmViewModel = null;
            //RestorationAlarmLists.RestAlarmsListViewModel.LoadRestorationAlarmsAsync();

        }


        public bool canViewMain()
        {
            return true;
        }

    }
}
