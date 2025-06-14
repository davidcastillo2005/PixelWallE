using System.Windows.Media;
using PixelWallE.Interfaces;
using PixelWallE.SourceCodeAnalisis.Semantic.Visitors;

namespace PixelWallE.View
{
    public class Handler : IHandle
    {
        public MainWindow Window { get; }
        public WallE WallE;
        public Handler(MainWindow window, WallE wallE)
        {
            this.WallE = wallE;
            Window = window;
        }
        public int GetActualX() => (int)Window.WallE.PositionX!;

        public int GetActualY() => (int)Window.WallE.PositionY!;

        public int GetCanvasSize()
            => Window.GetCanvasWidth() == Window.GetCanvasWidth()
            ? Window.GetCanvasSize()
            : throw new NotImplementedException();

        public void Spawn(int x, int y)
        {
            Window.WallE.SetPos(x, y);
            Window.DrawWallE();
            Window.WallE.Show();
        }

        public void DrawLine(int x1, int y1)
        {
            if (Window.WallE.isVisible && Window.WallE.PositionX is not null && Window.WallE.PositionY is not null)
            {
                PlotLine(x1, y1);
            }
            else
            {
                throw new Exception();
            }
        }

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

        public void Draw()
        {
            Window.DrawPixel();
            Window.Rectangles[GetActualX(), GetActualY()] = (SolidColorBrush)WallE.Brush;
        }

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

        public Object CallFunction(string Name, Object[] @params) => Name switch
        {
            "GetActualX" => new Object(GetActualX()),
            "GetActualY" => new Object(GetActualY()),
            "GetCanvasSize" => new Object(GetCanvasSize()),
            "GetColorCount" => new Object(GetColorCount(@params[0].ToString(),
                                                        @params[1].ToInterger(),
                                                        @params[2].ToInterger(),
                                                        @params[3].ToInterger(),
                                                        @params[4].ToInterger())),

            _ => throw new NotImplementedException(),
        };

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
                case "Color":
                    Color(@params[0].ToString());
                    break;
                case "DrawLine":
                    DrawLine(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "DrawEllipse":
                    DrawEllipse(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void DrawEllipse(int radiusX, int radiusY)
        {
            if (Window.WallE.isVisible && Window.WallE.PositionX is not null && Window.WallE.PositionY is not null)
            {
                MidpointAlgorithm(radiusX, radiusY);
            }
            else
            {
                throw new Exception();
            }
        }

        private void MidpointAlgorithm(int radiusX, int radiusY)
        {
            var x0 = (int)Window.WallE.PositionX!;
            var y0 = (int)Window.WallE.PositionY!;

            var x = 0;
            var y = radiusY;

            var p1 = (radiusY * radiusY) - (radiusX * radiusX * radiusY) + (0.25f * radiusX * radiusX);
            var dx = 2 * radiusY * radiusY * x;
            var dy = 2 * radiusX * radiusX * y;

            while (dx < dy)
            {
                Plot(x0 + x, y0 + y);
                Plot(x0 - x, y0 + y);
                Plot(x0 + x, y0 - y);
                Plot(x0 - x, y0 - y);

                if (p1 < 0)
                {
                    x += 1;
                    dx += 2 * radiusY * radiusY;
                    p1 += dx + radiusY * radiusY;
                }
                else
                {
                    x += 1;
                    y -= 1;
                    dx += 2 * radiusX * radiusX;
                    dy -= 2 * radiusY * radiusY;
                    p1 += dx - dy + radiusY * radiusY;
                }
            }
        }

        private void Color(string color)
        {
            WallE.ChangeBrush(ToBrush(color));
        }

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

        private void Move(int x, int y)
        {
            WallE.SetPos(x, y);
            Window.DrawWallE();
        }

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
            if (WallE is not null && WallE.isVisible)
            {
                return true;
            }
            return false;
        }

        private bool MoveErr()
        {
            if (WallE is not null && WallE.isVisible)
            {
                return true;
            }
            return false;
        }

        private bool DrawErr()
        {
            if (WallE is not null
                && !WallE.isVisible
                && WallE.PositionX is not null
                && WallE.PositionY is not null)
            {
                return true;
            }
            return false;
        }

        private bool SpawnErr()
        {
            if (Window.WallE.isVisible)
            {
                return true;
            }
            Window.WallE.Show();
            return false;
        }
    }
}
