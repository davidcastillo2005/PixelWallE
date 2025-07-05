using System.Windows.Media;

namespace PixelWallE.View
{
    public class Handler(MainWindow window)
    {
        public MainWindow Window { get; } = window;

        private Dictionary<string, SolidColorBrush> stringToBrush = new()
        {
            {"White", Brushes.White},
            {"Black", Brushes.Black },
            {"Red", Brushes.Red },
            {"Blue", Brushes.Blue },
            {"Yellow", Brushes.Yellow },
            {"Purple", Brushes.Purple },
            {"Green", Brushes.Green },
            {"Orange", Brushes.Orange },
        };

        #region Functions

        public DynamicValue CallFunction(string Name, DynamicValue[] @params) => Name switch
        {
            "GetActualX" or "getactualx" => new DynamicValue(GetActualX()),
            "GetActualY" or "getactualy" => new DynamicValue(GetActualY()),
            "GetCanvasSize" or "getcanvassize" => new DynamicValue(GetCanvasSize()),
            "GetCanvasWidth" or "getcanvaswidth" => new DynamicValue(GetCanvasWidth()),
            "GetCanvasHeight" or "getcanvasheight" => new DynamicValue(GetCanvasHeight()),
            "GetBrushSize" or "getbrushsize" => new DynamicValue(GetBrushSize()),
            "GetColorCount" or "getcolorcount" => new DynamicValue(GetColorCount(@params[0].ToString(),
                                                        @params[1].ToInt(),
                                                        @params[2].ToInt(),
                                                        @params[3].ToInt(),
                                                        @params[4].ToInt())),
            "IsBrushColor" or "isbrushcolor" => new DynamicValue(IsBrushColor(@params[0].ToString())),
            "IsBrushSize" or "isbrushsize" => new DynamicValue(IsBrushSize(@params[0].ToInt())),
            "IsCanvascolor" or "iscanvascolor" => new DynamicValue(IsCanvasColor(@params[0].ToString(),
                                                        @params[1].ToInt(),
                                                        @params[2].ToInt())),
            _ => throw new Exception(),
        };

        private int GetActualX() => Window.GetWallEPosX()!;

        private int GetActualY() => Window.GetWallEPosY()!;

        private int GetCanvasSize()
            => Window.GetCanvasWidth() == Window.GetCanvasWidth()
            ? Window.GetCanvasSize()
            : throw new Exception();

        private int GetCanvasWidth() => Window.GetCanvasWidth();

        private int GetCanvasHeight() => Window.GetCanvasHeight();

        private int GetBrushSize() => Window.WallE.Thickness;

        private int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            int a = 0;
            for (int i = x1; i < x2 + 1; i++)
            {
                for (int j = y1; j < y2 + 1; j++)
                {
                    if (Window.Rectangles[i, j].Fill.Equals(stringToBrush[color]))
                    {
                        a++;
                    }
                }
            }
            return a;
        }

        private int IsBrushColor(string color)
        {
            var brushColor = Window.WallE.Brush;
            return stringToBrush[color] == brushColor ? 1 : 0;
        }

        private int IsBrushSize(int size)
        {
            var brushThickness = Window.WallE.Thickness;
            return brushThickness == size ? 1 : 0;
        }

        private int IsCanvasColor(string color, int v2, int v3)
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            if (!IsInsideBounds(x0 + v2, y0 + v3))
            {
                return 0;
            }
            var fill = Window.Rectangles[x0 + v2, y0 + v3].Fill;
            return stringToBrush[color] == fill ? 1 : 0;
        }

        #endregion

        #region ErrFunctions

        public bool TryGetErrFunction(string identifier, DynamicValue[] @params, SemanticVisitor visitor, Coord coord, out DynamicValue @return)
        {
            switch (identifier)
            {
                case "GetActualX" or "getactualx":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out bool b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetActualXErr(visitor, coord);
                case "GetActualY" or "getactualy":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetActualYErr(visitor, coord);
                case "GetCanvasSize" or "getcanvassize":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetCanvasSizeErr(visitor, coord);
                case "GetCanvasWidth" or "getcanvaswidth":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetCanvasWidthErr(visitor, coord);
                case "GetCanvasHeight" or "getcanvasheight":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetCanvasHeightErr(visitor, coord);
                case "GetBrushSize" or "getbrushsize":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetBrushSizeErr();
                case "GetColorCount" or "getcolorcount":
                    AddMissArgErr(identifier, @params, visitor, coord, 5, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return GetColorCountErr(@params[0].ToString(),
                                            @params[1].ToInt(),
                                            @params[2].ToInt(),
                                            @params[3].ToInt(),
                                            @params[4].ToInt(),
                                            visitor,
                                            coord);
                case "IsBrushColor" or "isbrushcolor":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return IsBrushColorErr(@params[0].ToString(),
                                           visitor,
                                           coord);
                case "IsBrushSize" or "isbrushsize":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    @return = visitor.CheckDynamicValue(coord, null);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return IsBrushSizeErr(@params[0].ToInt(),
                                          visitor,
                                          coord);
                case "IsCanvascolor" or "iscanvascolor":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    @return = visitor.CheckDynamicValue(coord, null);
                    if (b)
                    {
                        @return = visitor.CheckDynamicValue(coord, null);
                        return b;
                    }
                    @return = visitor.CheckDynamicValue(coord, new DynamicValue(0));
                    return IsCanvasColorErr(@params[0].ToString(),
                                            @params[1].ToInt(),
                                            @params[2].ToInt(),
                                            visitor,
                                            coord);
                default:
                    visitor.SemanticProblems.Add(new Error(coord, $"Invalid '{identifier}' function"));
                    @return = visitor.CheckDynamicValue(coord, null);
                    return false;
            }
        }

        private bool GetBrushSizeErr() => false;

        private bool IsCanvasColorErr(string color, int v2, int v3, SemanticVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Unsupported '{color}' color"));
                return true;
            }
            return false;
        }

        private bool IsBrushSizeErr(int size, SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (size < 1) return true;
            return false;
        }

        private bool IsBrushColorErr(string color, SemanticVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Unsupported '{color}' color"));
                return true;
            }

            return false;
        }

        private bool GetColorCountErr(string color, int x0, int y0, int x1, int y1, SemanticVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Unsupported '{color}' color"));
                return true;
            }

            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x1}, {y1}> position"));
                return true;
            }

            return false;
        }

        private bool GetCanvasHeightErr(SemanticVisitor visitor, Coord coord)
        {
            return true;
        }

        private bool GetCanvasWidthErr(SemanticVisitor visitor, Coord coord) => false;

        private bool GetCanvasSizeErr(SemanticVisitor visitor, Coord coord)
        {
            if (Window.GetCanvasWidth() == Window.GetCanvasWidth())
            {
                return false;
            }

            visitor.SemanticProblems.Add(new Error(coord, "Canvas width does not match canvas height"));
            return true;
        }

        private bool GetActualYErr(SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (Window.WallE.PositionY is null)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Null Wall-E position"));
                return true;
            }

            return false;
        }

        private bool GetActualXErr(SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (Window.WallE.PositionX is null)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Null Wall-E position"));
                return true;
            }

            return false;
        }

        #endregion

        #region Actions

        public void CallAction(string Name, DynamicValue[] @params, Coord coord)
        {
            switch (Name)
            {
                case "Spawn" or "spawn":
                    Spawn(@params[0].ToInt(), @params[1].ToInt());
                    break;
                case "Draw" or "draw":
                    Draw();
                    break;
                case "Plot" or "plot":
                    Plot(@params[0].ToInt(), @params[1].ToInt());
                    break;
                case "Move" or "move":
                    Move(@params[0].ToInt(), @params[1].ToInt());
                    break;
                case "Size" or "size":
                    Size(@params[0].ToInt());
                    break;
                case "Color" or "color":
                    Color(@params[0].ToString());
                    break;
                case "ColorRGB" or "colorrgb":
                    ColorRGB(@params[0].ToInt(),
                                @params[1].ToInt(),
                                @params[2].ToInt());
                    break;
                case "DrawLine" or "drawline":
                    DrawLine(@params[0].ToInt(), @params[1].ToInt(), @params[2].ToInt());
                    break;
                case "PlotLine" or "plotline":
                    PlotLine(@params[0].ToInt(), @params[1].ToInt(), @params[2].ToInt(), @params[3].ToInt());
                    break;
                case "DrawCircle" or "drawcircle":
                    DrawCircle(@params[0].ToInt(), @params[1].ToInt(), @params[2].ToInt());
                    break;
                case "PlotCircle" or "plotcircle":
                    PlotCircle(@params[0].ToInt(), @params[1].ToInt(), @params[2].ToInt());
                    break;
                case "DrawRectangle" or "drawrectangle":
                    DrawRectangle(@params[0].ToInt(), @params[1].ToInt(), @params[2].ToInt(), @params[3].ToInt(), @params[4].ToInt());
                    break;
                case "PlotRectangle" or "plotrectangle":
                    PlotRectangle(@params[0].ToInt(), @params[1].ToInt(), @params[2].ToInt(), @params[3].ToInt());
                    Move(@params[0].ToInt(), @params[1].ToInt());
                    break;
                case "Fill" or "fill":
                    var x0 = GetActualX();
                    var y0 = GetActualY();
                    var currentSize = GetBrushSize();
                    var color = GetCurrentColor(x0, y0);
                    Size(1);
                    Plot(x0, y0);
                    Fill(color);
                    Move(x0, y0);
                    Size(currentSize);
                    break;
                case "Print" or "print":
                    TryParse(@params[0], out string? printedParam);
                    Print(printedParam!);
                    break;
                case "Erase" or "erase":
                    var currentColor = Window.WallE.Brush;
                    Erase();
                    Color(currentColor);
                    break;
                default:
                    throw new Exception();
            }
        }

        private void Color(SolidColorBrush currentColor)
        {
            Window.WallE.ChangeBrush(currentColor);
        }

        private void Erase()
        {
            ColorRGB(Window.BackgroundColor.r, Window.BackgroundColor.g, Window.BackgroundColor.b);
            Draw();
        }

        private void ColorRGB(int r, int g, int b)
        {
            Window.WallE.ColorRGB(r, g, b);
        }

        private void Spawn(int x, int y)
        {
            Window.WallE.SetPos(x, y);
            Window.DrawWallE();
            Window.WallE.Show();
        }

        private void Draw()
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            int offset = (Window.WallE.Thickness - 1) / 2;
            if (offset < 0)
                offset = -offset;
            if (offset < 1)
            {
                if (!IsInsideBounds(x0, y0))
                {
                    return;
                }
                Window.DrawPixel(x0, y0);
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
                        Window.DrawPixel(X, Y);
                    }
                }
            }
        }

        private void Plot(int x, int y)
        {
            Move(x, y);
            Draw();
        }

        private void Move(int x, int y)
        {
            Window.WallE.SetPos(x, y);
            Window.DrawWallE();
        }

        private void Color(string color) => Window.WallE.ChangeBrush(stringToBrush[color]);

        private void Size(int size) => Window.WallE.Thickness = size;

        private void DrawLine(int x1, int y1, int d)
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            var X = x0 + (x1 * d);
            var Y = y0 + (y1 * d);
            PlotLine(x0, y0, X, Y);
        }

        private void PlotLine(int x0, int y0, int x1, int y1)
        {
            var dx = x1 - x0;
            var dy = y1 - y0;
            var step = Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (step != 0)
            {
                var stepX = (double)dx / step;

                var stepY = (double)dy / step;
                for (int i = 0; i < step + 1; i++)
                {
                    Plot((int)Math.Round(x0 + (i * stepX)), (int)Math.Round(y0 + (i * stepY)));
                }
            }
        }

        private void DrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            var x0 = GetActualX() + (dirX * distance);
            var y0 = GetActualX() + (dirY * distance);
            PlotRectangle(x0, y0, width, height);
            Move(x0, y0);
        }

        private void PlotRectangle(int x0, int y0, int width, int height)
        {
            if (height < 0) height = -1 * height;
            if (width < 0) width = -1 * width;

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

        private void DrawCircle(int dirX, int dirY, int radius)
        {
            var x0 = GetActualX() + dirX;
            var y0 = GetActualY() + dirY;
            PlotCircle(x0, y0, radius);
            Move(x0, y0);
        }

        private void PlotCircle(int x0, int y0, int radius)
        {
            radius--;
            var X = 0;
            var Y = -radius;
            while (X < -Y)
            {
                var yMid = Y + 0.5;
                if ((X * X) + (yMid * yMid) > radius * radius)
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
            Move(x0, y0);
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
                if (!IsInsideBounds(X, Y) || brush != GetCurrentColor(X, Y))
                {
                    continue;
                }
                Plot(X, Y);
                Fill(brush);
            }
        }

        private void Print(string printedParam) => Window.PrintOutput(printedParam);

        #endregion

        #region ErrActions

        public bool TryGetErrAction(string identifier, DynamicValue[] @params, SemanticVisitor visitor, Coord coord)
        {
            switch (identifier)
            {
                case "Spawn" or "spawn":
                    AddMissArgErr(identifier, @params, visitor, coord, 2, out bool b);
                    if (b) return b;

                    return SpawnErr(@params[0].ToInt(),
                                    @params[1].ToInt(),
                                    visitor,
                                    coord);
                case "Draw" or "draw":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b) return b;

                    return DrawErr(visitor, coord);
                case "Move" or "move":
                    AddMissArgErr(identifier, @params, visitor, coord, 2, out b);
                    if (b) return b;

                    return MoveErr(@params[0].ToInt(),
                                   @params[1].ToInt(),
                                   visitor,
                                   coord);
                case "Color" or "color":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b) return b;

                    return ColorErr(@params[0].ToString(),
                                    visitor,
                                    coord);
                case "ColorRGB" or "colorrgb":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return ColorRGBErr(@params[0].ToInt(),
                                       @params[1].ToInt(),
                                       @params[2].ToInt(),
                                       visitor,
                                       coord);
                case "DrawLine" or "drawline":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return DrawLineErr(@params[0].ToInt(),
                                       @params[1].ToInt(),
                                       @params[2].ToInt(),
                                       visitor,
                                       coord);
                case "Plot" or "plot":
                    AddMissArgErr(identifier, @params, visitor, coord, 2, out b);
                    if (b) return b;

                    return PlotErr(@params[0].ToInt(),
                                 @params[1].ToInt(),
                                 visitor,
                                 coord);
                case "PlotLine" or "plotline":
                    AddMissArgErr(identifier, @params, visitor, coord, 4, out b);
                    if (b) return b;

                    return PlotLineErr(@params[0].ToInt(),
                             @params[1].ToInt(),
                             @params[2].ToInt(),
                             @params[3].ToInt(),
                             visitor,
                             coord);
                case "DrawCircle" or "drawcircle":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return DrawCircleErr(@params[0].ToInt(),
                               @params[1].ToInt(),
                               @params[2].ToInt(),
                               visitor,
                               coord);
                case "PlotCircle" or "plotcircle":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return PlotCircleErr(@params[0].ToInt(),
                               @params[1].ToInt(),
                               @params[2].ToInt(),
                               visitor,
                               coord);
                case "DrawRectangle" or "drawrectangle":
                    AddMissArgErr(identifier, @params, visitor, coord, 5, out b);
                    if (b) return b;

                    return DrawRectangleErr(@params[0].ToInt(),
                                  @params[1].ToInt(),
                                  @params[2].ToInt(),
                                  @params[3].ToInt(),
                                  @params[4].ToInt(),
                                  visitor,
                                  coord);
                case "PlotRectangle" or  "plotrectangle":
                    AddMissArgErr(identifier, @params, visitor, coord, 4, out b);
                    if (b) return b;

                    return PlotRectangleErr(@params[0].ToInt(),
                                  @params[1].ToInt(),
                                  @params[2].ToInt(),
                                  @params[3].ToInt(),
                                  visitor,
                                  coord);
                case "Fill" or "fill":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b) return b;

                    return FillErr(visitor, coord);
                case "Size" or "size":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b) return b;

                    return SizeErr(@params[0].ToInt(), visitor, coord);
                case "Print" or "print":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b) return b;
                    return PrintErr();
                case "Erase" or "erase":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b) return b;
                    return EraseErr(visitor, coord);
                default:
                    visitor.SemanticProblems.Add(new Error(coord, $"Invalid '{identifier}' action"));
                    return true;
            }
        }

        private bool EraseErr(SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }
            return false;
        }

        private bool ColorRGBErr(int r, int g, int b, SemanticVisitor visitor, Coord coord)
        {
            if (r < 0 || r > 255 || g < 0 || g > 255 || b < 0 || b > 255)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Invalid RGB color values: <{r}, {g}, {b}>"));
                return true;
            }
            return false;
        }

        private bool SizeErr(int size, SemanticVisitor visitor, Coord coord) => size <= 0;

        private bool PrintErr() => false;

        private bool SpawnErr(int x0, int y0, SemanticVisitor visitor, Coord coord)
        {
            if (Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Wall-E has already spawned"));
                return true;
            }

            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }

            Window.WallE.Show();
            Window.WallE.SetPos(x0, y0);
            return false;
        }

        private bool DrawErr(SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (Window.WallE.PositionX is null
                || Window.WallE.PositionY is null)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Null Wall-E position"));
                return true;
            }

            return false;
        }

        private bool MoveErr(int x1, int y1, SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x1}, {y1}> position"));
                return true;
            }
            return false;
        }

        private bool PlotErr(int x1, int y1, SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (Window.WallE.PositionX is null
                || Window.WallE.PositionY is null)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Null Wall-E position"));
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x1}, {y1}> position"));
                return true;
            }

            return false;
        }

        private bool ColorErr(string color, SemanticVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Unsupported '{color}' color"));
                return false;
            }
            return true;
        }

        private bool DrawLineErr(int dirX, int dirY, int d, SemanticVisitor visitor, Coord coord)
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            var X = x0 + (dirX * d);
            var Y = y0 + (dirY * d);
            if (!IsValidDirection(dirX) || !IsValidDirection(dirY))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Invalid <{dirX}, {dirY}> direction"));
                return true;
            }

            if (!IsInsideBounds(X, Y))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{X}, {Y}> position"));
                return true;
            }

            return false;
        }

        private bool PlotLineErr(int x0, int y0, int x1, int y1, SemanticVisitor visitor, Coord coord)
        {
            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x1}, {y1}> position"));
                return true;
            }

            return false;
        }

        private bool DrawCircleErr(int dirX, int dirY, int radius, SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }
            if (!IsValidDirection(dirX) || !IsValidDirection(dirY))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Invalid <{dirX}, {dirY}> direction"));
                return true;
            }
            var x0 = GetActualX() + dirX;
            var y0 = GetActualY() + dirY;
            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }
            return false;
        }

        private bool PlotCircleErr(int x0, int y0, int radius, SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }

            return false;
        }

        private bool DrawRectangleErr(int dirX, int dirY, int distance, int width, int height, SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }
            if (!IsValidDirection(dirX) || !IsValidDirection(dirY))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Invalid <{dirX}, {dirY}> direction"));
                return true;
            }
            var x0 = GetActualX() + (dirX * distance);
            var y0 = GetActualX() + (dirY * distance);
            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }

            return false;
        }

        private bool PlotRectangleErr(int x0, int y0, int width, int height, SemanticVisitor visitor, Coord coord)
        {
            if (!IsInsideBounds(x0, y0))
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Outside bounds <{x0}, {y0}> position"));
                return true;
            }

            return false;
        }

        private bool FillErr(SemanticVisitor visitor, Coord coord)
        {
            if (!Window.WallE.IsVisible)
            {
                visitor.SemanticProblems.Add(new Error(coord, "Wall-E has not spawned"));
                return true;
            }

            if (Window.WallE.PositionX is null
                || Window.WallE.PositionY is null)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Null Wall-E position"));
                return true;
            }

            return false;
        }

        #endregion

        private void AddMissArgErr(string identifier,
                                          DynamicValue[] @params,
                                          SemanticVisitor visitor,
                                          Coord coord,
                                          int argNum,
                                          out bool b)
        {
            b = false;
            if (@params.Length != argNum)
            {
                visitor.SemanticProblems.Add(new Error(coord, $"Method '{identifier}' takes {argNum} arguments."));
                b = true;
            }
        }

        private bool IsValidDirection(int dirX) => dirX == 1 || dirX == -1 || dirX == 0;

        private SolidColorBrush GetCurrentColor(int x, int y)
            => (SolidColorBrush)(IsInsideBounds(x, y) ? Window.Rectangles[x, y].Fill : throw new Exception());

        private bool IsInsideBounds(int X, int Y)
            => X > -1 && Y > -1 && X < GetCanvasWidth() && Y < GetCanvasHeight();

        private bool TryParse(DynamicValue dynaValue, out string? printedParam)
        {
            if (TryParse<int>(dynaValue, out printedParam)
                || TryParse<bool>(dynaValue, out printedParam)
                || TryParse<string>(dynaValue, out printedParam))
            {
                return true;
            }
            return false;
        }

        private bool TryParse<T>(DynamicValue dynaValue, out string? printedParam)
        {
            Type typeT = typeof(T);
            if (dynaValue.Value is not null && dynaValue.Type == typeT)
            {
                printedParam = ((T)dynaValue.Value).ToString();
                return true;
            }
            printedParam = null;
            return false;
        }
    }
}