using SilyEngine.Abstractions.Models.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Abstractions;

public interface IInputReader
{
    public InputState ReadInput();
}
