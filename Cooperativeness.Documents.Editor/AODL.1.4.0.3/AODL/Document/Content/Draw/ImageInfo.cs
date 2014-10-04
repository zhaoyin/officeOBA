using System;
using System.Drawing;
using AODL.IO;
using SizeConverter=AODL.Document.Helper.SizeConverter;

namespace AODL.Document.Content.Draw
{
    public class ImageInfo
    {
        private readonly int _widthInPixel;
        private readonly int _heightInPixel;
        private readonly int _dPI_X;
        private readonly int _dPI_Y;
        private readonly string _measurementFormat;
        private readonly double _width;
        private readonly double _height;
        private readonly string _svgWidth;
        private readonly string _svgHeight;
        private readonly string _name;

        public ImageInfo(IFile file, string svgWidth, string svgHeight)
        {
            Image image = null;
            Graphics graphics = null;
            try
            {
                try
                {
                    image = Image.FromStream(file.OpenRead());
                }
                catch (OutOfMemoryException e)
                {
                    throw new AODLGraphicException(string.Format("file '{0}' contains an unrecognized image", file), e);
                }

                try
                {
                    graphics = Graphics.FromImage(image);
                }
                catch (Exception e)
                {
                    throw new AODLGraphicException(string.Format(
                                                       "file '{0}' contains an unrecognized image. I could not" +
                                                       " create graphics object from image {1}", file, image),
                                                   e);
                }

                _widthInPixel = image.Width;
                _heightInPixel = image.Height;
                _dPI_X = (int) graphics.DpiX;
                _dPI_Y = (int) graphics.DpiY;
                _measurementFormat = "cm";
                _width = SizeConverter.GetWidthInCm(image.Width, _dPI_X);
                _height = SizeConverter.GetHeightInCm(image.Height, _dPI_Y);
                if (svgHeight == null && svgWidth == null)
                {
                    _svgHeight = _height.ToString("F3") + _measurementFormat;
                    if (_svgHeight.IndexOf(",", 0) > -1)
                        _svgHeight = _svgHeight.Replace(",", ".");
                    _svgWidth = _width.ToString("F3") + _measurementFormat;
                    if (_svgWidth.IndexOf(",", 0) > -1)
                        _svgWidth = _svgWidth.Replace(",", ".");
                }
                else
                {
                    // This should only reached, if the file is loading by a importer.
                    _measurementFormat = SizeConverter.IsCm(svgWidth) ? "cm" : "in";
                    _svgWidth = svgWidth;
                    _svgHeight = svgHeight;
                }

                _name = file.Name;
            }
            finally
            {
                if (image != null)
                    image.Dispose();
                if (graphics != null)
                    graphics.Dispose();
            }
        }

        public int WidthInPixel
        {
            get { return _widthInPixel; }
        }

        public int HeightInPixel
        {
            get { return _heightInPixel; }
        }

        public int DPI_X
        {
            get { return _dPI_X; }
        }

        public int DPI_Y
        {
            get { return _dPI_Y; }
        }

        public string MeasurementFormat
        {
            get { return _measurementFormat; }
        }

        public double Width
        {
            get { return _width; }
        }

        public double Height
        {
            get { return _height; }
        }

        public string SvgWidth
        {
            get { return _svgWidth; }
        }

        public string SvgHeight
        {
            get { return _svgHeight; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}