using SilyEngine.Abstractions;
using SilyEngine.Abstractions.Models.Input;
using System;
using System.Collections.Generic;
using System.Text;
namespace SilyEngine.Terminal;

public class TerminalInputReader : IInputReader
{
    private readonly GameKey[] _buffer = new GameKey[32];

    public InputState ReadInput()
    {
        int count = 0;

        while (Console.KeyAvailable)
        {
            var key = Console.ReadKey(true).Key;

            if (count < _buffer.Length)
                _buffer[count++] = MapConsolekey(key);
        }

        return new InputState(_buffer.AsMemory(0, count));
    }

    private static GameKey MapConsolekey(ConsoleKey ckey)
    {
        return ckey switch
        {
            ConsoleKey.UpArrow => GameKey.Up,
            ConsoleKey.DownArrow => GameKey.Down,
            ConsoleKey.LeftArrow => GameKey.Left,
            ConsoleKey.RightArrow => GameKey.Right,

            ConsoleKey.Spacebar => GameKey.Space,
            ConsoleKey.Enter => GameKey.Enter,
            ConsoleKey.Escape => GameKey.Escape,

            ConsoleKey.A => GameKey.A,
            ConsoleKey.B => GameKey.B,
            ConsoleKey.C => GameKey.C,
            ConsoleKey.D => GameKey.D,
            ConsoleKey.W => GameKey.W,
            ConsoleKey.S => GameKey.S,

            _ => GameKey.Unknown
        };
    }
}
