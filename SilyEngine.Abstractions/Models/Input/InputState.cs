using SilyEngine.Abstractions.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Abstractions.Models.Input;

public readonly struct InputState
{
    public ReadOnlyMemory<GameKey> Keys { get; }

    public InputState(ReadOnlyMemory<GameKey> keys)
    {
        Keys = keys;
    }

    public bool IsPressed(GameKey key)
        => Keys.Span.Contains(key);
}
