using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace LokiApp
{
  class BodyPartVisibilityConverter : IMultiValueConverter
  {

    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values[0] == DependencyProperty.UnsetValue)
        return Visibility.Hidden;

      BodyPart bodyP = (BodyPart)values[0];
      LokiApp.MainWindow.GameState state = (LokiApp.MainWindow.GameState)values[1];
      bool interactiveModeVisible = (bool)values[2];

      BodyPart param = (BodyPart)Enum.Parse(typeof(BodyPart), (string)parameter);
      if (bodyP != param && state != MainWindow.GameState.InteractiveMode)
        return System.Windows.Visibility.Hidden;
      else if (state == MainWindow.GameState.InteractiveMode && interactiveModeVisible)
        return System.Windows.Visibility.Visible;

      if (state == MainWindow.GameState.Playback)
        return System.Windows.Visibility.Visible;

      return System.Windows.Visibility.Hidden;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }
  }
}
