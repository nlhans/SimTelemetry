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
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace LiveTelemetry
{
    public partial class ucResizableImage : UserControl
    {
        public string Caption { get; set; }
        public bool Disabled;
        private string _imagePath = "";
        private Bitmap _imageBMP;
        private Size _bmpSize;

        private ImageAttributes grayscaleAttributes;

        public Size PictureSize
        {
            get
            {
                return _bmpSize;
            }
        }

        public ucResizableImage(string image)
        {
            if (File.Exists(image) == false) return;
            Caption = "";
            InitializeComponent();

            _imagePath = image;
            if (image.ToLower().EndsWith(".tga"))
                _imageBMP = Paloma.TargaImage.LoadTargaImage(image); // http://www.codeproject.com/Articles/31702/NET-Targa-Image-Reader
            else
                _imageBMP = (Bitmap)Image.FromFile(image);

            SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.UserPaint |
              ControlStyles.DoubleBuffer, true);


            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
                new[]
                    {
                        new[] {0.3f, 0.3f, 0.3f, 0.0f, 0.0f},
                        new[] {0.59f, 0.59f, 0.59f, 0.0f, 0.0f},
                        new[] {0.11f, 0.11f, 0.11f, 0.0f, 0.0f},
                        new[] {0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
                        new[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
                    });

            //create some image attributes
            grayscaleAttributes = new ImageAttributes();

            //set the color matrix attribute
            grayscaleAttributes.SetColorMatrix(colorMatrix);
        }

        public void Crop(int w, int h)
        {
            Crop(w, h, true);
        }

        public void Crop(int w, int h, bool resize)
        {
            if (_imageBMP == null)
                return;

            if (h > _imageBMP.Size.Height)
                h = _imageBMP.Size.Height;

            Size = new Size(w, h);
            if (w > _imageBMP.Size.Width)
                w = _imageBMP.Size.Width;
            
            _bmpSize = new Size(w, h);
            
            if (w < _imageBMP.Size.Height)
            {
                _bmpSize = new Size(h * _imageBMP.Size.Width / _imageBMP.Size.Height, h);
            }
            
            if (_imageBMP.Size.Width > w || w < _imageBMP.Size.Width)
            {
                _bmpSize = new Size(w, w * _imageBMP.Size.Height / _imageBMP.Size.Width);
            }
            
            if (this._bmpSize.Height > h)
            {
                // back to original..
                _bmpSize = new Size(h * _imageBMP.Size.Width / _imageBMP.Size.Height, h);
            }

            if (resize)
            {
                Invalidate();
                OnResize(null);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            if (_bmpSize.Width == 0)
            {
                Crop(Size.Width, Size.Height, false);
            }
            var bitmapObject = new Bitmap(Size.Width, Size.Height);
            var graphicsObject = Graphics.FromImage(bitmapObject);

            var sizeRect = new Rectangle((Width - _bmpSize.Width)/2, (Height - _bmpSize.Height)/2,
                                        _bmpSize.Width, _bmpSize.Height);
            graphicsObject.DrawImage(_imageBMP, (Width - _bmpSize.Width) / 2, (Height - _bmpSize.Height) / 2, _bmpSize.Width, _bmpSize.Height);
            graphicsObject.DrawString(Caption, new Font("Tahoma", 10.0f, FontStyle.Underline), Brushes.White, 5, Height-15);
            graphicsObject.Dispose();
            BackgroundImage = bitmapObject;
            if (e != null) base.OnResize(e);
        }

    }

}