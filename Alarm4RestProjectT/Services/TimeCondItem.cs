/* ********************************************
 * TimeCondItem |TimeType|Count|
 * 
 * Sample Item : |"Day"|2|, |Month|3|, |Day|5|, etc
 * 
 * ********************************************/
using System;
using System.Globalization;
using System.Windows.Data;

namespace Alarm4Rest_Viewer.Services
{
    public class TimeCondItem
    {
        public string TimeType { get; private set; }
        public int Value { get; private set; }

        public TimeCondItem(int value)
        {
            Value = value;
            TimeType = "Day";
        }
        public TimeCondItem(string timeType, int value)
        {
            Value = value;
            TimeType = timeType;
        }

        public override string ToString()
        {
            return TimeType;
        }
    }
    public class MultiValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // return String.Format("{0} {1}", values[0], values[1]);

                TimeCondItem data = new TimeCondItem(values[0].ToString(), System.Convert.ToInt32(values[1]));
                return data;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
