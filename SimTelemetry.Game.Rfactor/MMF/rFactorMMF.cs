using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using SimTelemetry.Objects;

namespace SimTelemetry.Game.Rfactor.MMF
{
    public class rFactorMMF
    {
        public rFactorTelemetry Telemetry { get; set; }
        public List<rFactorDriver> Drivers { get; set; }

        private Thread _mRefresher;

        public bool Hooked { get; private set; }

        private MemoryMappedFile _mMMF;
        private MemoryMappedViewAccessor _mAccessor;

        public rFactorMMF()
        {
            _mRefresher = new Thread(DataUpdater);
            _mRefresher.IsBackground = true;
            _mRefresher.Start();
        }

        public void Connect()
        {
            string map = @"Local\SimTelemetryRfactor2"; // TODO: Fix this name :)
            try
            {
                _mMMF = MemoryMappedFile.CreateOrOpen(map, 16*1024, MemoryMappedFileAccess.ReadWrite);
                _mAccessor = _mMMF.CreateViewAccessor(0, 16*1024);
                Hooked = true;
            }
            catch (Exception e)
            {
                Hooked = false;
                //
            }
        }

        public void Disconnect()
        {
            Hooked = false;
            _mAccessor.Dispose();
            _mMMF.Dispose();
        }

        private void DataUpdater()
        {
            while (_mRefresher.IsAlive)
            {
                while (Hooked)
                {
                    Update();

                    Thread.Sleep(
                        1);
                }

                Connect();
                Thread.Sleep(500);

            }
        }
    

        public void Update()
        {
            // Update general telemetry:
            byte[] TelemeData = new byte[Marshal.SizeOf(typeof(rFactorTelemetry))];
            _mAccessor.ReadArray(0, TelemeData, 0, TelemeData.Length);
            Telemetry = ByteMethods.ToObject<rFactorTelemetry>(TelemeData);

            // Update all drivers:
            Drivers = new List<rFactorDriver>();
            for (int i = 0; i < Telemetry.Session.Cars; i++)
            {
                int offset = 4 + 124 + 296 + i * 108;// TODO: autodetect
                byte[] DriverData = new byte[Marshal.SizeOf(typeof(rFactorDriver))];
                _mAccessor.ReadArray(offset, DriverData, 0, DriverData.Length);

                rFactorDriver driver = ByteMethods.ToObject<rFactorDriver>(DriverData);
                Drivers.Add(driver);
            }
        }
    }

}
