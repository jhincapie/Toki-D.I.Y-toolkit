using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LokiApp
{
  class GestureVisibilityConverter : IValueConverter
  {
    object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return System.Windows.Visibility.Hidden;

      LokiUtil.LokiEventType eventT = (LokiUtil.LokiEventType)value;
      LokiUtil.LokiEventType param = (LokiUtil.LokiEventType)Enum.Parse(typeof(LokiUtil.LokiEventType), (string)parameter);
      if (eventT == param)
        return System.Windows.Visibility.Visible;
      return System.Windows.Visibility.Hidden;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }
  }
}
