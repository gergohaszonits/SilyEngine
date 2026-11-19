using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SilyEngine.TerminalSnake;

public readonly struct Position
{
    public readonly int X;
    public readonly int Y;
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X,Y);
    }
    public static bool operator ==(Position a, Position b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Position a, Position b)
    {
        return !a.Equals(b);
    }
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Position pos)
            return false;

        return this.X == pos.X && this.Y == pos.Y;
    }
}
