using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace mapJ
{
    public partial class MainWindow : Window
    {
        // マップのサイズやパラメータを定義
        const int mapWidth = 50; // マップの横幅
        const int mapHeight = 50; // マップの縦幅
        const int maxHeight = 10; // 大陸の最大高さ
        const int minHeight = 1; // 大陸の最小高さ
        const int borderHeight = 2; // 大陸と海の境界の高さ
        const int numLakes = 5; // 水域の数
        const int numHills = 20; // 丘の数
        const int hillHeight = 3; // 丘の高さ

        // マップを格納する2次元配列を初期化
        private int[,] _map = new int[mapWidth, mapHeight];


        private void DrawMap()
        {
            // キャンバスをクリア
            mapCanvas.Children.Clear();

            // マップを描画
            double hexSize = 10;
            double hexWidth = hexSize * Math.Sqrt(3);
            double hexHeight = hexSize * 2;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
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
            else if (height < borderHeight)
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
