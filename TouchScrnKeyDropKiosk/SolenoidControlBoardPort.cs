using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace KeyCabinetKiosk
{
    /// <summary>
    /// This class gives the layout for any hardware library which wishes to control relays. All libraries must contain these items.
    /// </summary>
    abstract public class SolenoidControlBoardPort
    {
        abstract protected SerialPort port {get; set; }
        abstract public void OpenAndCloseRelay(int location);
        abstract public void ClosePort();
    }
}
