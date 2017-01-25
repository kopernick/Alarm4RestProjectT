using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Alarm4Rest.Data;
using System.Windows.Threading;
using System.Linq.Expressions;
using System.Threading;
using System.Collections;
using System.ComponentModel;

namespace Alarm4Rest_Viewer.Services
{
     public static class RestAlarmsRepo //Ultilities Class
    {
        public static Alarm4RestorationContext DBContext;
        public static List<RestorationAlarmList> RestAlarmListDump { get; private set; }
        public static List<RestorationAlarmList> CustAlarmListDump { get; private set; }
        public static List<RestorationAlarmList> QueryAlarmListDump { get; private set; }
        public static List<string> StationsName { get; private set; }

        //private static DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private static DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        //private static Thread thrChkNewEvents;
        public static int MainPage { get; private set; }
        public static int pageSize { get; set; }
        public static int LastAlarmRecIndex { get; set; }
        public static int LastMaxAlarmRecIndex { get; set; }

        public static int RestPageCount { get; set; }
        public static int RestPageIndex { get; set; }
        public static int PreviousAlarmRecIndex { get; private set; }
        public static int startNewRestItemArray { get; private set; }
        public static RestorationAlarmList maxPkRecIndex { get; private set; }
        public static int restAlarmCount { get; private set; }

        public static int startNewCustItemArray { get; private set; }
        public static int LastCustAlarmRecIndex { get; set; }
        public static int custPageIndex { get; set; }
        public static int custPageCount { get; set; }
        public static int custAlarmCount { get; private set; }

        public static int startNewQueryItemArray { get; private set; }
        public static int LastQueryAlarmRecIndex { get; set; }
        public static int queryPageIndex { get; set; }
        public static int queryPageCount { get; set; }
        public static int queryAlarmCount { get; private set; }


        public static TimeCondItem DateTimeCondItem { get; private set; }

        
        public static List<string> Priority { get; private set; }
        public static List<string> GroupDescription { get; private set; }
        public static List<string> Message { get; private set; }

        


        

        //Global Filter Keyword
        public static Expression<Func<RestorationAlarmList, bool>> filterParseDeleg;
        public static SortItem orderParseDeleg;

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
            QueryAlarmListDump = new List<RestorationAlarmList>();

            DateTimeCondItem = new TimeCondItem("Day", 2);
            filterParseDeleg = null;
            //StationsName = new List<string>();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 20);
            dispatcherTimer.Start();
        }

        //Get All Data from SQL
        public static async Task GetInitDataRepositoryAsync()
        {
            RestEventArgs arg = new RestEventArgs();

            try
            {
                RestAlarmListDump = await GetRestAlarmsAsync();
                LastAlarmRecIndex = RestAlarmListDump[0].PkAlarmListID; //Set Last PkAlarmList initializing
                LastMaxAlarmRecIndex = LastAlarmRecIndex;
                StationsName = await GetStationNameAsync();
                Priority = await GetPriorityAsync();
                GroupDescription = await GetGroupAsync();
                Message = await GetMessageAsync();

                //Send Message to Subscriber
                arg.message = "Start Success";
                onRestAlarmChanged(arg);//Raise Event
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
            }
            catch
            {
                //Send Message to Subscriber
                arg.message = "Start Fail";
                onRestAlarmChanged(arg);//Raise Event
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
            }
        }

        private static void InitializeComponent()
        {
            MainPage = 1;
            LastAlarmRecIndex = -1;
            LastMaxAlarmRecIndex = -1;
            LastCustAlarmRecIndex = -1;
            LastQueryAlarmRecIndex = -1;
            PreviousAlarmRecIndex = 0;
            maxPkRecIndex = null;
            RestPageIndex = 1;
            custPageIndex = 1;
            queryPageIndex = 1;
            pageSize = 35;
        }

        public static Task<List<RestorationAlarmList>> GetAllRestAlarmsAsync()
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

        public static async Task<List<string>> GetMessageAsync()
        {
            return await DBContext.RestorationAlarmLists
                .Select(x => x.Message)
                .Distinct()
                .ToListAsync<string>();
        }
        

        public static async Task<List<RestorationAlarmList>> GetRestAlarmsAsync()
        {
            //To Do ปรับปรุงให้มีการ Query ครั้งเดียว
            //Get RestAlarm count
            restAlarmCount = (from alarm in DBContext.RestorationAlarmLists
                              orderby alarm.PkAlarmListID descending
                              select alarm).Count();

            if (restAlarmCount % pageSize == 0)
            {
                RestPageCount = (restAlarmCount / pageSize);
            }else
            { 
                RestPageCount = (restAlarmCount / pageSize) + 1;
            }
            
            //Get one page
            return await DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Skip((RestPageIndex - 1) * pageSize)
                            .Take(pageSize)
                            //.Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }

        public static async Task<List<RestorationAlarmList>> GetRestAlarmsAsync(int pageIndex, int pageSize)
        {
            //To Do ปรับปรุงให้มีการ Query ครั้งเดียว
            //Get RestAlarm count
            restAlarmCount = (from alarm in DBContext.RestorationAlarmLists
                              orderby alarm.PkAlarmListID descending
                              select alarm).Count();

            if (restAlarmCount % pageSize == 0)
            {
                RestPageCount = (restAlarmCount / pageSize);
            }
            else
            {
                RestPageCount = (restAlarmCount / pageSize) + 1;
            }

            //Get one page
            return await DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            //.Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }

        public static async Task<List<RestorationAlarmList>> GetCustomRestAlarmsAsync()
        {

            //To Do ปรับปรุงให้มีการ Query ครั้งเดียว
            //Get RestAlarm count
            custAlarmCount = DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Where(filterParseDeleg)
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
                            //.OrderBy(c => c.Priority).ThenByDescending(c => c.PkAlarmListID)
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Where(filterParseDeleg)
                            .Skip((custPageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }
        public static async Task<List<RestorationAlarmList>> TGetCustomRestAlarmsAsync(DateTime exclusiveEnd, TimeCondItem timeCondition)
        {

            //exclusiveEnd = DateTime.ParseExact(exclusiveEnd.ToString(), "dd/MM/yyyy HH:mm:ss.000", System.Globalization.CultureInfo.InvariantCulture);
            DateTime inclusiveStart = new DateTime();
            TimeSpan ts = new TimeSpan(0, 0, 0);
            switch (timeCondition.TimeType)
            {
                case "Day":
                    inclusiveStart = exclusiveEnd.AddDays((-1) * (timeCondition.Value-1));
                    inclusiveStart = inclusiveStart.Date + ts; // Reset time to HH: mm: ss.000" -> 0000:00.000
                    break;
                case "Week":
                    int diff = exclusiveEnd.DayOfWeek - DayOfWeek.Monday;
                    if (diff < 0)
                    {
                        diff += 7;
                    }
                    inclusiveStart= exclusiveEnd.AddDays(-1 * diff).Date;
                    inclusiveStart = inclusiveStart.Date + ts; // Reset time to HH: mm: ss.000" -> 0000:00.000
                    break;
                case "Month":
                    inclusiveStart = exclusiveEnd.AddMonths((-1) * (timeCondition.Value - 1));
                    inclusiveStart = new DateTime(inclusiveStart.Year, inclusiveStart.Month, 1);
                    break;
                default: //All one year
                    inclusiveStart = exclusiveEnd.AddYears(-1);
                    inclusiveStart = new DateTime(inclusiveStart.Year, 1, 1);
                    break;

            }       

            //To Do ปรับปรุงให้มีการ Query ครั้งเดียว
            //Get CustRestAlarm count
            custAlarmCount = DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Where(filterParseDeleg)
                            .Where(c => c.DateTime >= inclusiveStart
                                        && c.DateTime < exclusiveEnd)
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
                            //.OrderBy(c => c.Priority).ThenByDescending(c => c.PkAlarmListID)
                            .OrderByDescending(c => c.PkAlarmListID)
                            .Where(filterParseDeleg)
                            .Where(c => c.DateTime >= inclusiveStart
                                        && c.DateTime < exclusiveEnd)
                            .Skip((custPageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }

        public static async Task<List<RestorationAlarmList>> GetQueryRestAlarmsAsync(SortItem orderParseDeleg)
        {

            string[] sortOreder = orderParseDeleg.ToArray();

            //Get one page
            IEnumerable<RestorationAlarmList> Query = await DBContext.RestorationAlarmLists
                            .OrderByDescending(c => c.PkAlarmListID)
                            //.Where(filterParseDeleg)
                            .ToListAsync();

            var resultList = Query.BuildOrderBy(
                                new SortDescription(sortOreder[0], ListSortDirection.Ascending),
                                new SortDescription(sortOreder[1], ListSortDirection.Ascending));

            queryAlarmCount = resultList.Count();

            if (queryAlarmCount % pageSize == 0)
            {
                queryPageCount = (queryAlarmCount / pageSize);
            }
            else
            {
                queryPageCount = (queryAlarmCount / pageSize) + 1;
            }
            return resultList
                            .Skip((queryPageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();
        }


        public static async Task<List<RestorationAlarmList>> GetCustomRestAlarmsAsync(Expression<Func<RestorationAlarmList, bool>> filter_Parse, int custPageIndex = 1, int pageSize = 30)
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
                            .Skip((custPageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync<RestorationAlarmList>();
        }

        public static async Task GetCustAlarmAct()
        {
            RestEventArgs arg = new RestEventArgs();
            //Test Raise read "LoadStationName"
            if (filterParseDeleg == null) return;
            CustAlarmListDump = await GetCustomRestAlarmsAsync();
            if (CustAlarmListDump.Count != 0)
            {
                LastCustAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
                startNewCustItemArray = CustAlarmListDump.Count - 1;
                arg.message = "GetFilterAlarmCust";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else //filter result no item
            {

                arg.message = "filterAlarmCustNoResult";
                LastCustAlarmRecIndex = -1;
                startNewCustItemArray = -1;
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        public static async Task TGetCustAlarmAct(DateTime exclusiveEnd, TimeCondItem DatetimeCond)
        {
            RestEventArgs arg = new RestEventArgs();

            DateTimeCondItem = DatetimeCond;
            //Test Raise read "LoadStationName"
            if (filterParseDeleg == null) return;
            CustAlarmListDump = await TGetCustomRestAlarmsAsync(exclusiveEnd, DateTimeCondItem);
            if (CustAlarmListDump.Count != 0)
            {
                LastCustAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
                startNewCustItemArray = CustAlarmListDump.Count - 1;
                arg.message = "filterAlarmCust";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else //filter result no item
            {

                arg.message = "filterAlarmCustNoResult";
                LastCustAlarmRecIndex = -1;
                startNewCustItemArray = -1;
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        public static async Task GetQueryAlarmAct(SortItem orderParseDeleg)
        {
            RestEventArgs arg = new RestEventArgs();

            //Test Raise read "LoadStationName"
            if (orderParseDeleg == null) return;
            QueryAlarmListDump = await GetQueryRestAlarmsAsync(orderParseDeleg);
            if (QueryAlarmListDump.Count != 0)
            {
                LastQueryAlarmRecIndex = QueryAlarmListDump[0].PkAlarmListID;
                startNewQueryItemArray = QueryAlarmListDump.Count - 1;
                arg.message = "QueryAlarmAct";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else //filter result no item
            {

                arg.message = "GetQueryAlarmNoResult";
                LastQueryAlarmRecIndex = -1;
                startNewQueryItemArray = -1;
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        public static async Task GetQueryAlarmAct()
        {
            RestEventArgs arg = new RestEventArgs();

            //Test Raise read "LoadStationName"
            if (orderParseDeleg == null) return;
            QueryAlarmListDump = await GetQueryRestAlarmsAsync(orderParseDeleg);
            if (QueryAlarmListDump.Count != 0)
            {
                LastQueryAlarmRecIndex = QueryAlarmListDump[0].PkAlarmListID;
                startNewQueryItemArray = QueryAlarmListDump.Count - 1;
                arg.message = "QueryAlarmAct";
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
            else //filter result no item
            {

                arg.message = "GetQueryAlarmNoResult";
                LastQueryAlarmRecIndex = -1;
                startNewQueryItemArray = -1;
                Console.WriteLine(DateTime.Now.ToString() + " : Raise Event " + arg.message);
                onRestAlarmChanged(arg);//Raise Event
            }
        }

        public static async Task GetRestAlarmAct()
        {
            RestEventArgs arg = new RestEventArgs();
            //Test Raise read "LoadStationName"
            RestAlarmListDump = await GetRestAlarmsAsync();
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
                RestAlarmListDump = await GetRestAlarmsAsync();
                CheckNewRestAlarm();

                if (filterParseDeleg != null)
                {
                    CustAlarmListDump = await GetCustomRestAlarmsAsync();
                    if (CustAlarmListDump.Count != 0)
                        CheckNewCustomRestAlarm();
                }
            }catch
            {
                Console.WriteLine("Timer Tick Load Fail");
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

                LastMaxAlarmRecIndex = maxPkRecIndex.PkAlarmListID;
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
                LastMaxAlarmRecIndex = 0;
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
            Predicate<int> isEqualLastCust = (Index) => Index == LastCustAlarmRecIndex;

            if (isEqualLastCust(CustAlarmListDump[0].PkAlarmListID)) return; //No new custom filter alarm

            if (hasAllNew(CustAlarmListDump)) //All New alarm or DB has been reset
            {
                LastCustAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
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

                LastCustAlarmRecIndex = CustAlarmListDump[0].PkAlarmListID;
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
