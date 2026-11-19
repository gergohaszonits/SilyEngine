using System;
using System.Collections.Generic;
using System.Text;

namespace SilyEngine.Abstractions.Abstractions;

public interface IConfigurable<TOptions>
{
    void Configure(Func<TOptions> config);
}
