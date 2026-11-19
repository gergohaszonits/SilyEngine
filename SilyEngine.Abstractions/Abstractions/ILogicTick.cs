using SilyEngine.Abstractions.Models.Input;
using SilyEngine.Abstractions.Models.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Abstractions;

public interface ILogicTick
{
    ValueTask<DrawState> GameTick(InputState input,CancellationToken token);
}
