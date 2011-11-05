using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace LokiApp
{
  public class Level : INotifyPropertyChanged
  {

    public List<Step> Steps { get; set; }

    private int currentStepIndex = 1;
    public int CurrentStepIndex
    {
      get { return currentStepIndex; }
      set
      {
        currentStepIndex = value;
        OnPropertyChanged("CurrentStepIndex");
        OnPropertyChanged("CurrentStep");
      }
    }

    public Step CurrentStep
    {
      get
      {
        if (Steps.Count == 0)
          return null;
        return Steps[currentStepIndex - 1];
      }
    }

    public Level()
    {
      Steps = new List<Step>();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

  }

}
