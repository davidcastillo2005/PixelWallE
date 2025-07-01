using System.CodeDom;
using System.Printing.IndexedProperties;
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

        #region Function
        public DynamicValue CallFunction(string Name, DynamicValue[] @params) => Name switch
        {
            "GetActualX" => new DynamicValue(GetActualX()),
            "GetActualY" => new DynamicValue(GetActualY()),
            "GetCanvasSize" => new DynamicValue(GetCanvasSize()),
            "GetCanvasWidth" => new DynamicValue(GetCanvasWidth()),
            "GetCanvasHeight" => new DynamicValue(GetCanvasHeight()),
            "GetColorCount" => new DynamicValue(GetColorCount(@params[0].ToString(),
                                                        @params[1].ToInterger(),
                                                        @params[2].ToInterger(),
                                                        @params[3].ToInterger(),
                                                        @params[4].ToInterger())),
            "IsBrushColor" => new DynamicValue(IsBrushColor(@params[0].ToString())),
            "IsBrushSize" => new DynamicValue(IsBrushSize(@params[0].ToInterger())),
            "IsCanvascolor" => new DynamicValue(IsCanvasColor(@params[0].ToString(),
                                                        @params[1].ToInterger(),
                                                        @params[2].ToInterger())),
            _ => throw new NotImplementedException(),
        };

        private int GetActualX() => Window.GetWallEPosX()!;

        private int GetActualY() => Window.GetWallEPosY()!;

        private int GetCanvasSize()
            => Window.GetCanvasWidth() == Window.GetCanvasWidth()
            ? Window.GetCanvasSize()
            : throw new NotImplementedException();

        private int GetCanvasWidth() => Window.GetCanvasWidth();

        private int GetCanvasHeight() => Window.GetCanvasHeight();

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
            var brushColor = Window.GetWallEBrushColor();
            return stringToBrush[color] == brushColor ? 1 : 0;
        }

        private int IsBrushSize(int size)
        {
            var brushThickness = Window.GetWallEBrushThickness();
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

        #region Actions

        public void CallAction(string Name, DynamicValue[] @params, Coord coord)
        {
            switch (Name)
            {
                case "Spawn":
                    Spawn(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "Draw":
                    Draw();
                    break;
                case "Plot":
                    Plot(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "Move":
                    Move(@params[0].ToInterger(), @params[1].ToInterger());
                    break;
                case "Size":
                    Size(@params[0].ToInterger());
                    break;
                case "Color":
                    Color(@params[0].ToString());
                    break;
                case "DrawLine":
                    DrawLine(@params[0].ToInterger(), @params[1].ToInterger(), @params[2].ToInterger());
                    break;
                case "PlotLine":
                    PlotLine(@params[0].ToInterger(), @params[1].ToInterger(), @params[2].ToInterger(), @params[3].ToInterger());
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
                    var x0 = GetActualX();
                    var y0 = GetActualY();
                    Fill(GetCurrentColor(x0, y0));
                    Move(x0, y0);
                    break;
                case "Print":
                    TryParse(@params[0], out string? printedParam);
                    Print(printedParam!);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void TryParse(DynamicValue dynaValue, out string? printedParam)
        {
            
            if (TryParse<int>(dynaValue, out printedParam) 
                || TryParse<bool>(dynaValue, out printedParam)
                || TryParse<string>(dynaValue, out printedParam))
            {
                return;
            }
        }

        private bool TryParse<T>(DynamicValue dynaValue, out string? printedParam)
        {
            Type TType = typeof(T);
            if (dynaValue.Value is not null && dynaValue.Type == TType)
            {
                printedParam = (string?)(((T)dynaValue.Value).ToString());
                return true;
            }
            printedParam = null;
            return false;
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
            int offset = (Window.GetWallEBrushThickness() - 1) / 2;
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

        private void Size(int size) => Window.ChangeBrushSize(size);

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

        private void Print(string printedParam) => Window.Print(printedParam);
        
        #endregion

        #region ErrFunction
        public bool TryGetErrFunction(string identifier, DynamicValue[] @params, SemanticErrVisitor visitor, Coord coord, out DynamicValue @return)
        {
            switch (identifier)
            {
                case "GetActualX":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out bool b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return GetActualXErr(visitor, coord);
                case "GetActualY":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return GetActualYErr(visitor, coord);
                case "GetCanvasSize":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return GetCanvasSizeErr(visitor, coord);
                case "GetCanvasWidth":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return GetCanvasWidthErr(visitor, coord);
                case "GetCanvasHeight":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return GetCanvasHeightErr(visitor, coord);
                case "GetColorCount":
                    AddMissArgErr(identifier, @params, visitor, coord, 5, out b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return GetColorCountErr(@params[0].ToString(),
                                            @params[1].ToInterger(),
                                            @params[2].ToInterger(),
                                            @params[3].ToInterger(),
                                            @params[4].ToInterger(),
                                            visitor,
                                            coord);
                case "IsBrushColor":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return IsBrushColorErr(@params[0].ToString(),
                                           visitor,
                                           coord);
                case "IsBrushSize":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    @return = visitor.GetObject(coord, null);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return IsBrushSizeErr(@params[0].ToInterger(),
                                          visitor,
                                          coord);
                case "IsCanvascolor":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    @return = visitor.GetObject(coord, null);
                    if (b)
                    {
                        @return = visitor.GetObject(coord, null);
                        return b;
                    }
                    @return = visitor.GetObject(coord, new DynamicValue(0));
                    return IsCanvasColorErr(@params[0].ToString(),
                                            @params[1].ToInterger(),
                                            @params[2].ToInterger(),
                                            visitor,
                                            coord);
                default:
                    visitor.AddException(coord, $"Invalid '{identifier}' function");
                    @return = visitor.GetObject(coord, null);
                    return false;
            }
        }

        private bool IsCanvasColorErr(string color, int v2, int v3, SemanticErrVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.AddException(coord, $"Unsupported '{color}' color");
                return true;
            }
            return false;
        }

        private bool IsBrushSizeErr(int size, SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (size < 1) return true;
            return false;
        }

        private bool IsBrushColorErr(string color, SemanticErrVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.AddException(coord, $"Unsupported '{color}' color");
                return true;
            }

            return false;
        }

        private bool GetColorCountErr(string color, int x0, int y0, int x1, int y1, SemanticErrVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.AddException(coord, $"Unsupported '{color}' color");
                return true;
            }

            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.AddException(coord, $"Outside bounds <{x1}, {y1}> position");
                return true;
            }

            return false;
        }

        private bool GetCanvasHeightErr(SemanticErrVisitor visitor, Coord coord)
        {
            return true;
        }

        private bool GetCanvasWidthErr(SemanticErrVisitor visitor, Coord coord) => false;

        private bool GetCanvasSizeErr(SemanticErrVisitor visitor, Coord coord)
        {
            if (Window.GetCanvasWidth() == Window.GetCanvasWidth())
            {
                return false;
            }

            visitor.AddException(coord, "Canvas width does not match canvas height");
            return true;
        }

        private bool GetActualYErr(SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (Window.WallE.PositionY is null)
            {
                visitor.AddException(coord, $"Null Wall-E position");
                return true;
            }

            return false;
        }

        private bool GetActualXErr(SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (Window.WallE.PositionX is null)
            {
                visitor.AddException(coord, $"Null Wall-E position");
                return true;
            }

            return false;
        }

        #endregion

        #region ErrAction
        public bool TryGetErrAction(string identifier, DynamicValue[] @params, SemanticErrVisitor visitor, Coord coord)
        {
            switch (identifier)
            {
                case "Spawn":
                    AddMissArgErr(identifier, @params, visitor, coord, 2, out bool b);
                    if (b) return b;

                    return SpawnErr(@params[0].ToInterger(),
                                    @params[1].ToInterger(),
                                    visitor,
                                    coord);
                case "Draw":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b) return b;

                    return DrawErr(visitor, coord);
                case "Move":
                    AddMissArgErr(identifier, @params, visitor, coord, 2, out b);
                    if (b) return b;

                    return MoveErr(@params[0].ToInterger(),
                                   @params[1].ToInterger(),
                                   visitor,
                                   coord);
                case "Color":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b) return b;

                    return ColorErr(@params[0].ToString(),
                                    visitor,
                                    coord);
                case "DrawLine":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return DrawLineErr(@params[0].ToInterger(),
                                       @params[1].ToInterger(),
                                       @params[2].ToInterger(),
                                       visitor,
                                       coord);
                case "Plot":
                    AddMissArgErr(identifier, @params, visitor, coord, 2, out b);
                    if (b) return b;

                    return PlotErr(@params[0].ToInterger(),
                                 @params[1].ToInterger(),
                                 visitor,
                                 coord);
                case "PlotLine":
                    AddMissArgErr(identifier, @params, visitor, coord, 4, out b);
                    if (b) return b;

                    return PlotLineErr(@params[0].ToInterger(),
                             @params[1].ToInterger(),
                             @params[2].ToInterger(),
                             @params[3].ToInterger(),
                             visitor,
                             coord);
                case "DrawCircle":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return DrawCircleErr(@params[0].ToInterger(),
                               @params[1].ToInterger(),
                               @params[2].ToInterger(),
                               visitor,
                               coord);
                case "PlotCircle":
                    AddMissArgErr(identifier, @params, visitor, coord, 3, out b);
                    if (b) return b;

                    return PlotCircleErr(@params[0].ToInterger(),
                               @params[1].ToInterger(),
                               @params[2].ToInterger(),
                               visitor,
                               coord);
                case "DrawRectangle":
                    AddMissArgErr(identifier, @params, visitor, coord, 5, out b);
                    if (b) return b;

                    return DrawRectangleErr(@params[0].ToInterger(),
                                  @params[1].ToInterger(),
                                  @params[2].ToInterger(),
                                  @params[3].ToInterger(),
                                  @params[4].ToInterger(),
                                  visitor,
                                  coord);
                case "PlotRectangle":
                    AddMissArgErr(identifier, @params, visitor, coord, 4, out b);
                    if (b) return b;

                    return PlotRectangleErr(@params[0].ToInterger(),
                                  @params[1].ToInterger(),
                                  @params[2].ToInterger(),
                                  @params[3].ToInterger(),
                                  visitor,
                                  coord);
                case "Fill":
                    AddMissArgErr(identifier, @params, visitor, coord, 0, out b);
                    if (b) return b;

                    return FillErr(visitor, coord);
                case "Print":
                    AddMissArgErr(identifier, @params, visitor, coord, 1, out b);
                    if (b) return b;
                    return PrintErr();
                default:
                    visitor.AddException(coord, $"Invalid '{identifier}' action");
                    return true;
            }
        }

        private bool PrintErr() => true;

        private void AddMissArgErr(string identifier,
                                          DynamicValue[] @params,
                                          SemanticErrVisitor visitor,
                                          Coord coord,
                                          int argNum,
                                          out bool b)
        {
            b = false;
            if (@params.Length != argNum)
            {
                visitor.AddException(coord, $"Missing arguments at '{identifier}'.");
                b = true;
            }
        }

        private bool SpawnErr(int x0, int y0, SemanticErrVisitor visitor, Coord coord)
        {
            if (Window.GetWallEVisibility())
            {
                visitor.AddException(coord, $"Wall-E has already spawned");
                return true;
            }

            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }

            Window.WallE.Show();
            Window.WallE.SetPos(x0, y0);
            return false;
        }

        private bool DrawErr(SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (Window.WallE.PositionX is null
                || Window.WallE.PositionY is null)
            {
                visitor.AddException(coord, $"Null Wall-E position");
                return true;
            }

            return false;
        }

        private bool MoveErr(int x1, int y1, SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.AddException(coord, $"Outside bounds <{x1}, {y1}> position");
                return true;
            }
            return false;
        }

        private bool PlotErr(int x1, int y1, SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (Window.WallE.PositionX is null
                || Window.WallE.PositionY is null)
            {
                visitor.AddException(coord, $"Null Wall-E position");
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.AddException(coord, $"Outside bounds <{x1}, {y1}> position");
                return true;
            }

            return false;
        }

        private bool ColorErr(string color, SemanticErrVisitor visitor, Coord coord)
        {
            if (!stringToBrush.ContainsKey(color))
            {
                visitor.AddException(coord, $"Unsupported '{color}' color");
                return false;
            }
            return true;
        }

        private bool DrawLineErr(int dirX, int dirY, int d, SemanticErrVisitor visitor, Coord coord)
        {
            var x0 = GetActualX();
            var y0 = GetActualY();
            var X = x0 + (dirX * d);
            var Y = y0 + (dirY * d);
            if (!IsValidDirection(dirX) || !IsValidDirection(dirY))
            {
                visitor.AddException(coord, $"Invalid <{dirX}, {dirY}> direction");
                return true;
            }

            if (!IsInsideBounds(X, Y))
            {
                visitor.AddException(coord, $"Outside bounds <{X}, {Y}> position");
                return true;
            }

            return false;
        }

        private bool PlotLineErr(int x0, int y0, int x1, int y1, SemanticErrVisitor visitor, Coord coord)
        {
            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }

            if (!IsInsideBounds(x1, y1))
            {
                visitor.AddException(coord, $"Outside bounds <{x1}, {y1}> position");
                return true;
            }

            return false;
        }

        private bool DrawCircleErr(int dirX, int dirY, int radius, SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }
            if (!IsValidDirection(dirX) || !IsValidDirection(dirY))
            {
                visitor.AddException(coord, $"Invalid <{dirX}, {dirY}> direction");
                return true;
            }
            var x0 = GetActualX() + dirX;
            var y0 = GetActualY() + dirY;
            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }
            return false;
        }

        private bool PlotCircleErr(int x0, int y0, int radius, SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }

            return false;
        }

        private bool DrawRectangleErr(int dirX, int dirY, int distance, int width, int height, SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }
            if (!IsValidDirection(dirX) || !IsValidDirection(dirY))
            {
                visitor.AddException(coord, $"Invalid <{dirX}, {dirY}> direction");
                return true;
            }
            var x0 = GetActualX() + (dirX * distance);
            var y0 = GetActualX() + (dirY * distance);
            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }

            return false;
        }

        private bool PlotRectangleErr(int x0, int y0, int width, int height, SemanticErrVisitor visitor, Coord coord)
        {
            if (!IsInsideBounds(x0, y0))
            {
                visitor.AddException(coord, $"Outside bounds <{x0}, {y0}> position");
                return true;
            }

            return false;
        }

        private bool FillErr(SemanticErrVisitor visitor, Coord coord)
        {
            if (!Window.GetWallEVisibility())
            {
                visitor.AddException(coord, "Wall-E has not spawned");
                return true;
            }

            return false;
        }

        #endregion

        private bool IsValidDirection(int dirX)
            => dirX == 1 || dirX == -1 || dirX == 0;

        private SolidColorBrush GetCurrentColor(int x, int y)
            => (SolidColorBrush)(IsInsideBounds(x, y) ? Window.Rectangles[x, y].Fill : throw new Exception());

        private bool IsInsideBounds(int X, int Y)
            => X > -1 && Y > -1 && X < GetCanvasWidth() && Y < GetCanvasHeight();
    }
}
