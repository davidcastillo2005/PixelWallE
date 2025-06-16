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
using System.Windows.Shapes;

namespace PixelWallE
{
    /// <summary>
    /// Interaction logic for NewCanvasSetupWindow.xaml
    /// </summary>
    public partial class NewCanvasSetupWindow : Window
    {
        public int CanvasHeight { get; private set; }
        public int CanvasWidth { get; private set; }

        public NewCanvasSetupWindow()
        {
            InitializeComponent();
            CanvasWidth = 0;
            CanvasHeight = 0;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanvasHeight == 0 || CanvasWidth == 0)
            {
                MessageBox.Show("Invalid width or invalid height.");
                return;
            }
            DialogResult = true;
            Close();
        }

        private void UpdatePreviewCanvas(double height, double width)
        {
            if (PreviewBorder == null
                || PreviewCanvas == null
                || width == 0
                || height == 0)
                return;

            double aspectRatio = (double)height / width;
            double newHeight, newWidth;
            if (aspectRatio >= 1)
            {
                newHeight = GetPreviewMaxSize();
                newWidth = GetPreviewMaxSize() / aspectRatio;
            }
            else
            {
                newWidth = GetPreviewMaxSize();
                newHeight = GetPreviewMaxSize() * aspectRatio;
            }

            PreviewCanvas.Height = newHeight;
            PreviewCanvas.Width = newWidth;
        }

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
            => UpdatePreviewCanvas(GetPreviewMaxSize(), GetPreviewMaxSize());

        private double GetPreviewMaxSize() => PreviewBorder.ActualHeight * 0.75;

        private void HeightTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string str = HeightTextBox.Text;
            if (!int.TryParse(str, out int height)
                || height <= 0)
            {
                return;
            }
            CanvasHeight = height;
            UpdatePreviewCanvas(CanvasHeight, CanvasWidth);
        }

        private void WidthTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string str = WidthTextBox.Text;
            if (!int.TryParse(str, out int width)
                || width <= 0)
            {
                return;
            }
            CanvasWidth = width;
            UpdatePreviewCanvas(CanvasHeight, CanvasWidth);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
