using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace LokiApp
{
  class GameStateVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      LokiApp.MainWindow.GameState state = (MainWindow.GameState)value;
      String tStates = (string)parameter;
      String[] states = tStates.Split(new Char[] { ',' });
      var statesOK = states.Select<String, LokiApp.MainWindow.GameState>(
        result => (MainWindow.GameState)Enum.Parse(typeof(MainWindow.GameState), result)
        );

      if (statesOK.Contains(state))
        return System.Windows.Visibility.Visible;
      return System.Windows.Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return null;
    }
  }
}
