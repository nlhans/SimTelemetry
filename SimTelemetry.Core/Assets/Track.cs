using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SimTelemetry.Objects.Garage;

namespace SimTelemetry.Core.Assets
{
    public class TrackData
    {
        public string Name;
        public double Length { get; set; }

        public ITrack GarageObject;
        
        public List<TrackDataPoint> Waypoints { get; set; }
        public RectangleF Bounds { get; set; }

        public IEnumerable<TrackDataPoint> Track { get { return Waypoints.Where(x => x.Sector != TrackSector.PITS); } }
        public IEnumerable<TrackDataPoint> Pits { get { return Waypoints.Where(x => x.Sector == TrackSector.PITS); } }

        public TrackData(string name, ITrack garageObject, List<TrackDataPoint> route)
        {
            Waypoints = route;
            Name = name;
            GarageObject = garageObject;
            
            // Sort on meters:
            Waypoints.Sort((x1, x2) => x1.Meter.CompareTo(x2.Meter));

            // Get min/max X/Y
            var minX = (float) Waypoints.Min(x => x.X);
            var minY = (float) Waypoints.Min(x => x.Y);
            var maxX = (float) Waypoints.Max(x => x.X);
            var maxY = (float) Waypoints.Max(x => x.Y);

            // Track map bounds:
            Bounds = new RectangleF(minX, minY, maxX - minX, maxY - minY);

            // Track length:
            Length = Track.Max(x => x.Meter) - Track.Min(x => x.Meter);
        }
    }

    public class TrackDataPoint
    {
        public double Meter;
        public double X;
        public double Y;

        public double Width;
        public TrackSector Sector;
    }

    public enum TrackSector
    {
        PITS,
        SECTOR1,
        SECTOR2,
        SECTOR3
    }

    public class ImageMargin
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public ImageMargin(int l, int t, int r, int b)
        {
            Left = l;
            Top = t;
            Right = r;
            Bottom = b;
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
}
