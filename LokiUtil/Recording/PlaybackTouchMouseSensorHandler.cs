using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.TouchMouseSensor;

namespace LokiUtil.Recording
{

  public delegate void PlaybackTouchMouseSensorHandler(object sender, PlaybackTouchMouseSensorEventArgs e);

  public class PlaybackTouchMouseSensorEventArgs : EventArgs
  {
    public TimeSpan Elapsed { get; set; }
    public byte[] Image { get; set; }
    public TOUCHMOUSESTATUS Status { get; set; }
  }

}
