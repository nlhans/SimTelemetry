using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimTelemetry.Objects.Peripherals
{
    public interface IDevices
    {
        event DevicePacketEvent RX;
        void TX(DevicePacket packet, string destination);
    }

}
