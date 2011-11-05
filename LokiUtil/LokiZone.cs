using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace LokiUtil
{

  public enum LokiZoneState { Clear, Pressed };

  public class LokiZone
  {
    public String Name { get; set; }
    public LokiZoneState State { get; set; }
    public bool RecentTap { get; set; }
    public DateTime PressStartTime { get; set; }
    public DateTime LastTapTime { get; set; }
    public double LastZoneSum { get; set; }

    //calibration data
    public double Avg { get; set; }
    public double Stdev { get; set; }
    public double Min { get { return Avg - Stdev; } }
    public double Max { get { return Avg + 3 * Stdev; } }

    //zone map
    public byte[] Map { get; set; }
    public CircularList<double> FramesWindow { get; set; }

    //tag time
    public bool IsTimerRunning { get; set; }
    private Timer Timer { get; set; }
    private LokiManager Manager { get; set; }

    public LokiZone(string name, LokiManager manager, byte[] zoneMap)
    {
      Name = name;
      RecentTap = false;
      State = LokiZoneState.Clear;
      Manager = manager;

      Map = zoneMap;
      FramesWindow = new CircularList<double>(manager.FramesInWindow);
      manager.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(manager_PropertyChanged);

      IsTimerRunning = false;
      Timer = new Timer();
      Timer.Elapsed += OnTimerEvent;
      Timer.Interval = manager.TapReleaseTime;
    }

    void manager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName != "FramesInWindow")
        return;
      if (FramesWindow.Length == Manager.FramesInWindow)
        return;

      lock (this)
      {
        CircularList<double> old = FramesWindow;
        FramesWindow = new CircularList<double>(Manager.FramesInWindow);
        foreach (double oldValue in old)
        {
          FramesWindow.Value = oldValue;
          FramesWindow.Next();
        }
      }
    }

    public override string ToString()
    {
      return Name;
    }

    public void StartTapTimer()
    {
      Timer.Start();
    }

    public void StopTapTimer()
    {
      Timer.Stop();
      IsTimerRunning = false;
    }

    private void OnTimerEvent(Object source, ElapsedEventArgs e)
    {
      StopTapTimer();
      Manager.DispatchEvent(this, LokiEventType.Tap);
    }

  }
}
