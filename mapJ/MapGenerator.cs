using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;

namespace mapJ
{
    public class MapGenerator
    {
        private double _hexSize;
        private double _hexRadius;
        private double _hexGap;
        private readonly Canvas _canvas;
        private readonly Random _random;

        public MapGenerator(Canvas canvas, double hexSize)
        {
            _canvas = canvas;
            _hexSize = hexSize;
            _random = new Random();
            _hexRadius = GetHexRadius(_hexSize);
            _hexGap = GetHexGap(_hexSize);
        }
        #region GetHexRadius
        private double GetHexRadius(double size)
        {
            return size / 2;
        }
        #endregion
        #region GetHexGap
        private double GetHexGap(double size)
        {
            return size / 4;
        }
        #endregion
        #region GenerateMapChips
        public void GenerateMapChips(string[] mapChipFiles)
        {
            foreach (var file in mapChipFiles)
            {
                var bitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/" + file, UriKind.Absolute));
                var image = new Image { Source = bitmap, Height = 32, Width = 32 };

                // ランダムな座標に画像を配置する
                var x = _random.Next((int)_canvas.ActualWidth - (int)image.Width);
                var y = _random.Next((int)_canvas.ActualHeight - (int)image.Height);
                Canvas.SetLeft(image, x);
                Canvas.SetTop(image, y);

                _canvas.Children.Add(image);
            }
        }
        #endregion

        /// <summary>
        /// 現時点では内側のヘックスの線を消していない
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="colCount"></param>
        public void GenerateHexMap(int rowCount, int colCount, bool includeInnerHexLine)
        {
            double hexHeight = GetHexHeight(_hexRadius);
            double hexWidth = GetHexWidth(_hexRadius);

            Hexagon hexagon = new Hexagon(_hexRadius);

            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    double x = col * hexWidth * 3 / 4;
                    double y = row * (hexHeight + _hexGap);
                    if (col % 2 == 1)
                    {
                        y += hexHeight / 2 + _hexGap / 2;
                    }

                    var path = new Path
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1
                    };

                    var geometry = new PathGeometry();
                    var figure = new PathFigure
                    {
                        StartPoint = hexagon.GetPoint(0, x, y)
                    };
                    for (int i = 1; i <= 6; i++)
                    {
                        figure.Segments.Add(new LineSegment(hexagon.GetPoint(i, x, y), true));
                    }
                    geometry.Figures.Add(figure);
                    path.Data = geometry;

                    _canvas.Children.Add(path);
                }
            }
        }

        public class Hexagon
        {
            private readonly double _radius;
            private readonly double _height;
            private readonly double _width;

            public Hexagon(double radius)
            {
                _radius = radius;
                _height = 2 * radius;
                _width = Math.Sqrt(3) * radius;
            }

            public System.Windows.Point GetPoint(int index, double x, double y)
            {
                double angle = 2.0 * Math.PI / 6 * index;
                double x1 = _radius * Math.Cos(angle);
                double y1 = _radius * Math.Sin(angle);

                double absX = x + _width / 2 + x1;
                double absY = y + _height / 2 + y1;

                return new System.Windows.Point(absX, absY);
            }
        }
        #region GetHexHeight
        private double GetHexHeight(double radius)
        {
            return Math.Sqrt(3) * radius;
        }
        #endregion
        #region GetHexWidth
        private double GetHexWidth(double radius)
        {
            return 2 * radius;
        }
        #endregion

        public void GenerateFantasyMap(string[] mapChipFiles, int minLineCount, int maxLineCount)
        {
            // 六角形のマスを描画する
            GenerateHexMap(minLineCount, maxLineCount, true);

            // マップチップを配置する
            GenerateMapChips(mapChipFiles);
        }
    }
}
