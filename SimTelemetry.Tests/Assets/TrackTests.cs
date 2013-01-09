using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SimTelemetry.Core.Assets;
using SimTelemetry.Core.Enumerations;
using SimTelemetry.Core.ValueObjects;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Tests.Assets
{
    [TestFixture]
    public class TrackTests
    {
        private string Name = "Test Track";
        private ITrack GarageObject = (ITrack)null;
        private List<TrackPoint> Route = new List<TrackPoint>();

        private static TrackData track;

        [SetUp]
        public void CreateTrackData()
        {
            Route = new List<TrackPoint>(new []
                                             {
                                                 new TrackPoint(0.0f, TrackPointType.SECTOR1, -1.0f, -1.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(10.0f, TrackPointType.SECTOR1, 0.0f, -1.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(20.0f, TrackPointType.SECTOR1, 0.0f, 0.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(30.0f, TrackPointType.SECTOR2, 1.0f, 0.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(40.0f, TrackPointType.SECTOR2, 1.0f, 1.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(50.0f, TrackPointType.SECTOR3, 0.0f, 1.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(60.0f, TrackPointType.SECTOR3, -1.0f, 1.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(70.0f, TrackPointType.SECTOR3, -1.0f, 0.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(5.0f, TrackPointType.PITS, -1.5f, -1.0f, 0.0f, new double[0], new double[0]),
                                                 new TrackPoint(15.0f, TrackPointType.PITS, -1.5f, -2.0f, 0.0f, new double[0], new double[0]),
                                             });


            track = new TrackData(Name, GarageObject, new List<TrackPoint>(Route));

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
