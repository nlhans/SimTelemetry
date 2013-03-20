using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Logger;

namespace SimTelemetry.Tests.DataAnalyzer
{
    [TestFixture]
    class DataAnalyzer
    {
        private bool IsLockingWheels(LogSampleGroup drv)
        {
            var wheelSpeedLF = drv.ReadAs<float>("TyreSpeedLF")*0.327*-3.6; // speed*radius*3.6 in km/h
            var wheelSpeedRF = drv.ReadAs<float>("TyreSpeedRF")*0.327*-3.6;
            var wheelSpeedLR = drv.ReadAs<float>("TyreSpeedLR")*0.328*-3.6;
            var wheelSpeedRR = drv.ReadAs<float>("TyreSpeedRR")*0.328*-3.6;
            var speed = drv.ReadAs<float>("Speed")*3.6;

            var factorLF = wheelSpeedLF/speed;
            var factorRF = wheelSpeedRF/speed;
            var factorLR = wheelSpeedLR/speed;
            var factorRR = wheelSpeedRR/speed;

            if (factorLF < 0.85 && (speed - wheelSpeedLF) > 10) return true;
            if (factorRF < 0.85 && (speed - wheelSpeedRF) > 10) return true;
            if (factorLR < 0.85 && (speed - wheelSpeedLR) > 10) return true;
            if (factorRR < 0.85 && (speed - wheelSpeedRR) > 10) return true;
            return false;
        }

        private bool IsSpinningWheels(LogSampleGroup drv)
        {
            var wheelSpeedLF = drv.ReadAs<float>("TyreSpeedLF") * 0.327 * -3.6; // speed*radius*3.6 in km/h
            var wheelSpeedRF = drv.ReadAs<float>("TyreSpeedRF") * 0.327 * -3.6;
            var wheelSpeedLR = drv.ReadAs<float>("TyreSpeedLR") * 0.328 * -3.6;
            var wheelSpeedRR = drv.ReadAs<float>("TyreSpeedRR") * 0.328 * -3.6;
            var speed = drv.ReadAs<float>("Speed") * 3.6;

            var factorLF = wheelSpeedLF / speed;
            var factorRF = wheelSpeedRF / speed;
            var factorLR = wheelSpeedLR / speed;
            var factorRR = wheelSpeedRR / speed;

            //if (factorLF > 1.2 && (wheelSpeedLF - speed) > 20) return true;
            //if (factorRF > 1.2 && (wheelSpeedRF - speed) > 20) return true;

            // only check on driven wheels
            if (factorLR > 1.2 && (wheelSpeedLR - speed) > 20) return true;
            if (factorRR > 1.2 && (wheelSpeedRR - speed) > 20) return true;
            return false;
        }

        [Test]
        public void analyzer()
        {
            // read the data file
            var telRead = new LogFileReader(TestConstants.TestFolder + "\\2013-03-19-18-48 - Session.zip"); // data file of movie
            var telProvider = telRead.GetProvider(new[] { "Driver 7427264" }, 0, 100000000); // TODO: end-of-file-time

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Time,RPM,Speed,X,Y,Z,Yaw,Whlspd LF, Whlspd RF, Whlspd LR, Whlspd RR,Blocking,Spinning");

            foreach (var sample in telProvider.GetSamples())
            {
                var me = sample.Get("Driver 7427264");
                //
                builder.AppendLine(sample.Timestamp + "," + me.ReadAs<float>("RPM") + "," + me.ReadAs<float>("Speed") +
                                   "," + me.ReadAs<float>("CoordinateX") + "," + me.ReadAs<float>("CoordinateY") + "," + me.ReadAs<float>("CoordinateZ") + "," + me.ReadAs<float>("Yaw")
                                   + "," + me.ReadAs<float>("TyreSpeedLF")+ "," + me.ReadAs<float>("TyreSpeedRF")+ "," + me.ReadAs<float>("TyreSpeedLR")+ "," + me.ReadAs<float>("TyreSpeedRR")
                                   + "," + IsLockingWheels(me) + "," + IsSpinningWheels(me));
            }

            File .WriteAllText("out.2csv", builder.ToString());
        }
    }
}
