using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cabbage.Crawler.Analysis
{
    public interface IAnalysis : IDependency
    {
        void CreteTask(Action ready);

        void Analyze(object callbackCOMObj, string script, string method, Uri url, Action<object> completed);
    }
}
