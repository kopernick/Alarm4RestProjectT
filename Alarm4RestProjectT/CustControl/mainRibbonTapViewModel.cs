using System;
using Alarm4Rest_Viewer.Services;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq.Expressions;
using Alarm4Rest.Data;
using System.Linq;

namespace Alarm4Rest_Viewer.CustControl
{
    class mainRibbonTapViewModel : PropertyChangeEventBase
    {

        public static List<SortItem> sortOrderList = new List<SortItem>();
        //public Expression<Func<RestorationAlarmList, object>> orderParseDeleg;
        public SortItem orderParseDeleg;

        public mainRibbonTapViewModel()
        {
            InitSortOrderTemplate();

            RunStdSortQuery1 = new RelayCommand(o => onRunStdSortQuery1(), o => canRunStdSortQuery());
            RunStdSortQuery2 = new RelayCommand(o => onRunStdSortQuery2(), o => canRunStdSortQuery());
            RunStdSortQuery3 = new RelayCommand(o => onRunStdSortQuery3(), o => canRunStdSortQuery());
        }

    #region Helper Function
        private void InitSortOrderTemplate()
        {
            sortOrderList.Add(new SortItem(1,"StationName", "DateTime", "Priority"));
            //sortOrderList.Add(new SortItem(1, "", "", ""));
            sortOrderList.Add(new SortItem(2,"StationName", "Priority", "DateTime"));
            sortOrderList.Add(new SortItem(3,"DateTime", "StationName", "Priority"));
        }

    /* WPF call method with parameter*/
    RelayCommand _RunFilterTimeCondCmd;
        public ICommand RunFilterTimeCondCmd
        {
            get
            {
                if (_RunFilterTimeCondCmd == null)
                {
                    _RunFilterTimeCondCmd = new RelayCommand(p => RunFilterTimeCond(p),
                        p => true);
                }
                return _RunFilterTimeCondCmd;
            }
        }

        // WPF Call with parameter
        private async void RunFilterTimeCond(object value)
        {
            TimeCondItem DateTimeCond = (TimeCondItem)value;
            
            DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.TGetCustAlarmAct(exclusiveEnd, DateTimeCond);

            //Console.WriteLine(filterParseDeleg.Body);
        }

        public RelayCommand RunStdSortQuery1 { get; private set; }

        public bool canRunStdSortQuery()
        {
            // return (RestAlarmsRepo.CustAlarmListDump.Count != 0);
            return true;
        }
        public async void onRunStdSortQuery1()
        {

            SortItem sortOrder = sortOrderList.First(i => i.ID == 1);
            orderParseDeleg = sortOrder;
            //orderParseDeleg = SortExpression.BuildOrderBys<RestorationAlarmList>(sortOrder);

            //DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.SGetCustAlarmAct(sortOrder);

            Console.WriteLine(sortOrder.ID);
        }

        public RelayCommand RunStdSortQuery2 { get; private set; }


        public async void onRunStdSortQuery2()
        {

            SortItem sortOrder = sortOrderList.First(i => i.ID == 2);
            orderParseDeleg = sortOrder;
            //orderParseDeleg = SortExpression.BuildOrderBys<RestorationAlarmList>(sortOrder);

            //DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.SGetCustAlarmAct(sortOrder);

            Console.WriteLine(sortOrder.ID);
        }

        public RelayCommand RunStdSortQuery3 { get; private set; }

        public async void onRunStdSortQuery3()
        {

            SortItem sortOrder = sortOrderList.First(i => i.ID == 3);
            orderParseDeleg = sortOrder;
            //orderParseDeleg = SortExpression.BuildOrderBys<RestorationAlarmList>(sortOrder);

            //DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.SGetCustAlarmAct(sortOrder);

            Console.WriteLine(sortOrder.ID);
        }
        #endregion
    }
}