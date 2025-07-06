using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelWallE.View
{
    public class WallE
    {
        private int size = 1;
        private const int DEFAULTTHICKNESS = 1;
        public Image Image { get; } = new Image()
        {
            Source = new BitmapImage(new Uri("C:\\Users\\Audiovisual1\\Desktop\\285567672_130050706321124_2680004539479726113_n.jpg")),
        };
        public bool IsVisible { get; set; } = false;
        public int? PositionX { get; set; } = null;
        public int? PositionY { get; set; } = null;
        public int R { get; set; } = 0;
        public int G { get; set; } = 0;
        public int B { get; set; } = 0;
        public SolidColorBrush Brush { get; set;}
        public int Thickness
        {
            get { return size; }
            set
            {
                size = value - (value + 1) % 2;
            }
        }

        public WallE()
        {
            Brush = new SolidColorBrush(Color.FromRgb((byte)R,
                                                      (byte)G,
                                                      (byte)B));
            Thickness = DEFAULTTHICKNESS;
        }

        public void SetPos(int x, int y)
        {
            PositionX = x;
            PositionY = y;
        }

        public void Hide() => IsVisible = false;

        public void Show() => IsVisible = true;

        public void ChangeBrushFromRGB(int r, int g, int b)
        {
            R = r;
            G = g;
            B = b;
            Brush = new SolidColorBrush(Color.FromRgb((byte)R,
                                                      (byte)G,
                                                      (byte)B));
        }

        public void ChangeBrush(SolidColorBrush brush) => Brush = brush;

        public void Reset()
        {
            IsVisible = false;
            PositionX = null;
            PositionY = null;
            Thickness = DEFAULTTHICKNESS;
            Brush = Brushes.Black;
        }
    }
}
