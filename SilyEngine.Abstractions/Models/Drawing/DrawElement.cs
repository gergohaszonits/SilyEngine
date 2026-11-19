using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Abstractions.Models.Output;
public record DrawElement
{
    public DrawElementType Type { get; init; }

    public int X { get; init; }
    public int Y { get; init; }

    public string? Content { get; init; }
    //ehhh
    public string? Color { get; init; }
}
