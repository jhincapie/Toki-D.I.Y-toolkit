/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.       *
*                                                       *
********************************************************/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Research.TouchMouseSensor;
using LokiUtil;
using LokiUtil.Recording;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CsWPFBitmapSample
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window, ILokiListener, ILokiCalibrationListener
  {

    private LokiRecorder Recorder { get; set; }
    private LokiPlayer Player { get; set; }
    private LokiManager Manager { get; set; }

    private Dictionary<LokiZone, Label> labels = new Dictionary<LokiZone, Label>();

    public Window1()
    {
      InitializeComponent();

      Player = new LokiPlayer(Environment.CurrentDirectory + @"\RecordedMouseInput.loki");
      Recorder = new LokiRecorder(Environment.CurrentDirectory + @"\RecordedMouseInput.loki");
      bPlay.IsEnabled = false;
      bPause.IsEnabled = false;

      Manager = new LokiManager();
      Manager.RegisterListener(LokiManager.Zone2, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone2, LokiEventType.ZoneReleased, this);
      Manager.RegisterListener(LokiManager.Zone4, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone4, LokiEventType.ZoneReleased, this);
      Manager.RegisterListener(LokiManager.Zone5, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone5, LokiEventType.ZoneReleased, this);
      Manager.RegisterListener(LokiManager.Zone6, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone6, LokiEventType.ZoneReleased, this);
      Manager.RegisterListener(LokiManager.Zone7, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone7, LokiEventType.ZoneReleased, this);
      Manager.RegisterListener(LokiManager.Zone8, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone8, LokiEventType.ZoneReleased, this);
      Manager.RegisterListener(LokiManager.Zone9, LokiEventType.ZonePressed, this);
      Manager.RegisterListener(LokiManager.Zone9, LokiEventType.ZoneReleased, this);

      // Ensure the image rendering does not interpolate
      RenderOptions.SetBitmapScalingMode(SensorImage, BitmapScalingMode.NearestNeighbor);

      // Initialise the mouse and register the callback function
      TouchMouseSensorEventManager.Handler += TouchMouseSensorHandler;
      TouchMouseSensorEventManager.Handler += Manager.TouchMouseSensorHandler;

      // Registers callback function for the player
      Player.Handler += new PlaybackTouchMouseSensorHandler(Player_Handler);
      Player.Handler += Manager.PlayerHandler;

      labels.Add(LokiManager.Zone1, lZone1);
      labels.Add(LokiManager.Zone2, lZone2);
      labels.Add(LokiManager.Zone3, lZone3);
      labels.Add(LokiManager.Zone4, lZone4);
      labels.Add(LokiManager.Zone5, lZone5);
      labels.Add(LokiManager.Zone6, lZone6);
      labels.Add(LokiManager.Zone7, lZone7);
      labels.Add(LokiManager.Zone8, lZone8);
      labels.Add(LokiManager.Zone9, lZone9);
    }

    void Player_Handler(object sender, PlaybackTouchMouseSensorEventArgs e)
    {
      // We're in a thread belonging to the mouse, not the user interface 
      // thread. Need to dispatch to the user interface thread.
      Dispatcher.Invoke((Action<PlaybackTouchMouseSensorEventArgs>)SetSource, e);
    }

    void SetSource(PlaybackTouchMouseSensorEventArgs e)
    {
      // Convert bitmap from memory to graphic form.
      BitmapSource source =
          BitmapSource.Create(e.Status.m_dwImageWidth, e.Status.m_dwImageHeight,
          96, 96,
          PixelFormats.Gray8, null, e.Image, e.Status.m_dwImageWidth);

      // Show bitmap in user interface.
      SensorImage.Source = source;
    }

    /// <summary>
    /// Handle callback from mouse.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    void TouchMouseSensorHandler(object sender, TouchMouseSensorEventArgs e)
    {
      // We're in a thread belonging to the mouse, not the user interface 
      // thread. Need to dispatch to the user interface thread.
      Dispatcher.Invoke((Action<TouchMouseSensorEventArgs>)SetSource, e);
    }

    void SetSource(TouchMouseSensorEventArgs e)
    {
      //Send for recording
      Recorder.Append(e.Status, e.Image);

      // Convert bitmap from memory to graphic form.
      BitmapSource source =
          BitmapSource.Create(e.Status.m_dwImageWidth, e.Status.m_dwImageHeight,
          96, 96,
          PixelFormats.Gray8, null, e.Image, e.Status.m_dwImageWidth);

      // Show bitmap in user interface.
      SensorImage.Source = source;
    }

    private void bRecord_Click(object sender, RoutedEventArgs e)
    {
      Recorder.StartRecording();
      lRecording.Visibility = Visibility.Visible;
      bPause.IsEnabled = true;
      bPlay.IsEnabled = false;
      bRecord.IsEnabled = false;
    }

    private void bPlay_Click(object sender, RoutedEventArgs e)
    {
      Player.StartPlayer();
      lPlaying.Visibility = Visibility.Visible;
      bPause.IsEnabled = true;
      bPlay.IsEnabled = false;
      bRecord.IsEnabled = false;
    }

    private void bPause_Click(object sender, RoutedEventArgs e)
    {
      if (lRecording.Visibility == Visibility.Visible)
      {
        Recorder.StopRecording();
        Player.Started = false;

        lRecording.Visibility = Visibility.Collapsed;
        bPause.IsEnabled = false;
        bPlay.IsEnabled = true;
        bRecord.IsEnabled = true;
      }
      else if (lPlaying.Visibility == Visibility.Visible)
      {
        Player.PausePlayer();

        lPlaying.Visibility = Visibility.Collapsed;
        bPause.IsEnabled = false;
        bPlay.IsEnabled = true;
        bRecord.IsEnabled = true;
      }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      if (Recorder.FileExists())
        bPlay.IsEnabled = true;
      else
        bPlay.IsEnabled = false;
    }

    private void Window_Closed(object sender, EventArgs e)
    {
      Player.StopPlayer();
    }

    public void HandleLokiEvent(LokiZone zone, LokiEventType lEvent)
    {
      Dispatcher.Invoke((Action<LokiZone, LokiEventType>)TSHandleLokiEvent, zone, lEvent);
    }

    private void TSHandleLokiEvent(LokiZone zone, LokiEventType lEvent)
    {
      Console.WriteLine("LokiEvent: {0} in {1}", lEvent, zone);
      foreach (LokiZone tmpZ in labels.Keys)
      {
        Label tmpL = (Label)labels[tmpZ];
        tmpL.Content = String.Empty;
      }

      Label zoneLabel = (Label)labels[zone];
      zoneLabel.Content = lEvent;
    }

    private void bCalibrate_Click(object sender, RoutedEventArgs e)
    {
      bCalibrate.IsEnabled = false;
      Manager.Calibrate(this, 10);
    }

    public void CalibrationFinished()
    {
      // We're in a thread belonging to the mouse, not the user interface 
      // thread. Need to dispatch to the user interface thread.
      Dispatcher.Invoke((Action)TSCalibrationFinished, null);
    }

    void TSCalibrationFinished()
    {
      bCalibrate.IsEnabled = true;
    }

    public void CalibrationInProgress(double percentage)
    {
    }
  }
}
