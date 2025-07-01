using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
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
        private const double RECTSIZE = 1;
        private double zoomFactor = 1;
        private double lastValidZoomFactor;
        private readonly Handler handler;
        private string? currentFilePath = null;

        public WallE WallE { get; set; } = new WallE();
        public Rectangle[,] Rectangles = new Rectangle[0, 0];

        public MainWindow()
        {
            InitializeComponent();
            handler = new Handler(this);
            bool? showCanvasSetupWindow = ShowCanvasSetupWindow();
            if (showCanvasSetupWindow is not null && (bool)showCanvasSetupWindow)
            {
                InitializeMainCanvas();
                zoomFactor = GetDefaultZoom();
                ApplyZoom();
            }
            else
            { 
                Close(); 
            }
        }

        #region Clicks

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Copy();

        private void SelectAllMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.SelectAll();

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Paste();

        private void RedoMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Redo();

        private void UndoMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Undo();

        private void CutMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Cut();

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e) => OpenFile();

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e) => SaveAs();

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Close();
        
        #endregion

        private double GetDefaultZoom()
        {
            if (MainScrollViewer != null)
            {
                double availableWidth = MainScrollViewer.ViewportWidth;
                double availableHeight = MainScrollViewer.ViewportHeight;

                if (availableWidth == 0 || availableHeight == 0)
                {
                    availableWidth = MainScrollViewer.ActualWidth;
                    availableHeight = MainScrollViewer.ActualHeight;
                }

                if (canvasWidth == 0 
                    || canvasHeight == 0 
                    || availableWidth == 0 
                    || availableHeight == 0)
                    return 1;

                double scaleX = availableWidth / canvasWidth;
                double scaleY = availableHeight / canvasHeight;
                return Math.Min(scaleX, scaleY);
            }
            return 1;
        }

        private void UpdateMainCanvasSize()
        {
            MainCanvas.Height = canvasHeight;
            MainCanvas.Width = canvasWidth;
        }

        private void InitializeMainCanvas()
        {
            if (canvasHeight == 0 || canvasWidth == 0)
            {
                return;
            }

            Reset();
            MainCanvas.UpdateLayout();
        }

        private void ApplyZoom()
        {
            if (MainCanvas == null)
                return;
            if (MainCanvas.LayoutTransform is ScaleTransform scaleTransform)
            {
                scaleTransform.ScaleX = zoomFactor;
                scaleTransform.ScaleY = zoomFactor;
            }
            else
            {
                MainCanvas.LayoutTransform = new ScaleTransform(zoomFactor, zoomFactor);
            }

            UpdateLastValidZoomFactor(zoomFactor);

            MainCanvas.UpdateLayout();
            MainScrollViewer.UpdateLayout();
        }

        private bool? ShowCanvasSetupWindow()
        {
            NewCanvasSetupWindow newCanvasSetupWindow = new();

            bool? result = newCanvasSetupWindow.ShowDialog();
            if (result != true)
            {
                return result;
            }
            if (newCanvasSetupWindow.CanvasHeight <= 0 || newCanvasSetupWindow.CanvasWidth <= 0)
            {
                return false;
            }
            canvasHeight = newCanvasSetupWindow.CanvasHeight;
            canvasWidth = newCanvasSetupWindow.CanvasWidth;

            UpdateMainCanvasSize();
            return result;
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

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool? showCanvasSetupWindow = ShowCanvasSetupWindow();
            if (showCanvasSetupWindow is not null && (bool)showCanvasSetupWindow)
            {
                Reset();
            }
        }

        public void DrawWallE()
        {
            if (!MainCanvas.Children.Contains(WallE.Image))
            {
                MainCanvas.Children.Add(WallE.Image);
            }
            WallE.Image.Width = RECTSIZE;
            WallE.Image.Height = RECTSIZE;
            Canvas.SetLeft(WallE.Image, GetWallEPosX() * RECTSIZE);
            Canvas.SetTop(WallE.Image, GetWallEPosY() * RECTSIZE);
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
            var context = new Context(handler);
            var semanticErr = new SemanticErrVisitor(context);
            var interpreter = new InterpreterVisitor(context);
            var ast = GetAST(lexer, parser);

            try
            {
                CheckErr(semanticErr, ast);
                if (semanticErr.Exceptions.Count != 0)
                    PrintErr(semanticErr);
                else
                    Execute(interpreter, ast);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " : " + ex.Source + " : " + ex.StackTrace);
            }
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

        private void Reset()
        {
            UpdateMainCanvasSize();
            WallE.Reset();
            MainCanvas.Children.Clear();
            OutputTextBox.Document.Blocks.Clear();
            ProblemsTextBox.Document.Blocks.Clear();
            CreateCanvasRectangles();
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

        internal void ChangeBrushSize(int size) => WallE.Thickness = size;

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAsMenuItem_Click(sender, e);
                return;
            }
            SaveToFile(currentFilePath);
        }

        private void SaveAs()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                Filter = "PixelWallE Files (*.pw)|*.pw|All Files (*.*)|*.*",
                DefaultExt = ".pw",
                AddExtension = true
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveToFile(saveFileDialog.FileName);
            }
        }

        private bool OpenFile()
        {
            var openFileDialog = new OpenFileDialog()
            {
                Filter = "PixelWallE Files (*.pw)|*.pw|All Files (*.*)|*.*",
                DefaultExt = ".pw",
                AddExtension = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string text = File.ReadAllText(openFileDialog.FileName);
                SourceCode.Document.Blocks.Clear();
                SourceCode.Document.Blocks.Add(new Paragraph(new Run(text)));
                currentFilePath = openFileDialog.FileName;
                return true;
            }
            return false;
        }

        private void SaveToFile(string filePath)
        {
            var start = SourceCode.Document.ContentStart;
            var end = SourceCode.Document.ContentEnd;
            var range = new TextRange(start, end);
            File.WriteAllText(filePath, range.Text);
            currentFilePath = filePath;
        }

        private void CreateCanvasRectangles()
        {
            Rectangles = new Rectangle[canvasWidth, canvasHeight];
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    var rect = new Rectangle
                    {
                        Height = RECTSIZE,
                        Width = RECTSIZE,
                        Fill = Brushes.White,
                    };
                    Rectangles[i, j] = rect;
                    MainCanvas.Children.Add(rect);
                    Canvas.SetLeft(rect, i * RECTSIZE);
                    Canvas.SetTop(rect, j * RECTSIZE);
                }
            }
        }

        public void Print(string text)
        {
            Paragraph paragraph = new();
            Run run = new(text);
            paragraph.Inlines.Add(run);
            OutputTextBox.Document.Blocks.Add(paragraph);
        }
    }
}