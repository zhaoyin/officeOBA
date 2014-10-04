using AODL.IO;

namespace AODL.Document.Content.Draw
{
    public class ImageInfo
    {
        private readonly int _dPI_X;
        private readonly int _dPI_Y;
        private readonly double _height;
        private readonly int _heightInPixel;
        private readonly string _measurementFormat;
        private readonly string _name;
        private readonly string _svgHeight;
        private readonly string _svgWidth;
        private readonly double _width;
        private readonly int _widthInPixel;

        public ImageInfo(IFile file, string svgWidth, string svgHeight)
        {
            _measurementFormat = "cm";
            _svgWidth = svgWidth;
            _svgHeight = svgHeight;

            _name = "image";
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