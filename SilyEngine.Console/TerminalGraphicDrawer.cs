using SilyEngine.Abstractions;
using SilyEngine.Abstractions.Models.Output;

namespace SilyEngine.Terminal;

public sealed class TerminalGraphicDrawer : IGraphicsDrawer
{
    private const ConsoleColor DefaultColor = ConsoleColor.White;
    private static readonly ScreenCell EmptyCell = new(' ', DefaultColor);

    private readonly int _width = Console.WindowWidth;
    private readonly int _height = Console.WindowHeight;

    private ScreenCell[] _current;
    private ScreenCell[] _previous;

    public TerminalGraphicDrawer()
    {
        _current = CreateBuffer();
        _previous = CreateBuffer();

        Console.CursorVisible = false;
        Console.Clear();
    }

    public ValueTask RenderDrawState(DrawState state)
    {
        Array.Fill(_current, EmptyCell);

        foreach (var element in state.Elements)
            Compose(element);

        FlushChanges();

        (_current, _previous) = (_previous, _current);

        return ValueTask.CompletedTask;
    }

    private void Compose(DrawElement element)
    {
        var color = MapColor(element.Color);

        if (element.Type == DrawElementType.Text)
            WriteText(element.X, element.Y, element.Content, color);
        else
            SetCell(element.X, element.Y, new(GlyphFor(element.Type), color));
    }

    private void WriteText(int x, int y, string? text, ConsoleColor color)
    {
        if (text is null)
            return;

        for (var i = 0; i < text.Length; i++)
            SetCell(x + i, y, new(text[i], color));
    }

    private void SetCell(int x, int y, ScreenCell cell)
    {
        if (IsInsideScreen(x, y))
            _current[y * _width + x] = cell;
    }

    private bool IsInsideScreen(int x, int y) =>
        x >= 0 && x < _width &&
        y >= 0 && y < _height;

    private void FlushChanges()
    {
        ConsoleColor? activeColor = null;

        for (var i = 0; i < _current.Length; i++)
        {
            var cell = _current[i];

            if (cell == _previous[i])
                continue;

            Console.SetCursorPosition(i % _width, i / _width);

            if (cell.Color != activeColor)
            {
                Console.ForegroundColor = cell.Color;
                activeColor = cell.Color;
            }

            Console.Write(cell.Character);
        }

        Console.ResetColor();
    }

    private ScreenCell[] CreateBuffer()
    {
        var buffer = new ScreenCell[_width * _height];
        Array.Fill(buffer, EmptyCell);
        return buffer;
    }

    private static char GlyphFor(DrawElementType type) => type switch
    {
        DrawElementType.Rectangle => '█',
        DrawElementType.Circle => '●',
        DrawElementType.Line => '─',
        _ => ' ',
    };

    private static ConsoleColor MapColor(string? color) =>
        Enum.TryParse(color, ignoreCase: true, out ConsoleColor result) ? result : DefaultColor;
}
