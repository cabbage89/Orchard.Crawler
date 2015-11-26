using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard;

namespace Cabbage.Crawler.Core
{
    public interface IJsCallback : IOutput, IDependency
    {
        void Handle(string handle, string data);
    }
}
