using Microsoft.Win32;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Semantic;
using PixelWallE.View;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PixelWallE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double RECTSIZE = 1;
        private double zoomFactor = 1;
        private double lastValidZoomFactor;
        private readonly Handler handler;
        private string? currentFilePath = null;

        public int CanvasHeight { get; set; }
        public int CanvasWidth { get; set; }
        public Rectangle[,] Rectangles = new Rectangle[0, 0];
        public WallE WallE { get; set; } = new WallE();
        public (int r, int g, int b) BackgroundColor { get; set; } = (255, 255, 255);

        public MainWindow()
        {
            InitializeComponent();
            handler = new Handler(this);
            bool? showCanvasSetupWindow = ShowCanvasSetupWindow();
            if (showCanvasSetupWindow is not null && (bool)showCanvasSetupWindow)
            {
                InitializeMainCanvas();
            }
            else
            {
                Close();
            }
        }

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            zoomFactor = GetDefaultZoom();
            ApplyZoom();
            UpdateLastValidZoomFactor(zoomFactor);
            ZoomTextBox.Text = (zoomFactor * 100).ToString() + "%";
        }

        private void CopyMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Copy();

        private void SelectAllMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.SelectAll();

        private void PasteMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Paste();

        private void RedoMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Redo();

        private void UndoMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Undo();

        private void CutMenuItem_Click(object sender, RoutedEventArgs e) => SourceCode.Cut();

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e) => OpenFile();

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e) => SaveAs();

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e) => Close();

        private void Button_Click(object sender, RoutedEventArgs e) => Run();

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            bool? showCanvasSetupWindow = ShowCanvasSetupWindow();
            if (showCanvasSetupWindow is not null && (bool)showCanvasSetupWindow)
            {
                Reset();
                zoomFactor = GetDefaultZoom();
                ApplyZoom();
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(currentFilePath))
            {
                SaveAsMenuItem_Click(sender, e);
                return;
            }
            SaveFile(currentFilePath);
        }

        private void ZoomTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            string[] subStrings = ZoomTextBox.Text.Split('%');
            string lastZoomFactorText = $"{lastValidZoomFactor * 100}";
            if ((double.TryParse(subStrings[0], out double zoomPercent)
                || double.TryParse(ZoomTextBox.Text, out zoomPercent))
                && (zoomPercent > 0))
            {
                zoomFactor = zoomPercent * 0.01f;
                ZoomTextBox.Text = zoomPercent.ToString() + "%";
                ApplyZoom();
            }
            else
            {
                InvalidZoom(lastZoomFactorText);
            }
        }

        #endregion

        #region Zoom

        private void InvalidZoom(string lastZoomFactorText)
        {
            MessageBox.Show("Invalid zoom factor.");
            ZoomTextBox.Text = lastZoomFactorText + "%";
        }

        private void UpdateLastValidZoomFactor(double newZoomFactor)
            => lastValidZoomFactor = newZoomFactor;

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

                if (CanvasWidth == 0
                    || CanvasHeight == 0
                    || availableWidth == 0
                    || availableHeight == 0)
                    return 1;

                double scaleX = availableWidth / CanvasWidth;
                double scaleY = availableHeight / CanvasHeight;
                return Math.Min(scaleX, scaleY);
            }
            return 1;
        }

        #endregion

        #region Canvas

        public int GetCanvasSize()
        {
            if (CanvasHeight == CanvasWidth)
            {
                return CanvasWidth;
            }
            throw new Exception();
        }

        private void UpdateMainCanvasSize()
        {
            MainCanvas.Height = CanvasHeight;
            MainCanvas.Width = CanvasWidth;
        }

        private void InitializeMainCanvas()
        {
            if (CanvasHeight == 0 || CanvasWidth == 0)
            {
                return;
            }
            Reset();
            MainCanvas.UpdateLayout();
        }

        public void DrawPixel(int x0, int y0, int size)
        {
            int offset = (size - 1) / 2;
            if (offset < 0)
                offset = -offset;
            if (offset < 1)
            {
                if (!IsInsideBounds(x0, y0))
                {
                    return;
                }
                var rect = Rectangles[x0, y0];
                if (!rect.Fill.Equals(WallE.Brush))
                {
                    rect.Fill = WallE.Brush;
                }
            }
            else
            {
                for (int i = -offset; i <= offset; i++)
                {
                    for (int j = -offset; j <= offset; j++)
                    {
                        var X = x0 + i;
                        var Y = y0 + j;
                        if (!IsInsideBounds(X, Y))
                        {
                            continue;
                        }
                        var rect = Rectangles[X, Y];
                        if (!rect.Fill.Equals(WallE.Brush))
                        {
                            rect.Fill = WallE.Brush;
                        }
                    }
                }
            }
        }

        public bool IsInsideBounds(int x, int y) 
            => x > -1 && y > -1 && x < CanvasWidth && y < CanvasHeight;

        private void CreateCanvasRectangles()
        {
            Rectangles = new Rectangle[CanvasWidth, CanvasHeight];
            for (int i = 0; i < CanvasWidth; i++)
            {
                for (int j = 0; j < CanvasHeight; j++)
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

        #endregion

        #region Preview

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
            CanvasHeight = newCanvasSetupWindow.CanvasHeight;
            CanvasWidth = newCanvasSetupWindow.CanvasWidth;

            UpdateMainCanvasSize();
            return result;
        }

        #endregion

        #region Execution

        private void Run()
        {
            Reset();
            List<Problem> problems = [];
            string sourceCode = ReadSourceCode();

            var lexer = new Lexer(sourceCode);
            Token[] tokens = lexer.Scan();
            if (lexer.Problems.Count != 0)
            {
                problems.AddRange(lexer.Problems);
            }

            var parser = new Parser(tokens);
            IStatement ast = parser.Parse();
            if (parser.Problems.Count != 0)
            {
                problems.AddRange(parser.Problems);
            }

            var context = new Context(handler);
            var semanticDectector = new SemanticVisitor(context);
            ast.Accept(semanticDectector);
            if (semanticDectector.SemanticProblems.Count != 0)
            {
                problems.AddRange(semanticDectector.SemanticProblems);
            }
            else
            {
                Reset();
                var interpreter = new InterpreterVisitor(context);
                ast.Accept(interpreter);
            }
            Sort(problems);
            PrintProblems(problems);
        }

        private void Sort(List<Problem> problems)
        {
            if (problems.Count == 0)
            {
                return;
            }
            for (int i = 1; i < problems.Count; i++)
            {
                if (problems[i].Row < problems[i - 1].Row || problems[i].Column < problems[i - 1].Column)
                {
                    var temp = problems[i];
                    problems[i] = problems[i - 1];
                    problems[i - 1] = temp;
                }
            }
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

        #endregion

        #region Terminal

        public void PrintOutput(string text)
        {
            Paragraph paragraph = new();
            Run run = new(text);
            paragraph.Inlines.Add(run);
            OutputTextBox.Document.Blocks.Add(paragraph);
        }

        private void PrintProblems(List<Problem> Errors)
        {
            foreach (Problem item in Errors)
            {
                string errMessage = item.PrintMessage();
                Paragraph paragraph = new();
                Run run = new(errMessage);
                paragraph.Inlines.Add(run);
                ProblemsTextBox.Document.Blocks.Add(paragraph);
            }
            ProblemsTextBox.ScrollToEnd();
        }

        #endregion

        #region WallE

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

        public int GetWallEPosX() => (int)WallE.PositionX!;

        public int GetWallEPosY() => (int)WallE.PositionY!;

        #endregion

        #region File

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
                SaveFile(saveFileDialog.FileName);
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

        private void SaveFile(string filePath)
        {
            var start = SourceCode.Document.ContentStart;
            var end = SourceCode.Document.ContentEnd;
            var range = new TextRange(start, end);
            File.WriteAllText(filePath, range.Text);
            currentFilePath = filePath;
        }

        #endregion
    }
}