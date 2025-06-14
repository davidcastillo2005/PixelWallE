using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixelWallE.View
{
    public class WallE
    {
        private int size = 1;

        public Image Image { get; } = new Image()
        {
            Source = new BitmapImage(new Uri("C:\\Users\\Audiovisual1\\Desktop\\285567672_130050706321124_2680004539479726113_n.jpg")),
        };
        public bool isVisible { get; set; } = false;
        public int? PositionX { get; set; } = null;
        public int? PositionY { get; set; } = null;
        public Brush Brush { get; set; } = Brushes.Black;
        public int BrushSize
        {
            get { return size; }
            set
            {
                size = value - (value + 1) % 2;
            }
        }

        public WallE() { }

        public void SetPos(int x, int y)
        {
            PositionX = x;
            PositionY = y;
        }

        public void Hide()
        {
            isVisible = false;
        }

        public void Show()
        {
            isVisible = true;
        }

        public void Reset()
        {
            isVisible = false;
            PositionX = null;
            PositionY = null;
            Brush = Brushes.White;
        }

        public void ChangeBrush(Brush brush)
        {
            Brush = brush;
        }
    }
}
