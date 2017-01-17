using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Alarm4Rest.Data;
using System.Windows.Threading;
using System.Linq.Expressions;
using System.Threading;

namespace Alarm4Rest_Viewer.Services
{
     public static class RestAlarmsRepo //Ultilities Class
    {
        public static Alarm4RestorationContext DBContext;
        public static List<RestorationAlarmList> RestAlarmListDump { get; private set; }
        public static List<RestorationAlarmList> CustAlarmListDump { get; private set; }
        public static List<string> StationsName { get; private set; }

        //private static DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private static DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        //private static Thread thrChkNewEvents;
        public static int MainPage { get; private set; }
        public static int LastAlarmRecIndex { get; set; }
        public static int LastMaxAlarmRecIndex { get; set; }
        public static int LastCustomAlarmRecIndex { get; set; }
        public static int PreviousAlarmRecIndex { get; private set; }
        public static int startNewRestItemArray { get; private set; }
        public static int startNewCustItemArray { get; private set; }
       
        public static RestorationAlarmList maxPkRecIndex { get; private set; }
        public static int pageIndex { get; set; }
        public static int custPageIndex { get; set; }
        public static int pageSize { get; set; }

        public static int pageCount { get; set; }
        public static int custPageCount { get; set; }
        public static int restAlarmCount { get; private set; }
        public static int custAlarmCount { get; private set; }
        public static List<string> Priority { get; private set; }
        public static List<string> GroupDescription { get; private set; }

        //Global Filter Keyword
        public static Expression<Func<RestorationAlarmList, bool>> filterParseDeleg;

        //public static List<string> filter_Parse { get; set; }


        private static bool isFilterStation(RestorationAlarmList alarm)
        {
            return alarm.StationName.Contains("BK");
        }

        private static bool isFilterPriority(RestorationAlarmList alarm)
        {
            return alarm.Priority.Contains("High");
        }

        public static void InitializeRepository()
        {
            InitializeComponent();

            DBContext = new Alarm4RestorationContext();
            RestAlarmListDump = new List<RestorationAlarmList>();
            CustAlarmListDump = new List<RestorationAlarmList>();
            StationsName = new List<string>();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 20);
            dispatcherTimer.Start();
        }

        //Get All Data from SQL
        public static async Task GetInitDataRepositoryAsync()
        {

            RestAlarmListDump = await GetRestAlarmsAsync(pageIndex, pageSize);
            LastAlarmRecIndex = RestAlarmListDump[0].PkAlarmListID; //Set Last PkAlarmList initializing
            LastMaxAlarmRecIndex = LastAlarmRecIndex;
            StationsName = await GetStationNameAsync();
            Priority = await GetPriorityAsync();
            GroupDescription = await GetGroupAsync();
        }

        private static void InitializeComponent()
        {
            MainPage = 1;
            LastAlarmRecIndex = -1;
            LastMaxAlarmRecIndex = -1;
            LastCustomAlarmRecIndex = -1;
            PreviousAlarmRecIndex = 0;
            maxPkRecIndex = null;
            pageIndex = 1;
            pageSize = 40;
        }

        public static Task<List<RestorationAlarmList>> GetRestAlarmsAsync()
        {
            return DBContext.RestorationAlarmLists.ToListAsync<RestorationAlarmList>();
        }

        public static async Task<List<string>> GetStationNameAsync()
        {
            return await DBContext.Stations
                .Select(x=>x.StationName)
                .ToListAsync<string>();
        }

        public static async Task<List<string>> GetPriorityAsync()
        {
            return await DBContext.RestorationAlarmLists
                .Select(x => x.Priority)
                .Distinct()
                .ToListAsync<string>();
        }

        public static async Task<List<string>> GetGroupAsync()
        {
            return await DBContext.RestorationAlarmLists
                .Select(x => x.GroupDescription)
                .Distinct()
                .ToListAsync<string>();
        }


        public static async Task<List<RestorationAlarmList>> GetRestAlarmsAsync(int pageIndex = 1, int pageSize = 30)
        {
            //To Do ปรับปรุงให้มีการ Query ครั้งเดียว
            //Get RestAlarm count
            restAlarmCount = (from alarm in DBContext.RestorationAlarmLists
                              orderby alarm.PkAlarmListID descending
                              select alarm).Count();

            if (restAlarmCount % pageSize == 0)
            {
                pageCount = (restAlarmCount / pageSize);
            }else
            { 
                pageCount = (restAlarmCount / pageSize) + 1;
            }
            
            //Get one page
            return await DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            //.Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }

        public static async Task<List<RestorationAlarmList>> GetCustomRestAlarmsAsync(Expression<Func<RestorationAlarmList, bool>> filter_Parse, int pageIndex = 1, int pageSize = 30)
        {

            //To Do ปรับปรุงให้มีการ Query ครั้งเดียว
            //Get RestAlarm count
            custAlarmCount = DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Where(filter_Parse)
                            .Count();

            if (custAlarmCount % pageSize == 0)
            {
                custPageCount = (custAlarmCount / pageSize);
            }
            else
            {
                custPageCount = (custAlarmCount / pageSize) + 1;
            }

            //Get one page
            return await DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Where(filter_Parse)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }

        public static async void FilterAct()
        {
            RestEventArgs arg = new RestEventArgs();
            //Test Raise read "LoadStationName"
            CustAlarmListDump = await GetCustomRestAlarmsAsync(filterParseDeleg, custPageIndex, pageSize);
            if (CustAlarmListDump.Count != 0)
            {
                LastCustomAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
                startNewCustItemArray = CustAlarmListDump.Count - 1;
                arg.message = "filterAlarmCust";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else //filter result no item
            {

                arg.message = "filterAlarmCustNoResult";
                LastCustomAlarmRecIndex = -1;
                startNewCustItemArray = -1;
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        public static async void GetRestAlarmAct()
        {
            RestEventArgs arg = new RestEventArgs();
            //Test Raise read "LoadStationName"
            RestAlarmListDump = await GetRestAlarmsAsync(pageIndex, pageSize);
            if (RestAlarmListDump.Count != 0)
            {
                LastAlarmRecIndex = RestAlarmListDump[0].PkAlarmListID;
                startNewRestItemArray = RestAlarmListDump.Count - 1;
                arg.message = "GetRestAlarm";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event

            }
            else //filter result no item
            {
                arg.message = "GetRestAlarmNoResult";
                LastAlarmRecIndex = -1;
                LastMaxAlarmRecIndex = -1;
                startNewRestItemArray = -1;
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }
        private static async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Predicate<int> isEqualLast = (newIndex) => LastMaxAlarmRecIndex == newIndex;
            try
            {

                maxPkRecIndex = (from alarm in DBContext.RestorationAlarmLists
                                 orderby alarm.PkAlarmListID descending
                                 select alarm).FirstOrDefault();

                if (maxPkRecIndex == null || isEqualLast(maxPkRecIndex.PkAlarmListID)) return; //Exit if Has no data or No new Alarm

                //Get Update Data
                RestAlarmListDump = await GetRestAlarmsAsync(pageIndex, pageSize);
                CheckNewRestAlarm();

                if (filterParseDeleg != null)
                {
                    CustAlarmListDump = await GetCustomRestAlarmsAsync(filterParseDeleg, custPageIndex, pageSize);
                    if (CustAlarmListDump.Count != 0)
                        CheckNewCustomRestAlarm();
                }
            }catch
            {
                Console.WriteLine("Load Fail");
            }
           
        }

        private static void CheckNewRestAlarm()
        {
            RestEventArgs arg = new RestEventArgs();

            //Func<int, int, bool> hasNewAlarm = hasNewAalrmChk;

            //Test using Predicate
            Predicate<int> hasNewAlarmChk = (newLastIndex) => newLastIndex > LastMaxAlarmRecIndex;

            if (hasNewAlarmChk(maxPkRecIndex.PkAlarmListID))
            {
                //To Do LastAlarmRecIndex_of_Page

                LastMaxAlarmRecIndex = maxPkRecIndex.PkAlarmListID ;
                PreviousAlarmRecIndex = LastAlarmRecIndex;
                //LastAlarmRecIndex = maxPkRecIndex.PkAlarmListID;
                LastAlarmRecIndex = RestAlarmListDump[0].PkAlarmListID;
                startNewRestItemArray = LastAlarmRecIndex-PreviousAlarmRecIndex-1;
                arg.message = "hasNewAlarm";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else //Database has been reset
            {
                //Restart Process
                PreviousAlarmRecIndex = 0;
                LastAlarmRecIndex = maxPkRecIndex.PkAlarmListID;
                startNewRestItemArray = RestAlarmListDump.Count-1;
                
                arg.message = "Reset";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        private static void CheckNewCustomRestAlarm()
        {
            RestEventArgs arg = new RestEventArgs();

            //Func<int, int, bool> hasNewAlarm = hasNewAalrmChk;

            //Test using Predicate
            Predicate<List<RestorationAlarmList>> hasAllNew = (alarm) => alarm[alarm.Count-1].PkAlarmListID > LastAlarmRecIndex || LastAlarmRecIndex > alarm[0].PkAlarmListID;
            Predicate<int> isEqualLastCust = (Index) => Index == LastCustomAlarmRecIndex;

            if (isEqualLastCust(CustAlarmListDump[0].PkAlarmListID)) return; //No new custom filter alarm

            if (hasAllNew(CustAlarmListDump)) //All New alarm or DB has been reset
            {
                LastCustomAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
                startNewCustItemArray = CustAlarmListDump.Count - 1;
                arg.message = "hasAllNewAlarmCust";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else
            {
                //Finding Start last RecIndex Position
                int i = 0;
                foreach (RestorationAlarmList alarm in CustAlarmListDump)
                {
                    //Get Starting Position
                    if (isEqualLastCust(alarm.PkAlarmListID))
                    {
                        startNewCustItemArray = i-1;
                        break;
                    }
                    i++;
                }

                LastCustomAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
                arg.message = "hasNewAlarmCust";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        //private static Expression<Func<RestorationAlarmList, bool>> FilterAppeding(string[] filterStr, DateTime startDate)
        //{
        //    var rule = PredicateExtensions.PredicateExtensions.Begin<RestorationAlarmList>();
        //    foreach (var filterWord in filterStr)
        //    {
        //        rule = rule.Or(FilterAppeding(filterWord, startDate));
        //    }
        //    return rule;
        //}

        public static event EventHandler<RestEventArgs> RestAlarmChanged;
        private static void onRestAlarmChanged(RestEventArgs arg)
        {
            if (RestAlarmChanged != null)
                RestAlarmChanged(null, arg);
        }
    }

}
