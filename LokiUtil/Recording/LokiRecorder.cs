using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Research.TouchMouseSensor;

namespace LokiUtil.Recording
{

  public class LokiRecorder
  {

    private String RecordFileName { get; set; }

    private DateTime StartPoint { get; set; }
    private FileStream RecordFile { get; set; }

    public LokiRecorder(String fileName)
    {
      StartPoint = DateTime.MinValue; ;
      RecordFileName = fileName;
    }

    public void StartRecording()
    {
      if (File.Exists(RecordFileName))
        File.Delete(RecordFileName);
      RecordFile = new FileStream(RecordFileName, FileMode.Create);
      StartPoint = DateTime.Now;
    }

    public void StopRecording()
    {
      StartPoint = DateTime.MinValue;
      RecordFile.Flush();
      RecordFile.Close();
    }

    public void Append(TOUCHMOUSESTATUS status, byte[] mouseMatrix)
    {
      if (StartPoint == DateTime.MinValue)
        return;

      TimeSpan elapse = new TimeSpan(DateTime.Now.Ticks - StartPoint.Ticks);
      RecordFile.Write(System.BitConverter.GetBytes(elapse.Ticks), 0, 8);
      RecordFile.Write(System.BitConverter.GetBytes(status.m_dwID), 0, 8);
      RecordFile.Write(System.BitConverter.GetBytes(status.m_dwImageHeight), 0, 4);
      RecordFile.Write(System.BitConverter.GetBytes(status.m_dwImageWidth), 0, 4);
      RecordFile.Write(System.BitConverter.GetBytes(status.m_dwTimeDelta), 0, 4);
      RecordFile.Write(System.BitConverter.GetBytes(status.m_fDisconnect), 0, 1);
      RecordFile.Write(mouseMatrix, 0, mouseMatrix.Length);
    }


    public bool FileExists()
    {
      return File.Exists(RecordFileName);
    }
  }

}
