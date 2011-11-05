using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LokiApp
{

  public enum BodyPart { None, Trunk, YellowEar, GreenEar, BlueHand, GreenHand, Belly };

  public class BodyPartConverter
  {
    public static LokiUtil.LokiZone GetMappedZone(BodyPart part)
    {
      switch (part)
      {
        case BodyPart.Trunk:
          return LokiUtil.LokiManager.Zone2;
        case BodyPart.YellowEar:
          return LokiUtil.LokiManager.Zone4;
        case BodyPart.GreenEar:
          return LokiUtil.LokiManager.Zone6;
        case BodyPart.BlueHand:
          return LokiUtil.LokiManager.Zone7;
        case BodyPart.GreenHand:
          return LokiUtil.LokiManager.Zone9;
        case BodyPart.Belly:
        default:
          return LokiUtil.LokiManager.Zone8;
      }
    }

    public static BodyPart GetBodyPart(LokiUtil.LokiZone zone)
    {
      if(zone == LokiUtil.LokiManager.Zone2)
        return BodyPart.Trunk;
      if(zone == LokiUtil.LokiManager.Zone4)
        return BodyPart.YellowEar;
      if(zone == LokiUtil.LokiManager.Zone6)
        return BodyPart.GreenEar;
      if(zone == LokiUtil.LokiManager.Zone7)
        return BodyPart.BlueHand;
      if(zone == LokiUtil.LokiManager.Zone9)
        return BodyPart.GreenHand;
      if(zone == LokiUtil.LokiManager.Zone8)
        return BodyPart.Belly;
      return BodyPart.None;
    }

  }
}
