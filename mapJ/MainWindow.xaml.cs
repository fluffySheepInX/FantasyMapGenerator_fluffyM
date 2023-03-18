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
        private MapGenerator _mapGenerator;
        public int Scale { get; set; } = 0;

        public MainWindow()
        {
            InitializeComponent();
            MapCanvas.MouseWheel += grdCanvas_MouseWheel;
            _mapGenerator = new MapGenerator(MapCanvas, hexSize:60);
        }

        private void grdCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.LeftCtrl) == false) && (Keyboard.IsKeyDown(Key.RightCtrl) == false))
            {
                return;
            }

            ScaleTransform? scaleTransform = null;
            if (MapCanvas.RenderTransform == null)
            {
                scaleTransform = new ScaleTransform();
            }
            else
            {
                var tran = MapCanvas.RenderTransform as ScaleTransform;
                if (tran != null)
                {
                    scaleTransform = tran;
                }
                else
                {
                    scaleTransform = new ScaleTransform();
                }
            }

            double scale = 0;
            if (this.Scale > e.Delta)
            {
                this.Scale = this.Scale - 1;
                scale = scaleTransform.ScaleX - 0.1;
            }
            else if (this.Scale == e.Delta)
            {
                return;
            }
            else
            {
                this.Scale = this.Scale + 1;
                scale = scaleTransform.ScaleX + 0.1;
            }

            // 倍率を制限する（大きいとドラッグでバグることがあるけど、原因はよく分からず）
            if (scale > 2.0)
            {
                scale = 2.0; // 最大 200%
            }
            if (scale < 0.1)
            {
                scale = 0.1; // 最小 50%
            }
            scaleTransform.ScaleX = scale;
            scaleTransform.ScaleY = scale;
            MapCanvas.RenderTransform = scaleTransform; ;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MapCanvas.Children.Clear();

            string[] mapChipFiles = { "10005.png", "10010.png", "10006.png" };

            int minLineCount = 20;
            int maxLineCount = 20;

            _mapGenerator.GenerateFantasyMap(mapChipFiles, minLineCount, maxLineCount);
        }
    }
}
