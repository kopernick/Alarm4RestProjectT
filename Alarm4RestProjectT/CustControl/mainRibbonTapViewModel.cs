using System;
using Alarm4Rest_Viewer.Services;
using System.Windows.Input;

namespace Alarm4Rest_Viewer.CustControl
{
    class mainRibbonTapViewModel : PropertyChangeEventBase
    {

        #region Helper Function

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

            /*
            IEnumerable<IGrouping<string, Item>> groupFields =
                    from item in filters
                    group item by item.FieldName;

            filterParseDeleg = FilterExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields);
            */

            //RestAlarmsRepo.filterParseDeleg = filterParseDeleg;
            //RestAlarmsRepo.filterParseDeleg = searchParseDeleg;
            DateTime exclusiveEnd = DateTime.Now;
            await RestAlarmsRepo.TGetCustAlarmAct(exclusiveEnd, DateTimeCond);

            //Console.WriteLine(filterParseDeleg.Body);
        }

        #endregion
    }
}

