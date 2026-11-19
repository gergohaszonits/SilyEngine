using SilyEngine.Abstractions.Models.Input;
using SilyEngine.Abstractions.Models.Output;

namespace SilyEngine.Abstractions.Services;

public sealed class SilyEngineRunner<TLogic, TGraphics, TInput>
    where TLogic : ILogicTick, new()
    where TGraphics : IGraphicsDrawer, new()
    where TInput : IInputReader, new()
{
    private readonly TLogic _logic = new();
    private readonly TInput _input = new();
    private readonly TGraphics _graphics = new();

    private readonly TimeSpan _tickInterval;
    private readonly CancellationTokenSource _cancellation;

    public SilyEngineRunner(int tickRate, CancellationTokenSource? cancellation = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(tickRate);

        _tickInterval = TimeSpan.FromSeconds(1.0 / tickRate);
        _cancellation = cancellation ?? new();
    }

    public async Task RunAsync(Func<InputState, DrawState, Task>? onTick = null)
    {
        var token = _cancellation.Token;

        try
        {
            using var timer = new PeriodicTimer(_tickInterval);

            while (await timer.WaitForNextTickAsync(token))
                await TickAsync(onTick, token);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            //sily bye bye
        }
    }

    private async Task TickAsync(Func<InputState, DrawState, Task>? onTick, CancellationToken token)
    {
        var input = _input.ReadInput();
        var frame = await _logic.GameTick(input, token);

        if (onTick is not null)
            await onTick(input, frame);

        await _graphics.RenderDrawState(frame);
    }
}