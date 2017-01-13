using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alarm4Rest.Data;
using System.Linq.Expressions;
using Alarm4Rest_Viewer.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Alarm4Rest_Viewer.RestorationAlarmLists;

namespace Alarm4Rest_Viewer.CustControl
{
    class searchToolBarViewModel : PropertyChangeEventBase
    {
        public static List<string> selectedStations = new List<string>();
        public static List<string> selectedField = new List<string>();
        public static List<string> selectedGroupDescription = new List<string>();
        public Expression<Func<RestorationAlarmList, bool>> searchParseDeleg;

        public static List<Item> filters = new List<Item>();

        private HashSet<Item> mCheckedItems;

        private ObservableCollection<Item> mpfieldItems;
        public IEnumerable<Item> fieldItems { get { return mpfieldItems; } }


        private ObservableCollection<Item> mgroupDescItems;
        public IEnumerable<Item> groupDescItems { get { return mgroupDescItems; } }


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
        public string selectedGroupDescriptionView
        {
            get { return _selectedGroupDescriptionView; }
            set
            {
                Set(ref _selectedGroupDescriptionView, value);
                OnPropertyChanged("selectedGroupDescriptionView");
            }
        }

        public string _selectedStationsView;
        public string _selectedFieldView;
        public string _selectedGroupDescriptionView;

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

        public searchToolBarViewModel()
        {
            mstationItems = new ObservableCollection<Item>();
            mpfieldItems = new ObservableCollection<Item>();
            mgroupDescItems = new ObservableCollection<Item>();
            mCheckedItems = new HashSet<Item>();

            //Subscribe to RestAlarmChanged of the RestAlarmsListViewModel
            RestAlarmsListViewModel.RestAlarmChanged += OnRestAlarmChanged;

            mstationItems.CollectionChanged += Items_CollectionChanged;
            mpfieldItems.CollectionChanged += Items_CollectionChanged;
            mgroupDescItems.CollectionChanged += Items_CollectionChanged;

            RunSearchCmd = new RelayCommand(o => onSearchAlarms(), o => canSearch());
        }

        //Auto Get Station Name when DB has been loaded.
        private void OnRestAlarmChanged(object source, RestEventArgs arg)
        {
            if (arg.message == "hasLoaded")
            {

                // Adding Station ComboBox items
                foreach (var Station in RestAlarmsRepo.StationsName)
                {
                    mstationItems.Add(new Item(Station.ToString(), "StationName"));
                }

                // Adding Priority ComboBox items
                
                mpfieldItems.Add(new Item("PointName", "FieldName"));
                mpfieldItems.Add(new Item("Message", "FieldName"));
                mpfieldItems.Add(new Item("GroupPointName", "FieldName"));
                mpfieldItems.Add(new Item("GroupDescription", "FieldName"));

                // Adding GoupDescription ComboBox items
                foreach (var GroupDescription in RestAlarmsRepo.GroupDescription)
                {
                    mgroupDescItems.Add(new Item(GroupDescription.ToString(), "GroupDescription"));
                }
            }
        }

        //Manual Get Station Name
        public void LoadFilterParameter()
        {

            //Stations = new ObservableCollection<string>(
            //                await RestAlarmsRepo.GetStationNameAsync());

            // Adding Station ComboBox items
            mstationItems.Add(new Item("All", "StationName"));
            foreach (var Station in RestAlarmsRepo.StationsName)
            {
                mstationItems.Add(new Item(Station.ToString(), "StationName"));
            }

            // Adding Priority ComboBox items
            mpfieldItems.Add(new Item("All", "FieldName"));
            foreach (var Field in RestAlarmsRepo.Priority)
            {
                mpfieldItems.Add(new Item(Field.ToString(), "FieldName"));
            }

            // Adding GoupDescription ComboBox items
            mgroupDescItems.Add(new Item("All", "GroupDescription"));
            foreach (var GroupDescription in RestAlarmsRepo.GroupDescription)
            {
                mgroupDescItems.Add(new Item(GroupDescription.ToString(), "GroupDescription"));
            }

        }

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

            UpdateFilterParseTxt();
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
                        case "GroupDescription":
                            selectedGroupDescription.Add(item.Value.TrimEnd());
                            break;
                    }

                    //selectedStations.Add(item.Value.TrimEnd());
                    filters.Add(item); //Add to filter parse
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
                        case "GroupDescription":
                            selectedGroupDescription.Remove(item.Value.TrimEnd());
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
            selectedStationsView = string.Join(" | ", selectedStations.ToArray());
            selectedFieldView = string.Join(" | ", selectedField.ToArray());
            selectedGroupDescriptionView = string.Join(" | ", selectedGroupDescription.ToArray());

            //To Do
            List<string> filterTextList = new List<string>();
            if (selectedStationsView.Count() > 0) filterTextList.Add("(" + selectedStationsView + ")");
            if (selectedFieldView.Count() > 0) filterTextList.Add("(" + selectedFieldView + ")");
            if (selectedGroupDescriptionView.Count() > 0) filterTextList.Add("(" + selectedGroupDescriptionView + ")");

            if (filterTextList.Count() > 0)
            {
                searchText = string.Join(" & ", filterTextList.ToArray());
                Console.WriteLine(searchText);
            }
        }

        private string _search_Parse;
        public string search_Parse
        {
            get { return _search_Parse; }
            set
            {
                _search_Parse = value;
                OnPropertyChanged("search_Parse");
                // RestAlarmsRepo.filter_Parse = value;
            }
        }
        public RelayCommand RunSearchCmd { get; private set; }

        public bool canSearch()
        {
            return mCheckedItems.Count != 0;
        }
        public void onSearchAlarms()
        {
            //Implement for each query Group by PropertyName : StationName , Priority or Desc.

            //ExpressGen();

            IEnumerable<IGrouping<string, Item>> groupFields =
                    from item in filters
                    group item by item.FieldName;

            searchParseDeleg = SearchingExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields, search_Parse);

            RestAlarmsRepo.filterParseDeleg = searchParseDeleg;
            RestAlarmsRepo.FilterAct();

            Console.WriteLine(searchParseDeleg.Body);

        }
    }
}
