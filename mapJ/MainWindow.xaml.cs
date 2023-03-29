using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace mapJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
                int direction = new Random(DateTime.Now.Millisecond).Next(1, 7);
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
                if (new Random(DateTime.Now.Millisecond).NextDouble() < 0.3)// 30%の確率で
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
                int lakeX = new Random(DateTime.Now.Millisecond).Next(1, mapWidth - 1); // 1からmapWidth-1までのランダムなx座標
                int lakeY = new Random(DateTime.Now.Millisecond).Next(1, mapHeight - 1); // 1からmapHeight-1までのランダムなy座標
                if (_map[lakeX, lakeY] >= borderHeight)
                { // 大陸の上に水域を設定しないようにする
                    _map[lakeX, lakeY] = 0;
                }
            }

            // 地形を詳細化
            for (int i = 0; i < numHills; i++)
            {
                int hillX = new Random(DateTime.Now.Millisecond).Next(1, mapWidth - 1); // 1からmapWidth-1までのランダムなx座標
                int hillY = new Random(DateTime.Now.Millisecond).Next(1, mapHeight - 1); // 1からmapHeight-1までのランダムなy座標
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
