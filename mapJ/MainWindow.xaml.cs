using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace mapJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CreateWorld();
        }

        private void CreateWorld()
        {
            // すべてのヘックスを海に設定
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    _map[x, y] = 0;
                }
            }

            // 大陸を生成
            int startX = MapWidth / 2;
            int startY = MapHeight / 2;
            int currentHeight = MaxHeight;

            while (currentHeight >= MinHeight)
            {
                // ランダムな方向に高さを下げる
                int direction = new Random().Next(1, 7);
                switch (direction)
                {
                    case 1: startX--; break;
                    case 2: startY--; break;
                    case 3: startX++; startY--; break;
                    case 4: startX++; break;
                    case 5: startY++; break;
                    case 6: startX--; startY++; break;
                }

                // マップの範囲内に収める
                startX = Math.Max(0, startX);
                startX = Math.Min(MapWidth - 1, startX);
                startY = Math.Max(0, startY);
                startY = Math.Min(MapHeight - 1, startY);

                // 高さを設定
                _map[startX, startY] = currentHeight;

                // 高さを下げる
                currentHeight--;

                // 一定の確率で高さを上げる
                if (new Random().NextDouble() < 0.3)
                {
                    currentHeight = Math.Min(MaxHeight, currentHeight + 1);
                }
            }

            // 大陸と海の境界線を作成
            for (int x = 0; x < MapWidth; x++)
            {
                for (int y = 0; y < MapHeight; y++)
                {
                    if (_map[x, y] == 0)
                    {
                        bool hasLandNeighbor = false;
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;
                                int nx = x + dx;
                                int ny = y + dy;
                                if (ny < 0 || ny >= MapHeight) continue;
                                if (nx < 0 || nx >= MapWidth || ny < 0 || ny >= MapHeight) continue;
                                if (_map[nx, ny] >= BorderHeight)
                                {
                                    hasLandNeighbor = true;
                                    break;
                                }
                            }
                            if (hasLandNeighbor)
                            {
                                _map[x, y] = BorderHeight;
                            }
                        }
                    }
                }
            }

            // 水域を追加
            for (int i = 0; i < NumLakes; i++)
            {
                int lakeX = new Random().Next(1, MapWidth - 1);
                int lakeY = new Random().Next(1, MapHeight - 1);
                if (_map[lakeX, lakeY] >= BorderHeight)
                {
                    _map[lakeX, lakeY] = 0;
                }
            }

            // 地形を詳細化
            for (int i = 0; i < NumHills; i++)
            {
                int hillX = new Random().Next(1, MapWidth - 1);
                int hillY = new Random().Next(1, MapHeight - 1);
                if (_map[hillX, hillY] > BorderHeight)
                {
                    _map[hillX, hillY] += HillHeight;
                }
            }

            // マップを表示
            DrawMap();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mapCanvas.Children.Clear();

            CreateWorld();
        }
    }
}
