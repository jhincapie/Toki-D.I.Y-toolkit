using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LokiUtil;
using System.ComponentModel;

namespace LokiApp
{

  public class Step : INotifyPropertyChanged
  {
    private BodyPart part;
    private LokiEventType gesture;

    public BodyPart Part
    {
      get { return part; }
      set
      {
        part = value;
        OnPropertyChanged("Part");
      }
    }

    public LokiEventType Gesture
    {
      get { return gesture; }
      set
      {
        gesture = value;
        OnPropertyChanged("Gesture");
      }
    }

    public Step()
    { }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }
  }
}
