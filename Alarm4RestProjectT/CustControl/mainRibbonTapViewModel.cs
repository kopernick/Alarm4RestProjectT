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

            //RunStdSortQuery1 = new RelayCommand(o => onRunStdSortQuery1(), o => canRunStdSortQuery());
        }

    #region Helper Function
        private void InitSortOrderTemplate()
        {
            sortOrderList.Add(new SortItem(1,"StationName", "DateTime", "Priority"));
            //sortOrderList.Add(new SortItem(1, "", "", ""));
            sortOrderList.Add(new SortItem(2,"StationName", "Priority", "DateTime"));
            sortOrderList.Add(new SortItem(3,"DateTime", "StationName", "Priority"));
        }

    /* WPF call method with 2 parameter*/
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

        // WPF Call with 2 parameter
        private async void RunFilterTimeCond(object value)
        {
            TimeCondItem DateTimeCond = (TimeCondItem)value;
            
            DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.TGetCustAlarmAct(exclusiveEnd, DateTimeCond);

            //Console.WriteLine(filterParseDeleg.Body);
        }

        /* WPF call method with 1 parameter*/

        RelayCommand _RunStdSortQuery;
        public ICommand RunStdSortQuery
        {
            get
            {
                if (_RunStdSortQuery == null)
                {
                    _RunStdSortQuery = new RelayCommand(p => RunStdSort(p),
                        p => true);
                }
                return _RunStdSortQuery;
            }
        }

        private async void RunStdSort(object txtSortTemplate)
        {
            int sortTemplate = Convert.ToInt32(txtSortTemplate);
            SortItem sortOrder = sortOrderList.First(i => i.ID == sortTemplate);
            orderParseDeleg = sortOrder;
            //orderParseDeleg = SortExpression.BuildOrderBys<RestorationAlarmList>(sortOrder);

            //DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.SGetCustAlarmAct(sortOrder);

            Console.WriteLine(sortOrder.ID);
        }
       
        #endregion
    }
}