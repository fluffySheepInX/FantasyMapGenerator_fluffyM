using System;
using System.Windows;

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
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    _map[x, y] = 0;
                }
            }
            // 大陸を生成
            int startX = mapWidth / 2; // 大陸の開始位置のx座標
            int startY = mapHeight / 2; // 大陸の開始位置のy座標
            int currentHeight = maxHeight; // 現在の高さ

            while (currentHeight >= MinHeight)
            {
                // ランダムな方向に高さを下げる
                //1から6までの整数をランダムに生成
                int direction = new Random().Next(1, 7);
                switch (direction)
                {
                    case 1: startX--; break;// 左
                    case 2: startY--; break;// 上
                    case 3: startX++; startY--; break;// 右上
                    case 4: startX++; break;// 右
                    case 5: startY++; break;// 下
                    case 6: startX--; startY++; break;// 左下
                }

                // マップの範囲内に収める
                startX = Math.Max(0, startX); // x座標が0未満にならないようにする
                startX = Math.Min(mapWidth - 1, startX);// x座標がmapWidth未満になるようにする
                startY = Math.Max(0, startY);// y座標が0未満にならないようにする
                startY = Math.Min(mapHeight - 1, startY); // y座標がmapHeight未満になるようにする

                // 高さを設定
                _map[startX, startY] = currentHeight;

                // 高さを下げる
                currentHeight--;

                // 一定の確率で高さを上げる
                if (new Random().NextDouble() < 0.3)// 30%の確率で
                {
                    // 最大高さを超えないように高さを上げる
                    currentHeight = Math.Min(maxHeight, currentHeight + 1);
                }
            }

            // 大陸と海の境界線を作成
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (_map[x, y] == 0)// 海の場合
                    {
                        bool hasLandNeighbor = false;
                        for (int dx = -1; dx <= 1; dx++) // 8方向をチェック
                        {
                            for (int dy = -1; dy <= 1; dy++)
                            {
                                if (dx == 0 && dy == 0) continue;// 自分自身はチェックしない
                                int nx = x + dx;
                                int ny = y + dy;
                                if (ny < 0 || ny >= mapHeight) continue;// マップの範囲外はチェックしない
                                if (nx < 0 || nx >= mapWidth || ny < 0 || ny >= mapHeight) continue;
                                if (_map[nx, ny] >= borderHeight)// 隣接するヘックスに大陸がある場合
                                {
                                    hasLandNeighbor = true;
                                    break;
                                }
                            }
                            // 隣接するヘックスに大陸がある場合、境界線に設定する
                            if (hasLandNeighbor)
                            {
                                _map[x, y] = borderHeight;
                            }
                        }
                    }
                }
            }

            // 水域を追加
            for (int i = 0; i < numLakes; i++)
            {
                int lakeX = new Random().Next(1, mapWidth - 1); // 1からmapWidth-1までのランダムなx座標
                int lakeY = new Random().Next(1, mapHeight - 1); // 1からmapHeight-1までのランダムなy座標
                if (_map[lakeX, lakeY] >= borderHeight)
                { // 大陸の上に水域を設定しないようにする
                    _map[lakeX, lakeY] = 0;
                }
            }

            // 地形を詳細化
            for (int i = 0; i < numHills; i++)
            {
                int hillX = new Random().Next(1, mapWidth - 1); // 1からmapWidth-1までのランダムなx座標
                int hillY = new Random().Next(1, mapHeight - 1); // 1からmapHeight-1までのランダムなy座標
                if (_map[hillX, hillY] > borderHeight)
                { // 大陸の上に丘を設定する
                    _map[hillX, hillY] += hillHeight; // 丘の高さを
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
