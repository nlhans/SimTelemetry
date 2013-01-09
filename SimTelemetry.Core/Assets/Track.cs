using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SimTelemetry.Core.Enumerations;
using SimTelemetry.Core.ValueObjects;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Core.Assets
{
    public class TrackData
    {
        public string Name;
        public double Length { get; set; }

        public ITrack GarageObject;

        public List<TrackPoint> Waypoints { get; set; }
        public RectangleF Bounds { get; set; }

        public IEnumerable<TrackPoint> Track { get { return Waypoints.Where(x => x.Type != TrackPointType.PITS); } }
        public IEnumerable<TrackPoint> Pits { get { return Waypoints.Where(x => x.Type == TrackPointType.PITS); } }

        public TrackData(string name, ITrack garageObject, List<TrackPoint> route)
        {
            Waypoints = route;
            Name = name;
            GarageObject = garageObject;

            // Sort on meters:
            Waypoints.Sort((x1, x2) => x1.Meter.CompareTo(x2.Meter));

            // Get min/max X/Y
            var minX = (float)Waypoints.Min(x => x.X);
            var minY = (float)Waypoints.Min(x => x.Y);
            var maxX = (float)Waypoints.Max(x => x.X);
            var maxY = (float)Waypoints.Max(x => x.Y);

            // Track map bounds:
            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);

            // Track length:
            Length = Track.Max(x => x.Meter) - Track.Min(x => x.Meter);
        }
    }
    public class Track
    {
        public TrackData Loaded { get; protected set; }

        public string Name { get { return ((Loaded == null) ? string.Empty : Loaded.Name); } }
        public double Length { get { return ((Loaded == null) ? -1 : Loaded.Length); } }

        public int ImageX { get; set; }
        public int ImageY { get; set; }

        public int DrawingWidth { get { return ImageX - Margins.Left - Margins.Right; } }
        public int DrawingHeight { get { return ImageY - Margins.Top - Margins.Bottom; } }

        public ImageMargin Margins { get; set; }

        protected SolidBrush Brush_Sector1 { get; set; }
        protected SolidBrush Brush_Sector2 { get; set; }
        protected SolidBrush Brush_Sector3 { get; set; }
        protected SolidBrush Brush_Pits { get; set; }

        public Track(int imageX, int imageY)
        {
            Margins = new ImageMargin(10, 10, 10, 10);

            ImageX = imageX;
            ImageY = imageY;

        }

        public double GetX(double x)
        {
            return Margins.Left + DrawingWidth*(x - Loaded.Bounds.X)/Loaded.Bounds.Width;
        }

        public double GetY(double y)
        {
            return Margins.Top + DrawingHeight* (y- Loaded.Bounds.Y) / Loaded.Bounds.Height;
        }

        public PointF Get(double x, double y)
        {
            return new PointF((float)GetX(x), (float)GetY(y));
        }

        public void Load(TrackData track)
        {
            Loaded = track;

            // Do track loading here.
            var bounds = new Rectangle();
        }
    }

    public class ImageMargin
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public ImageMargin(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
