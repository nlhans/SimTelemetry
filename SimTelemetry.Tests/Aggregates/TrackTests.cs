using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Domain.Aggregates;
using SimTelemetry.Domain.Enumerations;
using SimTelemetry.Domain.ValueObjects;

namespace SimTelemetry.Tests.Aggregates
{
    [TestFixture]
    class TrackTests : IDisposable
    {
        private Track track;
        public TrackTests()
        {
            track = new Track("Test.gdb", "Test", "Belgium", 100, 95);
            var points = new List<TrackPoint>
                             {
                                 new TrackPoint(0, TrackPointType.SECTOR1, 1, 1, 1, new float[0], new float[0]),
                                 new TrackPoint(10, TrackPointType.SECTOR1, 0.5f, 1, 1, new float[0], new float[0]),
                                 new TrackPoint(20, TrackPointType.SECTOR1, 0, 1, 1, new float[0], new float[0]),
                                 new TrackPoint(70, TrackPointType.SECTOR3, -0.5f, 0.5f, 1, new float[0], new float[0]),
                                 new TrackPoint(80, TrackPointType.SECTOR3, 0, 0.5f, 1, new float[0], new float[0]),
                                 new TrackPoint(90, TrackPointType.SECTOR3, 0.5f, 0.5f, 1, new float[0], new float[0]),
                                 new TrackPoint(30, TrackPointType.SECTOR2, -0.5f, 1, 1, new float[0], new float[0]),
                                 new TrackPoint(60, TrackPointType.SECTOR3, -1f, 0, 1, new float[0], new float[0]),
                                 new TrackPoint(40, TrackPointType.SECTOR2, -1f, 1, 1, new float[0], new float[0]),
                                 new TrackPoint(50, TrackPointType.SECTOR2, -1f, 0.5f, 1, new float[0], new float[0]),
                                 new TrackPoint(202, TrackPointType.PITS, 1f, 0.5f, 1, new float[0], new float[0]),
                                 new TrackPoint(-51, TrackPointType.PITS, 2f, -1f, 1, new float[0], new float[0]),
                                 new TrackPoint(0, TrackPointType.GRID, 100f, 100f, 1, new float[0], new float[0])
                             };
            track.SetTrack(points);
        }
        public void Dispose()
        {
            track = null;
        }

        [Test]
        public void Basic()
        {
            Assert.AreEqual("Test.gdb", track.ID);
            Assert.AreEqual("Test", track.Name);
            Assert.AreEqual("Belgium", track.Location);
            Assert.AreEqual(100.0f, track.LaprecordRace);
            Assert.AreEqual(95.0f, track.LaprecordQualify);
        }

        [Test]
        public void Route()
        {
            Assert.AreEqual(90.0f, track.Length); // 0-90 length

            Assert.AreEqual(2, track.Pits.ToList().Count);
            Assert.AreEqual(1, track.Grid.ToList().Count);
            Assert.AreEqual(3, track.Route.Where(x => x.Type == TrackPointType.SECTOR1).ToList().Count);
            Assert.AreEqual(3, track.Route.Where(x => x.Type == TrackPointType.SECTOR2).ToList().Count);
            Assert.AreEqual(4, track.Route.Where(x => x.Type == TrackPointType.SECTOR3).ToList().Count);

            var prevPoint = new TrackPoint(0, TrackPointType.GRID, 0,0,0, new float[1] { 0}, new float[0]);
            var prevPointInit = false;
            foreach(var point in track.Route)
            {
                if (prevPointInit)
                {
                    Assert.Greater(point.Meter, prevPoint.Meter);
                }
                prevPointInit = true;
                prevPoint = point;
            }
        }
    }
}
