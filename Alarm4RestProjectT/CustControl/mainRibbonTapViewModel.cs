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

        public mainRibbonTapViewModel()
        {
            //To Do ต้องประการที่เดียว
            InitSortOrderTemplate();
            RestAlarmsRepo.orderParseDeleg = sortOrderList.First(i => i.ID == 1);

            //RunStdSortQuery1 = new RelayCommand(o => onRunStdSortQuery1(), o => canRunStdSortQuery());
        }

    #region Helper Function
        private void InitSortOrderTemplate()
        {
            sortOrderList.Add(new SortItem(1, "StationName", "DateTime", "Priority"));
            sortOrderList.Add(new SortItem(2, "StationName", "Priority", "DateTime"));
            sortOrderList.Add(new SortItem(3, "DateTime", "StationName", "Priority"));
        }

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
            //orderParseDeleg = SortExpression.BuildOrderBys<RestorationAlarmList>(sortOrder);

            RestAlarmsRepo.qDateTimeCondEnd = DateTime.Now;

            //await RestAlarmsRepo.GetQueryAlarmAct();
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
                    _RunQueryTimeCondCmd = new RelayCommand(p => RunQueryTimeCond(p), p => true);
                }
                return _RunQueryTimeCondCmd;
            }
        }

        // WPF Call with 2 parameter
        private async void RunQueryTimeCond(object value)
        {

            RestAlarmsRepo.qDateTimeCondItem = (TimeCondItem)value;
            RestAlarmsRepo.qDateTimeCondEnd = DateTime.Now;
            await RestAlarmsRepo.TGetQueryAlarmAct();

            //Console.WriteLine(filterParseDeleg.Body);
        }

        #endregion
    }
}