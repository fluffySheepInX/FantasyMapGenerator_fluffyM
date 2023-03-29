using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace mapJ
{
    public partial class MainWindow : Window
    {
        private const int MapWidth = 50;
        private const int MapHeight = 50;
        private const int MaxHeight = 10;
        private const int MinHeight = 1;
        private const int BorderHeight = 2;
        private const int NumLakes = 5;
        private const int NumHills = 20;
        private const int HillHeight = 3;

        private int[,] _map = new int[MapWidth, MapHeight];


        private void DrawMap()
        {
            // キャンバスをクリア
            mapCanvas.Children.Clear();

            // マップを描画
            double hexSize = 10;
            double hexWidth = hexSize * Math.Sqrt(3);
            double hexHeight = hexSize * 2;

            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    double cx = hexWidth * x + ((y % 2 == 1) ? hexWidth / 2 : 0);
                    double cy = hexHeight * y * 0.75;
                    double height = _map[x, y];
                    Color color = GetColor((int)height);

                    Polygon hexagon = new Polygon();
                    hexagon.Points.Add(new Point(cx + hexSize * Math.Cos(0), cy + hexSize * Math.Sin(0)));
                    for (int i = 1; i <= 6; i++)
                    {
                        double angle = i * Math.PI / 3;
                        hexagon.Points.Add(new Point(cx + hexSize * Math.Cos(angle), cy + hexSize * Math.Sin(angle)));
                    }
                    hexagon.Fill = new SolidColorBrush(color);
                    hexagon.Stroke = Brushes.Black;
                    mapCanvas.Children.Add(hexagon);
                }
            }
        }

        private Color GetColor(int height)
        {
            if (height == 0)
            {
                return Colors.Blue;
            }
            else if (height < BorderHeight)
            {
                return Colors.LightBlue;
            }
            else if (height < MaxHeight - 1)
            {
                return Colors.Green;
            }
            else
            {
                return Colors.Gray;
            }
        }
    }
}
