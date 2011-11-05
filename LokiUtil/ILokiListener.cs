using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LokiUtil
{

  public interface ILokiListener
  {

    void HandleLokiEvent(LokiZone zone, LokiEventType lEvent);

  }

}
