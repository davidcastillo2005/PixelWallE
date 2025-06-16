using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Semantic;
using PixelWallE.View;

namespace PixelWallE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int canvasHeight;
        private int canvasWidth;
        private const double rectSize = 1;
        private double zoomFactor;
        private double lastValidZoomFactor;
        private readonly Handler handlers;

        public WallE WallE { get; set; }
        public Rectangle[,] Rectangles;

        public MainWindow()
        {
            InitializeComponent();
            WallE = new WallE();
            handlers = new Handler(this);
            zoomFactor = 1;
            UpdateLastValidZoomFactor(1);
            ShowCanvasSetupWindow();
            InitializeMainCanvas();
        }

        private void UpdateMainCanvasSize()
        {
            MainCanvas.Height = canvasHeight;
            MainCanvas.Width = canvasWidth;
        }

        private void InitializeMainCanvas()
        {
            if (canvasHeight != 0 || canvasWidth != 0)
                return;

            InitCanvas();
            MainCanvas.UpdateLayout();
        }

        private void ApplyZoom()
        {
            if (MainCanvas == null)
                return;
            if (MainCanvas.RenderTransform is ScaleTransform scaleTransform)
            {
                scaleTransform.ScaleX = zoomFactor;
                scaleTransform.ScaleY = zoomFactor;
            }
            else
            {
                MainCanvas.LayoutTransform = new ScaleTransform(zoomFactor, zoomFactor);
                MainCanvas.RenderTransformOrigin = new Point(0, 0);
            }

            UpdateLastValidZoomFactor(zoomFactor);

            MainCanvas.UpdateLayout();
            MainScrollViewer.UpdateLayout();
        }

        private void ShowCanvasSetupWindow()
        {
            NewCanvasSetupWindow newCanvasSetupWindow = new();

            bool? result = newCanvasSetupWindow.ShowDialog();
            if (result != true)
            {
                return;
            }
            canvasHeight = newCanvasSetupWindow.CanvasHeight;
            canvasWidth = newCanvasSetupWindow.CanvasWidth;

            UpdateMainCanvasSize();
        }

        private void ZoomTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            string[] subStrings = ZoomTextBox.Text.Split('%');
            string lastZoomFactorText = $"{lastValidZoomFactor * 100}";
            if (double.TryParse(subStrings[0], out double zoomPercent)
                || double.TryParse(ZoomTextBox.Text, out zoomPercent))
            {
                if (zoomPercent > 0)
                {
                    zoomFactor = zoomPercent * 0.01f;
                    ZoomTextBox.Text = zoomPercent.ToString() + "%";
                    ApplyZoom();
                }
                else
                {
                    MessageBox.Show("Zoom factor must be greater than 0.");
                    ZoomTextBox.Text = lastZoomFactorText + "%";
                }
            }
            else
            {
                MessageBox.Show("Invalid zoom factor.");
                ZoomTextBox.Text = lastZoomFactorText + "%";
            }
        }

        private void UpdateLastValidZoomFactor(double newZoomFactor)
            => lastValidZoomFactor = newZoomFactor;

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
            => Close();

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
            => ShowCanvasSetupWindow();

        public void DrawWallE()
        {
            if (!MainCanvas.Children.Contains(WallE.Image))
            {
                MainCanvas.Children.Add(WallE.Image);
            }
            WallE.Image.Width = rectSize;
            WallE.Image.Height = rectSize;
            Canvas.SetLeft(WallE.Image, GetWallEPosX() * rectSize);
            Canvas.SetTop(WallE.Image, GetWallEPosY() * rectSize);
        }

        public void DrawPixel(int x, int y)
        {
            var rect = Rectangles[x, y];
            var newBrush = (SolidColorBrush)GetWallEBrushColor();

            if (!rect.Fill.Equals(newBrush))
            {
                rect.Fill = newBrush;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Reset();
            var lexer = new Lexer();
            var parser = new Parser();
            var context = new Context(handlers);
            var semanticErr = new SemanticErrVisitor(context);
            var interpreter = new InterpreterVisitor(context);
            var ast = GetAST(lexer, parser);
            //try
            //{
            CheckErr(semanticErr, ast);
            if (semanticErr.Exceptions.Count != 0)
                PrintErr(semanticErr);
            else
                Execute(interpreter, ast);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + " : " + ex.Source + " : " + ex.StackTrace);
            //}
        }

        private void CheckErr(SemanticErrVisitor semanticErr, IStatement ast) => ast.Accept(semanticErr);

        private void Execute(InterpreterVisitor interpreter, IStatement ast)
        {
            Reset();
            ast.Accept(interpreter);
        }

        private void PrintErr(SemanticErrVisitor semanticErr)
        {
            foreach (var item in semanticErr.Exceptions)
            {
                string errMessage = item.PrintMessage();
                Paragraph paragraph = new();
                Run run = new(errMessage);
                paragraph.Inlines.Add(run);
                ProblemsTextBox.Document.Blocks.Add(paragraph);
            }
            ProblemsTextBox.ScrollToEnd();
        }

        private IStatement GetAST(Lexer lexer, Parser parser)
        {
            var code = ReadSourceCode();
            var tokens = lexer.Scan(code);
            var ast = parser.Parse(tokens);
            return ast;
        }

        private void InitCanvas()
        {
            WallE.Reset();
            MainCanvas.Children.Clear();
            ProblemsTextBox.Document.Blocks.Clear();
            Rectangles = new Rectangle[canvasWidth, canvasHeight];
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    var rect = new Rectangle
                    {
                        Height = rectSize,
                        Width = rectSize,
                        Fill = Brushes.White,
                    };
                    Rectangles[i, j] = rect;
                    MainCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, i * rectSize);
                    Canvas.SetTop(rect, j * rectSize);
                }
            }
        }

        private void Reset()
        {
            InitCanvas();
        }

        private string ReadSourceCode()
        {
            var start = SourceCode.Document.ContentStart;
            var end = SourceCode.Document.ContentEnd;
            var range = new TextRange(start, end);
            return range.Text;
        }

        public int GetCanvasSize()
        {
            if (canvasHeight == canvasWidth)
            {
                return canvasWidth;
            }
            throw new NotImplementedException();
        }

        public bool GetWallEVisibility() => WallE.isVisible;

        public int GetWallEPosX() => (int)WallE.PositionX!;

        public int GetWallEPosY() => (int)WallE.PositionY!;

        public int GetCanvasWidth() => canvasWidth;

        public int GetCanvasHeight() => canvasHeight;

        public int GetWallEBrushThickness() => WallE.Thickness;

        public Brush GetWallEBrushColor() => WallE.Brush;

        internal void ChangeBrushSize(int size)
        {
            WallE.Thickness = size;
        }
    }
}