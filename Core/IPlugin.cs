using Orchard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cabbage.Crawler.Core
{
    public interface IPlugin : IOutput, IDependency
    {
        string GetScriptRoot();

        void Start();

        void ExecScript(object callbackCOMObj, Uri crawlerUrl, string scriptFile, string method, Action<object> completed);
    }
}
