using SilyEngine.Abstractions;
using SilyEngine.Abstractions.Models.Input;
using SilyEngine.Abstractions.Models.Output;
using SilyEngine.TerminalSnake;

public sealed class SnakeGameLogic : ILogicTick
{
    private const int Width = 40;
    private const int Height = 20;
    private const int MoveInterval = 6;
    private const int PointsPerFood = 10;

    private const string BorderColor = "Gray";
    private const string FoodColor = "Red";
    private const string TextColor = "White";

    private static readonly string[] RainbowColors =
        ["Red", "DarkYellow", "Yellow", "Green", "Cyan", "Blue", "Magenta"];

    private static readonly (GameKey Arrow, GameKey Letter, Direction Direction)[] KeyBindings =
    [
        (GameKey.Up,    GameKey.W, Direction.Up),
        (GameKey.Down,  GameKey.S, Direction.Down),
        (GameKey.Left,  GameKey.A, Direction.Left),
        (GameKey.Right, GameKey.D, Direction.Right),
    ];

    private enum Direction { Up, Down, Left, Right }
    private enum GameStatus { WaitingForStart, Playing, GameOver }

    private readonly Random _random = new();
    private readonly List<Position> _body = [];

    private Position _food;
    private Direction _direction;
    private Direction _lastMovedDirection;
    private GameStatus _status = GameStatus.WaitingForStart;
    private int _tickCounter;

    public SnakeGameLogic() => InitializeGame();

    public Position HeadPosition => _body[0];
    public IReadOnlyList<Position> BodyPositions => _body;
    public int Score { get; private set; }

    public ValueTask<DrawState> GameTick(InputState input, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        HandleInput(input);

        if (_status == GameStatus.Playing && ++_tickCounter >= MoveInterval)
        {
            _tickCounter = 0;
            Move();
        }

        return ValueTask.FromResult(CreateDrawState());
    }

    private void InitializeGame()
    {
        const int startX = Width / 2;
        const int startY = Height / 2;

        _body.Clear();
        _body.AddRange([new(startX, startY), new(startX - 1, startY), new(startX - 2, startY)]);

        _direction = Direction.Right;
        _lastMovedDirection = Direction.Right;
        _tickCounter = 0;
        Score = 0;

        SpawnFood();
    }

    private void StartNewGame()
    {
        InitializeGame();
        _status = GameStatus.Playing;
    }

    private void SpawnFood()
    {
        do _food = new(_random.Next(Width), _random.Next(Height));
        while (_body.Contains(_food));
    }

    private void HandleInput(InputState input)
    {
        if (_status != GameStatus.Playing)
        {
            if (input.IsPressed(GameKey.Enter))
                StartNewGame();

            return;
        }

        foreach (var (arrow, letter, direction) in KeyBindings)
        {
            if (!input.IsPressed(arrow) && !input.IsPressed(letter))
                continue;

            // az utoljara tenylegesen megtett lepeshez hasonlitunk, igy ket gyors
            // gombnyomassal sem lehet egy lepesen belul visszafordulni sajat magunkba
            if (direction != Opposite(_lastMovedDirection))
                _direction = direction;

            return;
        }
    }

    private static Direction Opposite(Direction direction) => direction switch
    {
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.Left => Direction.Right,
        Direction.Right => Direction.Left,
        _ => direction,
    };

    private void Move()
    {
        var nextHead = Step(HeadPosition, _direction);
        var isEating = nextHead == _food;

        if (IsOutOfBounds(nextHead) || HitsBody(nextHead, isGrowing: isEating))
        {
            _status = GameStatus.GameOver;
            return;
        }

        _body.Insert(0, nextHead);
        _lastMovedDirection = _direction;

        if (isEating)
        {
            Score += PointsPerFood;
            SpawnFood();
        }
        else
        {
            _body.RemoveAt(_body.Count - 1);
        }
    }

    private static Position Step(Position p, Direction direction) => direction switch
    {
        Direction.Up => new(p.X, p.Y - 1),
        Direction.Down => new(p.X, p.Y + 1),
        Direction.Left => new(p.X - 1, p.Y),
        Direction.Right => new(p.X + 1, p.Y),
        _ => p,
    };

    private static bool IsOutOfBounds(Position p) =>
        p is not { X: >= 0 and < Width, Y: >= 0 and < Height };

    private bool HitsBody(Position p, bool isGrowing) =>
        (isGrowing ? _body : _body.SkipLast(1)).Contains(p);


    private DrawState CreateDrawState() => new()
    {
        Elements =
        [
            .. Border(),
            Text(0, Height + 2, $"Score: {Score}"),
            .. _body.Select((segment, i) => Cell(segment, RainbowColors[i % RainbowColors.Length])),
            Cell(_food, FoodColor),
            .. StatusOverlay(),
        ],
    };

    private static IEnumerable<DrawElement> Border()
    {
        for (var x = 0; x <= Width + 1; x++)
        {
            yield return Block(x, 0, BorderColor);
            yield return Block(x, Height + 1, BorderColor);
        }

        for (var y = 0; y <= Height + 1; y++)
        {
            yield return Block(0, y, BorderColor);
            yield return Block(Width + 1, y, BorderColor);
        }
    }

    private IEnumerable<DrawElement> StatusOverlay()
    {
        var message = _status switch
        {
            GameStatus.WaitingForStart => "PRESS ENTER TO PLAY LGBTQ SNAKE",
            GameStatus.GameOver => "GAME OVER! ENTER",
            _ => null,
        };

        if (message is not null)
            yield return Text(Width / 2 - 8, Height / 2 + 1, message);
    }

    private static DrawElement Cell(Position p, string color) => Block(p.X + 1, p.Y + 1, color);

    private static DrawElement Block(int x, int y, string color) => new()
    {
        Type = DrawElementType.Rectangle,
        X = x,
        Y = y,
        Color = color,
    };

    private static DrawElement Text(int x, int y, string content) => new()
    {
        Type = DrawElementType.Text,
        X = x,
        Y = y,
        Content = content,
        Color = TextColor,
    };
}
