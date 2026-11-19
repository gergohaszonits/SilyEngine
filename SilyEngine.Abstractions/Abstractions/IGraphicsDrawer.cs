using SilyEngine.Abstractions.Models.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Abstractions;

public interface IGraphicsDrawer
{
    ValueTask RenderDrawState(DrawState state);
}
