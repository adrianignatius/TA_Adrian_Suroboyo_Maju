using System;
using Windows.UI.Xaml.Data;

namespace SuroboyoMaju.Shared.Helper
{
    class LongDateTimeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                string date = (string)value;
                DateTime dateTime = DateTime.Parse(date);
                string tanggal = dateTime.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
                string waktu = dateTime.ToString("H:mm");
                string formatDate = tanggal + " Pukul " + waktu;
                return formatDate;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
