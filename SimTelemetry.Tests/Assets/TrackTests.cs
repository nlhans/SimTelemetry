using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Core.Assets;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Tests.Assets
{
    [TestFixture]
    public class TrackTests
    {
        private string Name = "Test Track";
        private ITrack GarageObject = (ITrack)null;
        private List<TrackDataPoint> Route = new List<TrackDataPoint>();

        private static TrackData track;

        [SetUp]
        public void CreateTrackData()
        {
            Route = new List<TrackDataPoint>();

            var point1 = new TrackDataPoint { Meter = 0, X = -1, Y = -1, Width = 10, Sector = TrackSector.SECTOR1 };
            var point2 = new TrackDataPoint { Meter = 10, X = 0, Y = -1, Width = 10, Sector = TrackSector.SECTOR1 };
            var point3 = new TrackDataPoint { Meter = 20, X = 1, Y = -1, Width = 10, Sector = TrackSector.SECTOR1 };
            var point4 = new TrackDataPoint { Meter = 30, X = 1, Y = 0, Width = 10, Sector = TrackSector.SECTOR2 };
            var point5 = new TrackDataPoint { Meter = 40, X = 0, Y = 0, Width = 10, Sector = TrackSector.SECTOR2 };
            var point6 = new TrackDataPoint { Meter = 50, X = 0, Y = 1, Width = 10, Sector = TrackSector.SECTOR2 };
            var point7 = new TrackDataPoint { Meter = 60, X = -1, Y = 1, Width = 10, Sector = TrackSector.SECTOR3 };
            var point8 = new TrackDataPoint { Meter = 70, X = -1, Y = 0, Width = 10, Sector = TrackSector.SECTOR3 };
            var pits1 = new TrackDataPoint { Meter = -5, X = -1, Y = -1.5, Width = 10, Sector = TrackSector.PITS };
            var pits2 = new TrackDataPoint { Meter = 75, X = 0, Y = -1.5, Width = 10, Sector = TrackSector.PITS };

            Route.Add(point1);
            Route.Add(point2);
            Route.Add(point3);
            Route.Add(point4);
            Route.Add(point5);
            Route.Add(point6);
            Route.Add(point7);
            Route.Add(point8);
            Route.Add(pits1);
            Route.Add(pits2);

            track = new TrackData(Name, GarageObject, new List<TrackDataPoint>(Route));

            Assert.AreEqual(2, track.Pits.ToList().Count);
            Assert.AreEqual(8, track.Track.ToList().Count);

        }

        [Test]
        public void TrackDataTest()
        {
            Assert.AreEqual(Name, track.Name);
            Assert.AreEqual(GarageObject, track.GarageObject);
            Assert.AreEqual(Route.Count, track.Waypoints.Count);

            // Track path analysis
            Assert.AreEqual(70, track.Length);
            Assert.AreEqual(-1, track.Bounds.X);
            Assert.AreEqual(-1.5, track.Bounds.Y);
            Assert.AreEqual(2, track.Bounds.Width);
            Assert.AreEqual(2.5, track.Bounds.Height);

            // Check ordering of track points.
            var lastMeters = -1.0;
            foreach (var point in track.Track)
            {
                Assert.Greater(point.Meter, lastMeters);
                lastMeters = point.Meter;
            }
        }

        [Test]
        public void TrackAssetTest()
        {
            var trackAsset = new Track(200, 200);
            trackAsset.Load(track);

            Assert.AreEqual(200, trackAsset.ImageX);
            Assert.AreEqual(200, trackAsset.ImageY);

            Assert.AreEqual(Name, trackAsset.Name);
            Assert.AreEqual(70, trackAsset.Length);

            // Coordinates based on imagesize
            var point1 = trackAsset.Get(-1, -1.5);
            var point2 = trackAsset.Get(-1, 0);
            var point3 = trackAsset.Get(0, 0);
            var point4 = trackAsset.Get(1, 1);

            Assert.AreEqual(new PointF(10, 10), point1);
            Assert.AreEqual(new PointF(10, 118), point2);
            Assert.AreEqual(new PointF(100, 118), point3);
            Assert.AreEqual(new PointF(190, 190), point4);

        }
    }
}
