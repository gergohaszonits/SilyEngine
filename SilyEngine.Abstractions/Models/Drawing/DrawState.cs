using SilyEngine.Abstractions.Abstractions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace SilyEngine.Abstractions.Models.Output;

public class DrawState : IState
{
    public IReadOnlyList<DrawElement> Elements { get; init; }
        = [];
}
