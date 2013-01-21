/*************************************************************************
 *                         SimTelemetry                                  *
 *        providing live telemetry read-out for simulators               *
 *             Copyright (C) 2011-2012 Hans de Jong                      *
 *                                                                       *
 *  This program is free software: you can redistribute it and/or modify *
 *  it under the terms of the GNU General Public License as published by *
 *  the Free Software Foundation, either version 3 of the License, or    *
 *  (at your option) any later version.                                  *
 *                                                                       *
 *  This program is distributed in the hope that it will be useful,      *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of       *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the        *
 *  GNU General Public License for more details.                         *
 *                                                                       *
 *  You should have received a copy of the GNU General Public License    *
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.*
 *                                                                       *
 * Source code only available at https://github.com/nlhans/SimTelemetry/ *
 ************************************************************************/
namespace SimTelemetry.Objects.Peripherals
{
    public delegate void DevicePacketEvent(DevicePacket packet, object sender);
    public class DevicePacket
    {
        public int ID { get; private set; }
        public int Length { get; private set; }
        public byte CRC { get; private set; }
        public byte[] Data { get; private set; }

        public DevicePacket(int id,byte[] data)
        {
            ID = id;
            Length = data.Length;
            Data = data;

            CRC = 0xAA;
            foreach (byte b in data)
                CRC -= b;
        }
    }
}