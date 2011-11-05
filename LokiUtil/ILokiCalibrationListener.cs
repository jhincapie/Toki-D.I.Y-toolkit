using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LokiUtil
{
  public interface ILokiCalibrationListener
  {
    void CalibrationInProgress(double percentage);
    void CalibrationFinished();
  }
}
