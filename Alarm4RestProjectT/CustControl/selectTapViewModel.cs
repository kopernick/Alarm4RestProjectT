using Alarm4Rest.Data;
using Alarm4Rest_Viewer.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Alarm4Rest_Viewer.CustControl
{
    class selectTapViewModel : PropertyChangeEventBase
    {

        #region Properties

        public static List<string> qSelectedGroupDescription = new List<string>();
        public static List<string> fltSelectedPriority = new List<string>();

        public Expression<Func<RestorationAlarmList, bool>> queryParseDeleg;

        public static List<Item> qFilters = new List<Item>();

        private HashSet<Item> mQCheckedItems;

        private Item _CatDesc87X;
        private Item _CatDesc56N;

        public Item CatDesc87X { get { return _CatDesc87X; } }
        public Item CatDesc56N { get { return _CatDesc56N; } }

        #endregion

        public selectTapViewModel()
        {
            isChecked87X = true;
            isChecked56N = false;

            _CatDesc87X = new Item("Group87X", "87X", "GroupDescription");
            if(isChecked87X) qFilters.Add(_CatDesc87X);

            _CatDesc56N = new Item("Group56N", "56N", "GroupDescription");
            if (isChecked56N) qFilters.Add(_CatDesc56N);

            RunUserQueryCmd = new RelayCommand(o => onUserQuery(), o => canUserQuery());
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

            if((string)Category == _CatDesc87X.Name)
            {
                AddRemoveProcess(ref _CatDesc87X, ref isChecked87X);
            }
            if ((string)Category == _CatDesc56N.Name)
            {
                AddRemoveProcess(ref _CatDesc56N, ref isChecked56N);
            }else
            {

            }

        }
        private void AddRemoveProcess(ref Item _CatDescX, ref bool isChecked)
        {
            Item CatTemp = _CatDescX;

            var qf = qFilters
                    .Where(i => i.Value == CatTemp.Value && i.FieldName == CatTemp.FieldName).ToList();

            if (isChecked)
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

        /* WPF call method with 1 parameter*/
        private bool isChecked87X;
        public bool IsChecked87X
        {
            get { return isChecked87X; }
            set
            {
                isChecked87X = value;
                OnPropertyChanged("IsChecked87X");
            }
        }

        /* WPF call method with 1 parameter*/
        private bool isChecked56N;
        public bool IsChecked56N
        {
            get { return isChecked56N; }
            set
            {
                isChecked56N = value;
                OnPropertyChanged("IsChecked56N");
            }
        }
        
        public RelayCommand RunUserQueryCmd { get; private set; }

        public bool canUserQuery()
        {
            return qFilters.Count != 0;
        }
        public async void onUserQuery()
        {
            //Implement for each query Group by PropertyName : StationName , Priority or Desc.
            //ExpressGen();
            Console.WriteLine("Run Standard Query cmd");
            //CustAlarmViewModel = _custAlarmViewModel;

            IEnumerable<IGrouping<string, Item>> groupFields =
                    from item in qFilters
                    group item by item.FieldName;

            queryParseDeleg = FilterExpressionBuilder.GetExpression<RestorationAlarmList>(groupFields);

            RestAlarmsRepo.filterParseDeleg = queryParseDeleg;
            RestAlarmsRepo.qDateTimeCondEnd = DateTime.Now;
            await RestAlarmsRepo.TGetQueryAlarmAct();

            Console.WriteLine(queryParseDeleg.Body);

        }

    }


    }