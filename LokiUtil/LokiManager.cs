using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.TouchMouseSensor;
using LokiUtil.Recording;
using System.ComponentModel;

namespace LokiUtil
{

  public enum LokiEventType
  {
    ZonePressed, ZoneReleased, Tap, DoubleTap, All
  };

  public struct LokiPair
  {
    public LokiZone Zone { get; set; }
    public LokiEventType Event { get; set; }
  }

  public class LokiManager : INotifyPropertyChanged
  {

    #region ZoneMaps

    private byte[] zonePlug1 = new byte[] 
    { 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug2 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug3 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug4 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug5 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug6 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug7 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
      0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug8 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 
      0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    private byte[] zonePlug9 = new byte[] 
    { 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
      0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
    };

    #endregion

    public static LokiZone Zone1;
    public static LokiZone Zone2;
    public static LokiZone Zone3;
    public static LokiZone Zone4;
    public static LokiZone Zone5;
    public static LokiZone Zone6;
    public static LokiZone Zone7;
    public static LokiZone Zone8;
    public static LokiZone Zone9;

    public int TapReleaseTime { get; set; }
    public int InterTapTime { get; set; }

    private int framesInWindow = 15;
    public int FramesInWindow
    {
      get { return framesInWindow; }
      set 
      {
        framesInWindow = value;
        OnPropertyChanged("FramesInWindow");
      }
    }

    private List<LokiZone> ActiveZones { get; set; }
    private IDictionary<LokiPair, ILokiListener> Listeners { get; set; }

    public LokiManager(int framesIW = 15, int tapReleaseTime = 500, int interTapTime = 500)
    {
      FramesInWindow = framesIW;
      TapReleaseTime = tapReleaseTime;
      InterTapTime = interTapTime;

      Zone1 = new LokiZone("Zone1", this, zonePlug1);
      Zone2 = new LokiZone("Zone2", this, zonePlug2);
      Zone3 = new LokiZone("Zone3", this, zonePlug3);
      Zone4 = new LokiZone("Zone4", this, zonePlug4);
      Zone5 = new LokiZone("Zone5", this, zonePlug5);
      Zone6 = new LokiZone("Zone6", this, zonePlug6);
      Zone7 = new LokiZone("Zone7", this, zonePlug7);
      Zone8 = new LokiZone("Zone8", this, zonePlug8);
      Zone9 = new LokiZone("Zone9", this, zonePlug9);

      ActiveZones = new List<LokiZone>();
      Listeners = new Dictionary<LokiPair, ILokiListener>();
    }

    public void TouchMouseSensorHandler(object sender, TouchMouseSensorEventArgs e)
    {
      if (calibrating)
        AddCalibrationFrame(e.Image);
      else
        ProcessPipeline(e.Image);
    }

    public void PlayerHandler(object sender, PlaybackTouchMouseSensorEventArgs e)
    {
      if (calibrating)
        AddCalibrationFrame(e.Image);
      else
        ProcessPipeline(e.Image);
    }

    
    private void ProcessPipeline(byte[] matrix)
    {
      cons++;
      bool isFinalFrame = matrix.All(cell => cell == 0);
      foreach (LokiZone zone in ActiveZones)
      {
        lock (zone) //this is added in order to allow changing the framesWindow size
        {
          LokiZoneState currentState = CalculateZoneState(matrix, zone.Map, zone, isFinalFrame);
          ProcessInteraction(zone, currentState, isFinalFrame);
        }
      }

      Console.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}", cons, Zone1.LastZoneSum, Zone2.LastZoneSum, Zone3.LastZoneSum, Zone4.LastZoneSum, Zone5.LastZoneSum, Zone6.LastZoneSum, Zone7.LastZoneSum, Zone8.LastZoneSum, Zone9.LastZoneSum);
    }

    private int cons = 0;
    private LokiZoneState CalculateZoneState(byte[] matrix, byte[] zoneprint, LokiZone zone, bool isFinalFrame)
    {
      CircularList<double> framesWindow = zone.FramesWindow;
      if (!isFinalFrame)
      {
        zone.LastZoneSum = CalculateZoneSum(matrix, zoneprint);
        if (zone.LastZoneSum < zone.Min)
          zone.LastZoneSum = zone.Min; //anything below the averague min is an outlier
        framesWindow.Next();
        framesWindow.Value = zone.LastZoneSum;
      }
      else
      {
        framesWindow.SetAll(0);
      }

      double currentAvg = framesWindow.Average();

      if (currentAvg > zone.Max)
        return LokiZoneState.Pressed;
      return LokiZoneState.Clear;
    }

    private int CalculateZoneSum(byte[] matrix, byte[] zoneprint)
    {
      int sum = 0;
      for (int index = 0; index < matrix.Length; index++)
      {
        if (zoneprint[index] == 0)
          continue;
        sum += matrix[index];
      }
      return sum;
    }

    /// <summary>
    /// Process a zone state
    /// </summary>
    /// <param name="zone"></param>
    /// <param name="newState"></param>
    public void ProcessInteraction(LokiZone zone, LokiZoneState newState, bool finalFrame)
    {
      if (zone.State == LokiUtil.LokiZoneState.Clear)
      {
        if (!newState.Equals(LokiUtil.LokiZoneState.Pressed))
          return;

        zone.PressStartTime = DateTime.Now;
        zone.State = LokiUtil.LokiZoneState.Pressed;

        TimeSpan timeFromLastTap = new TimeSpan(DateTime.Now.Ticks - zone.LastTapTime.Ticks);
        if (zone.RecentTap && timeFromLastTap.TotalMilliseconds > InterTapTime)
          zone.RecentTap = false;

        DispatchEvent(zone, LokiEventType.ZonePressed);
      }
      else if (zone.State == LokiUtil.LokiZoneState.Pressed)
      {
        if (!newState.Equals(LokiUtil.LokiZoneState.Clear))
          return;

        TimeSpan span = new TimeSpan(DateTime.Now.Ticks - zone.PressStartTime.Ticks);
        if (span.TotalMilliseconds < TapReleaseTime)
        {
          //Double tap
          if (zone.RecentTap)
          {
            zone.LastTapTime = DateTime.MinValue;
            zone.RecentTap = false;

            zone.StopTapTimer();
            DispatchEvent(zone, LokiEventType.ZoneReleased);
            DispatchEvent(zone, LokiEventType.DoubleTap);
          }
          //Tap
          else
          {
            zone.LastTapTime = DateTime.Now;
            zone.RecentTap = true;

            DispatchEvent(zone, LokiEventType.ZoneReleased);
            zone.StartTapTimer();
          }
        }
        else
        {
          //To long down time - no click
          DispatchEvent(zone, LokiEventType.ZoneReleased);
        }
        zone.State = LokiUtil.LokiZoneState.Clear;
      }
    }

    public void RegisterListener(LokiZone zone, LokiEventType pEvent, ILokiListener listener)
    {
      LokiPair pair = new LokiPair() { Zone = zone, Event = pEvent };
      Listeners.Add(pair, listener);

      //Will only look for input in the zones for which there is an event listener
      ActiveZones.Clear();
      foreach (LokiPair tPair in Listeners.Keys)
      {
        if (ActiveZones.Contains(tPair.Zone))
          continue;
        ActiveZones.Add(tPair.Zone);
      }
    }

    /// <summary>
    /// Dispatches events
    /// </summary>
    /// <param name="zone"></param>
    /// <param name="type"></param>
    public void DispatchEvent(LokiZone zone, LokiEventType type)
    {
      //Console.WriteLine(zone + " : " + type);
      var lToCall = Listeners.Where(tmp => tmp.Key.Zone == zone && (tmp.Key.Event == type || tmp.Key.Event == LokiEventType.All));
      foreach (var registry in lToCall)
      {
        ILokiListener listener = registry.Value;
        listener.HandleLokiEvent(zone, type);
      }
    }

    private bool calibrating = false;
    private int calibrationFramesReceived = 0;
    private int targetCalibrationFrames = 0;
    private ILokiCalibrationListener calibrationL;
    public void Calibrate(ILokiCalibrationListener listener, int seconds = 60)
    {
      calibrating = true;
      calibrationFramesReceived = 0;
      targetCalibrationFrames = seconds * 120;//the mouse normally sends 120 updates per second
      calibrationFrames = new List<byte[]>(targetCalibrationFrames);
      calibrationL = listener;
    }

    private List<byte[]> calibrationFrames;
    private void AddCalibrationFrame(byte[] matrix)
    {
      if (!calibrating)
        return;

      calibrationFramesReceived++;
      calibrationFrames.Add(matrix);
      if (calibrationL != null)
        calibrationL.CalibrationInProgress((calibrationFramesReceived / (double)targetCalibrationFrames) * 100);
      if (calibrationFramesReceived >= targetCalibrationFrames)
        Calibrate();
    }

    private void Calibrate()
    {
      if (!calibrating)
        return;

      //per active zone -- adds the averague and stdev, that determines the max and min
      foreach (LokiZone zone in ActiveZones)
      {
        double averague = calibrationFrames.Average(frame => CalculateZoneSum(frame, zone.Map));
        var sum = calibrationFrames.Sum(frame => Math.Pow(CalculateZoneSum(frame, zone.Map) - averague, 2));
        var stdev = Math.Sqrt((sum) / (calibrationFramesReceived - 1));

        zone.Avg = averague;
        zone.Stdev = stdev;

        //Sets all the values in the time windows to the average as a way to avoid initial triggers
        zone.FramesWindow.SetAll(averague);
      }

      calibrating = false;
      if (calibrationL != null)
        calibrationL.CalibrationFinished();
    }

    public void CancelCalibration()
    {
      calibrating = false;
      calibrationL = null;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

  }

}
