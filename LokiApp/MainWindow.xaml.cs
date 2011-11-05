using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LokiUtil;
using LokiUtil.Recording;
using Microsoft.Research.TouchMouseSensor;
using System.Timers;
using System.ComponentModel;

namespace LokiApp
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged, ILokiListener, ILokiCalibrationListener
  {

    public enum GameState { None, Calibrating, CalibrationFailed, LevelStart, Playback, LokiInput, ErrorAlert, InteractiveMode };

    //game state and levels
    private GameState currentState;
    public List<Level> Levels = new List<Level>();
    private int cLevelIndex = 1;
    private Timer eaTimer = null;

    //Playback for showing the steps of a level
    private Timer playbackTimer = null;

    //Lokie API
    public LokiManager Loki { get; set; }

    //Calibration variables 
    private double cProgress = 0;
    private Timer cTimer = null;

    public TokiElephant Elephant { get; set; }

    public int CurrentLevelIndex
    {
      get { return cLevelIndex; }
      set
      {
        cLevelIndex = value;
        OnPropertyChanged("CurrentLevelIndex");
        OnPropertyChanged("CurrentLevel");
      }
    }

    public Level CurrentLevel
    {
      get
      {
        if (Levels.Count == 0)
          return null;
        return Levels[CurrentLevelIndex - 1];
      }
    }

    public GameState CurrentState
    {
      get { return currentState; }
      set
      {
        currentState = value;
        OnPropertyChanged("CurrentState");
      }
    }

    public double CalibrationProgress
    {
      get { return Math.Round(cProgress); }
      set
      {
        cProgress = value;
        OnPropertyChanged("CalibrationProgress");
      }
    }

    public MainWindow()
    {
      Elephant = new TokiElephant();

      Loki = new LokiManager();
      Loki.RegisterListener(LokiManager.Zone1, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone2, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone3, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone4, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone5, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone6, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone7, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone8, LokiEventType.All, this);
      Loki.RegisterListener(LokiManager.Zone9, LokiEventType.All, this);

      // Initialise the mouse and register the callback function
      TouchMouseSensorEventManager.Handler += Loki.TouchMouseSensorHandler;

      Levels.Add(new Level() { Steps = new List<Step>() });
      Levels[0].Steps.Add(new Step() { Part = BodyPart.Trunk, Gesture = LokiEventType.ZonePressed });
      Levels[0].Steps.Add(new Step() { Part = BodyPart.GreenEar, Gesture = LokiEventType.ZonePressed });
      Levels[0].Steps.Add(new Step() { Part = BodyPart.YellowEar, Gesture = LokiEventType.ZonePressed });

      Levels.Add(new Level() { Steps = new List<Step>() });
      Levels[1].Steps.Add(new Step() { Part = BodyPart.Trunk, Gesture = LokiEventType.ZonePressed });
      Levels[1].Steps.Add(new Step() { Part = BodyPart.GreenEar, Gesture = LokiEventType.Tap });
      Levels[1].Steps.Add(new Step() { Part = BodyPart.YellowEar, Gesture = LokiEventType.Tap });

      Levels.Add(new Level() { Steps = new List<Step>() });
      Levels[2].Steps.Add(new Step() { Part = BodyPart.Trunk, Gesture = LokiEventType.ZonePressed });
      Levels[2].Steps.Add(new Step() { Part = BodyPart.GreenEar, Gesture = LokiEventType.Tap });
      Levels[2].Steps.Add(new Step() { Part = BodyPart.YellowEar, Gesture = LokiEventType.DoubleTap });

      cTimer = new Timer(LokiApp.Properties.Settings.Default.CalibrationNotRespondTimeOutSeconds * 1000);
      cTimer.Elapsed += new ElapsedEventHandler(cTimer_Elapsed);
      eaTimer = new Timer(LokiApp.Properties.Settings.Default.ErrorAlertSeconds * 1000);
      eaTimer.Elapsed += new ElapsedEventHandler(eaTimer_Elapsed);

      InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      Loki.Calibrate(this, LokiApp.Properties.Settings.Default.CalibrationSeconds);
      RunCalibration();
    }

    public void HandleLokiEvent(LokiZone zone, LokiEventType lEvent)
    {
      if (CurrentState != GameState.LokiInput && CurrentState != GameState.ErrorAlert && CurrentState != GameState.InteractiveMode)
        return;

      lock (this)
      {
        if (CurrentState == GameState.InteractiveMode)
          HandleInteractiveModeEvent(zone, lEvent);
        else if (CurrentState == GameState.LokiInput || CurrentState == GameState.ErrorAlert)
          HandleLokiInputEvent(zone, lEvent);
        Console.WriteLine("LokiEvent: {0} in {1}", lEvent, zone);
      }
    }

    private void HandleInteractiveModeEvent(LokiZone zone, LokiEventType lEvent)
    {
      if (lEvent == LokiEventType.ZoneReleased)
      {
        if (zone == LokiUtil.LokiManager.Zone2)
          Elephant.Trunk = false;
        if (zone == LokiUtil.LokiManager.Zone4)
          Elephant.YellowEar = false;
        if (zone == LokiUtil.LokiManager.Zone6)
          Elephant.GreenEar = false;
        if (zone == LokiUtil.LokiManager.Zone7)
          Elephant.BlueHand = false;
        if (zone == LokiUtil.LokiManager.Zone9)
          Elephant.GreenHand = false;
        if (zone == LokiUtil.LokiManager.Zone8)
          Elephant.Belly = false;
      }
      else if (lEvent == LokiEventType.ZonePressed)
      {
        if (zone == LokiUtil.LokiManager.Zone2)
          Elephant.Trunk = true;
        if (zone == LokiUtil.LokiManager.Zone4)
          Elephant.YellowEar = true;
        if (zone == LokiUtil.LokiManager.Zone6)
          Elephant.GreenEar = true;
        if (zone == LokiUtil.LokiManager.Zone7)
          Elephant.BlueHand = true;
        if (zone == LokiUtil.LokiManager.Zone9)
          Elephant.GreenHand = true;
        if (zone == LokiUtil.LokiManager.Zone8)
          Elephant.Belly = true;
      }
    }

    private void HandleLokiInputEvent(LokiZone zone, LokiEventType lEvent)
    {
      if (lEvent == LokiEventType.ZoneReleased)
        return;

      BodyPart part = BodyPartConverter.GetBodyPart(zone);
      if (CurrentLevel.CurrentStep.Gesture == lEvent && CurrentLevel.CurrentStep.Part == part)
      {
        if (CurrentLevel.CurrentStepIndex != CurrentLevel.Steps.Count)
        {
          CurrentLevel.CurrentStepIndex++;
        }
        else  //finished the level
        {
          if (CurrentLevelIndex != Levels.Count)
          {
            CurrentLevelIndex++;
            CurrentState = GameState.LevelStart;
          }
          else //finished the game, starts all over again
          {
            CurrentLevelIndex = 1;
            Levels.ForEach(delegate(Level level)
            {
              level.CurrentStepIndex = 1;
            });
            CurrentState = GameState.LevelStart;
          }
        }
      }
      else //wrong combination
      {
        CurrentState = GameState.ErrorAlert;
        eaTimer.Start();
      }
    }

    private void gClose_MouseDown(object sender, MouseButtonEventArgs e)
    {
      Close();
    }

    private void RunCalibration()
    {
      CurrentState = GameState.Calibrating;

      CalibrationProgress = 0;
      Loki.Calibrate(this, LokiApp.Properties.Settings.Default.CalibrationSeconds);
      cTimer.Start();
    }

    public void CalibrationInProgress(double percentage)
    {
      CalibrationProgress = percentage;
    }

    public void CalibrationFinished()
    {
      Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
        (Action)(() =>
        {
          CurrentState = GameState.LevelStart;

          //Initialize gameplay
          CurrentLevelIndex = 1;
        }));
    }

    void cTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (CalibrationProgress != 0)
      {
        cTimer.Stop();
        return;
      }

      Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
        (Action)(() =>
        {
          CurrentState = GameState.CalibrationFailed;
          cTimer.Stop();
        }));
    }

    void eaTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
        (Action)(() =>
        {
          CurrentState = GameState.LokiInput;
          eaTimer.Stop();
        }));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(String pName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(pName));
    }

    private void bPlay_MouseDown(object sender, MouseButtonEventArgs e)
    {
      lock (this)
      {
        CurrentLevel.CurrentStepIndex = 1;
        CurrentState = GameState.Playback;

        playbackTimer = new Timer(1000);
        playbackTimer.Elapsed += new ElapsedEventHandler(playbackTimer_Elapsed);
        playbackTimer.Start();
      }
    }

    void playbackTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      if (CurrentLevel.CurrentStepIndex < CurrentLevel.Steps.Count)
        CurrentLevel.CurrentStepIndex++;
      else
      {
        playbackTimer.Stop();
        CurrentLevel.CurrentStepIndex = 1;
        CurrentState = GameState.LokiInput;
      }
    }

    private void bRecalibrate_MouseDown(object sender, MouseButtonEventArgs e)
    {
      RunCalibration();
    }

    public class TokiElephant : INotifyPropertyChanged
    {
      private bool trunk = false;
      private bool belly = false;
      private bool greenEar = false;
      private bool yellowEar = false;
      private bool greenHand = false;
      private bool blueHand = false;

      public bool Belly
      {
        get { return belly; }
        set
        {
          belly = value;
          OnPropertyChanged("Belly");
        }
      }

      public bool GreenEar
      {
        get { return greenEar; }
        set
        {
          greenEar = value;
          OnPropertyChanged("GreenEar");
        }
      }

      public bool YellowEar
      {
        get { return yellowEar; }
        set
        {
          yellowEar = value;
          OnPropertyChanged("YellowEar");
        }
      }

      public bool GreenHand
      {
        get { return greenHand; }
        set
        {
          greenHand = value;
          OnPropertyChanged("GreenHand");
        }
      }

      public bool BlueHand
      {
        get { return blueHand; }
        set
        {
          blueHand = value;
          OnPropertyChanged("BlueHand");
        }
      }

      public bool Trunk
      {
        get { return trunk; }
        set
        {
          trunk = value;
          OnPropertyChanged("Trunk");
        }
      }

      public event PropertyChangedEventHandler PropertyChanged;
      private void OnPropertyChanged(String pName)
      {
        if (PropertyChanged != null)
          PropertyChanged(this, new PropertyChangedEventArgs(pName));
      }
    }


    private void bInteractiveMode_MouseDown(object sender, MouseButtonEventArgs e)
    {
      lock (this)
      {
        if (CurrentState != GameState.InteractiveMode)
          CurrentState = GameState.InteractiveMode;
        else
          CurrentState = GameState.LevelStart;
      }
    }

  }

}
