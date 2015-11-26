using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cabbage.Crawler.Core
{
    public interface IOutput
    {
        TextWriter Output { get; set; }
    }
}
