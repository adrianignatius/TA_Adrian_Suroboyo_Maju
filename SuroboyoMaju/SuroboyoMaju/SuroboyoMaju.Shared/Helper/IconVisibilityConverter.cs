using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SuroboyoMaju.Shared.Helper
{
    class IconVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int id = (int)value;
            if (id == 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
