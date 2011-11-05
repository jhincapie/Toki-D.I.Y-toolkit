using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Research.TouchMouseSensor;

namespace LokiUtil.Recording
{

  public class LokiPlayer
  {

    private String RecordFileName { get; set; }
    private FileStream RecordFile { get; set; }

    private Thread PlaybackThread { get; set; }
    public bool Running { get; set; }
    public bool Started { get; set; }
    public bool Stopped { get; set; }

    private int RecordIndex = 0;
    private List<PlaybackTouchMouseSensorEventArgs> Records;
    public event PlaybackTouchMouseSensorHandler Handler;

    public LokiPlayer(String fileName)
    {
      RecordFileName = fileName;
      Started = false;
      Running = false;
      Stopped = false;

      Records = new List<PlaybackTouchMouseSensorEventArgs>();
    }

    public void StartPlayer()
    {
      if (!Started)
      {
        Started = true;
        LoadAllRecords();
        PlaybackThread = new Thread(PlayBack.Play);
        PlaybackThread.Start(this);
      }

      lock (this)
        Running = true;
    }

    public void PausePlayer()
    {
      lock (this)
        Running = false;
    }

    public void StopPlayer()
    {
      PausePlayer();
      lock (this)
        Stopped = true;
    }

    private void LoadAllRecords()
    {
      if (!File.Exists(RecordFileName))
        throw new ArgumentException("Provided file name doesn't exist");

      Records.Clear();
      byte[] buffer = new byte[15 * 13];
      RecordFile = new FileStream(RecordFileName, FileMode.Open);
      while (RecordFile.Position != RecordFile.Length)
      {
        //RecordFile.Write(System.BitConverter.GetBytes(elapse.Ticks), 0, 8);
        //RecordFile.Write(System.BitConverter.GetBytes(status.m_dwID), 0, 8);
        //RecordFile.Write(System.BitConverter.GetBytes(status.m_dwImageHeight), 0, 4);
        //RecordFile.Write(System.BitConverter.GetBytes(status.m_dwImageWidth), 0, 4);
        //RecordFile.Write(System.BitConverter.GetBytes(status.m_dwTimeDelta), 0, 4);
        //RecordFile.Write(System.BitConverter.GetBytes(status.m_fDisconnect), 0, 1);
        //RecordFile.Write(mouseMatrix, 0, mouseMatrix.Length);

        long dwID;
        int dwImageHeight;
        int dwImageWidth;
        int dwTimeDelta;
        bool fDisconnect;
        PlaybackTouchMouseSensorEventArgs args = new PlaybackTouchMouseSensorEventArgs();

        RecordFile.Read(buffer, 0, 8);
        args.Elapsed = new TimeSpan(System.BitConverter.ToInt64(buffer, 0));
        RecordFile.Read(buffer, 0, 8);
        dwID = System.BitConverter.ToInt64(buffer, 0);
        RecordFile.Read(buffer, 0, 4);
        dwImageHeight = System.BitConverter.ToInt32(buffer, 0);
        RecordFile.Read(buffer, 0, 4);
        dwImageWidth = System.BitConverter.ToInt32(buffer, 0);
        RecordFile.Read(buffer, 0, 4);
        dwTimeDelta = System.BitConverter.ToInt32(buffer, 0);
        RecordFile.Read(buffer, 0, 1);
        fDisconnect = System.BitConverter.ToBoolean(buffer, 0);
        RecordFile.Read(buffer, 0, buffer.Length);

        args.Status = new TOUCHMOUSESTATUS() { m_dwID = dwID, m_dwImageHeight = dwImageHeight, m_dwImageWidth = dwImageWidth, m_dwTimeDelta = dwTimeDelta, m_fDisconnect = fDisconnect };
        args.Image = new byte[buffer.Length];
        buffer.CopyTo(args.Image, 0);

        Records.Add(args);
      }

      RecordFile.Close();
    }

    private void NotifyHandler()
    {
      if (Handler == null || Records.Count == 0)
        return;

      PlaybackTouchMouseSensorEventArgs record = Records[(RecordIndex++) % Records.Count];
      if (record.Status.m_dwTimeDelta != 0)
        Thread.Sleep(record.Status.m_dwTimeDelta);
      else
        Thread.Sleep(500);

      Handler(this, record);

      if (RecordIndex == Records.Count)
        RecordIndex = 0;
    }

    class PlayBack
    {
      PlayBack() { }
      public static void Play(object data)
      {
        LokiPlayer player = (LokiPlayer)data;
        while (true)
        {
          bool running = true;
          lock (player)
            running = player.Running;
          if (!running)
          {
            Thread.Sleep(500);

            bool stopped = true;
            lock (player)
              stopped = player.Stopped;
            if (stopped)
              break;

            continue;
          }

          player.NotifyHandler();
        }
      }
    }

  }

}
