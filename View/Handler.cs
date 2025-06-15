using System.Windows.Media;
using System.Windows.Shapes;

namespace PixelWallE.View
{
    public class Handler
    {
        public MainWindow Window { get; }

        public Handler(MainWindow window, WallE wallE) => Window = window;

        private void PlotLine(int x1, int y1)
        {
            var x0 = (int)Window.WallE.PositionX!;
            var y0 = (int)Window.WallE.PositionY!;
            if (Math.Abs(y1 - y0) < Math.Abs(x1 - x0))
            {
                if (x0 > x1)
                {
                    PlotLineLow(x1, y1, x0, y0);
                }
                else
                {
                    PlotLineLow(x0, y0, x1, y1);
                }
            }
            else
            {
                if (y0 > y1)
                {
                    PlotLineHigh(x1, y1, x0, y0);
                }
                else
                {
                    PlotLineHigh(x0, y0, x1, y1);
                }
            }
        }

        private void PlotLineHigh(int x1, int y1, int x0, int y0)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            var D = (2 * dx) - dy;
            var x = x0;

            for (int y = y0; y < y1; y++)
            {
                Plot(x, y);
                if (D > 0)
                {
                    x = x + xi;
                    D = D + (2 * (dx - dy));
                }
                else
                {
                    D = D + 2 * dx;
                }
            }
        }

        private void PlotLineLow(int x1, int y1, int x0, int y0)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var yi = 1;
            if (dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            var D = (2 * dy) - dx;
            var y = y0;

            for (int x = x0; x < x1; x++)
            {
                Plot(x, y);
                if (D > 0)
                {
                    y = y + yi;
                    D = D + (2 * (dy - dx));
                }
                else
                {
                    D = D + 2 * dy;
                }
            }
        }

        private void Plot(int x, int y)
        {
            Move(x, y);
            Draw();
        }
        public void DrawRectangle()
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            var brushSize = Window.GetWallEBrushSize();
            var mid = brushSize / 2;
            Rectangle rect = new()
            {
                Fill = Window.GetWallEBrushColor()
            };

            for (int i = 0; i < mid; i++)
            {
                Plot(x0, y0 + i);
            }
        }



        public Object CallFunction(string Name, Object[] @params) => Name switch
        {
            "GetActualX" => new Object(GetActualX()),
            "GetActualY" => new Object(GetActualY()),
            "GetCanvasSize" => new Object(GetCanvasSize()),
            "GetCanvasWidth" => new Object(GetCanvasWidth()),
            "GetCanvasHeight" => new Object(GetCanvasHeight()),
            "GetColorCount" => new Object(GetColorCount(@params[0].ToString(),
                                                        @params[1].ToInterger(),
                                                        @params[2].ToInterger(),
                                                        @params[3].ToInterger(),
                                                        @params[4].ToInterger())),
            _ => throw new NotImplementedException(),
        };
        public int GetActualX() => (int)Window.GetWallEPosX()!;
        public int GetActualY() => (int)Window.GetWallEPosY()!;
        public int GetCanvasSize()
            => Window.GetCanvasWidth() == Window.GetCanvasWidth()
            ? Window.GetCanvasSize()
            : throw new NotImplementedException();
        public int GetCanvasWidth() => Window.GetCanvasWidth();
        public int GetCanvasHeight() => Window.GetCanvasHeight();
        public int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            int a = 0;
            for (int i = x1; i < x2 + 1; i++)
            {
                for (int j = y1; j < y2 + 1; j++)
                {
                    if (Window.Rectangles[i, j] == ToBrush(color))
                    {
                        a++;
                    }
                }
            }
            return a;
        }

        #region Actions
        public void CallAction(string Name, Object[] @params)
        {
            switch (Name)
            {
                case "Spawn":
                    Spawn(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "Draw":
                    Draw();
                    break;
                case "Move":
                    Move(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "Plot":
                    Plot(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "Color":
                    this.Color(@params[0].ToString());
                    break;
                case "DrawLine":
                    DrawLine(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "DrawCircle":
                    DrawCircle(@params[0].ToInterger(), @params[1].ToInterger(), @params[2].ToInterger());
                    break;
                case "PlotCircle":
                    PlotCircle(@params[0].ToInterger(), @params[1].ToInterger(), @params[2].ToInterger());
                    break;
                case "DrawRectangle":
                    DrawRectangle(@params[0].ToInterger(), @params[1].ToInterger(), @params[2].ToInterger(), @params[3].ToInterger(), @params[4].ToInterger());
                    break;
                case "PlotRectangle":
                    PlotRectangle(@params[0].ToInterger(), @params[1].ToInterger(), @params[2].ToInterger(), @params[3].ToInterger());
                    break;
                case "Fill":

                    Fill(GetCurrentColor(GetActualX(), GetActualY()));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        private void Spawn(int x, int y)
        {
            Window.WallE.SetPos(x, y);
            Window.DrawWallE();
            Window.WallE.Show();
        }
        private void Draw()
        {
            Window.DrawPixel();
        }
        private void Move(int x, int y)
        {
            Window.WallE.SetPos(x, y);
            Window.DrawWallE();
        }
        private void Color(string color) => Window.WallE.ChangeBrush(ToBrush(color));
        private void DrawLine(int x1, int y1)
        {
            if (Window.WallE.isVisible
                && Window.WallE.PositionX is not null
                && Window.WallE.PositionY is not null)
            {
                PlotLine(x1, y1);
            }
            else
            {
                throw new Exception();
            }
        }
        private void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            if (IsValidDirection(dirX) && IsValidDirection(dirY))
            {
                var x0 = GetActualX() + dirX * distance;
                var y0 = GetActualX() + dirY * distance;
                PlotRectangle(x0, y0, width, height);
                Move(x0, y0);
            }
            throw new Exception();
        }
        private void DrawCircle(int dirX, int dirY, int radius)
        {
            if (Window.GetWallEVisibility())
            {
                var x0 = GetActualX() + dirX;
                var y0 = GetActualY() + dirY;
                PlotCircle(x0, y0, radius);
                Move(x0, y0);
            }
            else
            {
                throw new Exception();
            }
        }
        private void Fill(SolidColorBrush brush)
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            (int x, int y)[] vArr = [(1, 0), (0, 1), (-1, 0), (0, -1)];
            foreach (var (x, y) in vArr)
            {
                var X = x0 + x;
                var Y = y0 + y;
                var maxX = GetCanvasWidth();
                var maxY = GetCanvasHeight();
                if (IsInsideBounds(X, Y) && brush == GetCurrentColor(X, Y))
                {
                    Plot(X, Y);
                    Fill(brush);
                }
            }
        }
        #endregion

        public SolidColorBrush ToBrush(string color) => color switch
        {
            "Red" => Brushes.Red,
            "Blue" => Brushes.Blue,
            "Yellow" => Brushes.Yellow,
            "Green" => Brushes.Green,
            "Purple" => Brushes.Purple,
            "Orange" => Brushes.Orange,
            _ => throw new NotImplementedException(),
        };

        public bool TryGetErrFunction(string Name, Object[] @params, out Object result)
        {
            throw new NotImplementedException();
        }

        public bool TryGetErrAction(string Name, Object[] @params, SemanticErrVisitor visitor)
        {
            switch (Name)
            {
                case "Spawn":
                    return SpawnErr();
                case "Draw":
                    return DrawErr();
                case "Move":
                    return MoveErr();
                case "Color":
                    return ColorErr();
                default:
                    return false;
            }
        }

        private bool ColorErr()
        {
            if (Window.GetWallEVisibility())
            {
                return true;
            }
            return false;
        }

        private bool MoveErr()
        {
            if (Window.GetWallEVisibility())
            {
                return true;
            }
            return false;
        }

        private bool DrawErr()
        {
            if (!Window.GetWallEVisibility())
            {
                return true;
            }
            return false;
        }

        private bool SpawnErr()
        {
            if (Window.GetWallEVisibility())
            {
                return true;
            }
            Window.WallE.Show();
            return false;
        }

        private void PlotCircle(int x0, int y0, int radius)
        {
            radius--;
            var X = 0;
            var Y = -radius;
            while (X < -Y)
            {
                var yMid = Y + 0.5;
                if (X * X + yMid * yMid > radius * radius)
                {
                    Y += 1;
                }

                Plot(x0 + X, y0 + Y);
                Plot(x0 - X, y0 + Y);
                Plot(x0 + X, y0 - Y);
                Plot(x0 - X, y0 - Y);
                Plot(x0 + Y, y0 + X);
                Plot(x0 - Y, y0 + X);
                Plot(x0 + Y, y0 - X);
                Plot(x0 - Y, y0 - X);
                X += 1;
            }
        }

        private void PlotRectangle(int x0, int y0, int width, int height)
        {
            for (int i = y0; i < y0 + height; i++)
            {
                Plot(x0 + width - 1, i);
                Plot(x0 - width + 1, i);
            }
            for (int i = y0; i > y0 - height; i--)
            {
                Plot(x0 + width - 1, i);
                Plot(x0 - width + 1, i);
            }
            for (int i = x0; i < x0 + width; i++)
            {
                Plot(i, y0 + height - 1);
                Plot(i, y0 - height + 1);
            }
            for (int i = x0; i > x0 - width; i--)
            {
                Plot(i, y0 + height - 1);
                Plot(i, y0 - height + 1);
            }
        }
        private bool IsValidDirection(int dirX) 
            => dirX == 1 || dirX == -1 || dirX == 0;

        private SolidColorBrush GetCurrentColor(int x, int y) 
            => IsInsideBounds(x, y) ? Window.Rectangles[x, y] : throw new Exception();

        private bool IsInsideBounds(int X, int Y)
            => X > -1 && Y > -1 && X < GetCanvasWidth() && Y < GetCanvasHeight();


    }
}
