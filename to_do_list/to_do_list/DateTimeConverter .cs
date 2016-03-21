using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization.DateTimeFormatting;
using Windows.UI.Xaml.Data;

namespace To_Do_List_2
{
    /// <summary>
    /// DaTeTime to string converter, value is binded in xaml MainPage
    /// </summary>
    public sealed class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime)
            {
                //DateTimeFormatter dateFormatter = new DateTimeFormatter("day month year"); - doesn't work in this order (day month year)
                DateTime dateTime = DateTime.Now;
                if (true == DateTime.TryParse(value.ToString(), out dateTime))
                {
                    return ((DateTime)value).ToString("d. M. yyyy");
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }
    }
}
