global using Token = PixelWallE.Global.Token;
global using DynamicValue = PixelWallE.Global.DynamicValue;
global using Error = PixelWallE.Global.Error;
global using Coord = PixelWallE.Global.Coord;
global using Lexer = PixelWallE.SourceCodeAnalisis.Lexical.Lexer;
global using Parser = PixelWallE.SourceCodeAnalisis.Syntactic.Parser;
global using SemanticErrVisitor = PixelWallE.SourceCodeAnalisis.Semantic.Visitors.SemanticErrVisitor;
global using InterpreterVisitor = PixelWallE.SourceCodeAnalisis.Semantic.Visitors.InterpreterVisitor;