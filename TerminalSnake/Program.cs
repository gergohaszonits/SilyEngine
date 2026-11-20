using SilyEngine.Abstractions;
using SilyEngine.Abstractions.Models.Input;
using SilyEngine.Abstractions.Services;
using SilyEngine.Terminal;
using SilyEngine.TerminalSnake;

using var cancellation = new CancellationTokenSource();

var snakeGame =
    new SilyEngineRunner<SnakeGameLogic, TerminalGraphicDrawer, TerminalInputReader>(tickRate: 144, cancellation);

await snakeGame.RunAsync((input, _) =>
{
    // Escape-re sily bye bye
    if (input.IsPressed(GameKey.Escape))
        cancellation.Cancel();

    return Task.CompletedTask;
});

Console.ResetColor();
Console.Clear();
Console.CursorVisible = true;
