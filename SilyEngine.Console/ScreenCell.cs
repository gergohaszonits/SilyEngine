using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Terminal;

public readonly record struct ScreenCell(
   char Character,
   ConsoleColor Color);
