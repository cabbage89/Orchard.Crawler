using Cabbage.Crawler.Analysis;
using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Windows.Forms;

namespace Cabbage.Crawler.Core
{
    public class PluginBase : IPlugin
    {
        protected virtual IAnalysis Analyzer { get; set; }

        public PluginBase()
        {
            Analyzer = new MsWebBrowserAnalysis();
        }

        public virtual void Start()
        {

        }

        public TextWriter Output { get; set; }

        public virtual string GetScriptRoot()
        {
            return HostingEnvironment.MapPath(@"~/Modules/Cabbage.Crawler") + @"\Plugins\" + this.GetType().Name + @"\Scripts";
        }

        /// <summary>
        /// 获取脚本
        /// </summary>
        /// <param name="scriptFile">脚本文件名</param>
        protected string GetScript(string scriptFile)
        {
            var scriptRoot = GetScriptRoot();
            string path = null;
            try
            {
                path = Path.Combine(scriptRoot, scriptFile);
                return File.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Exception _e;
                do
                {
                    _e = ex;
                    ex = ex.InnerException;
                } while (ex != null);

                Output.WriteLine(_e.Message);
            }
            return null;
        }

        public void ExecScript(object callbackCOMObj, Uri crawlerUrl, string scriptFile, string method, Action<object> completed)
        {
            Analyzer.Analyze(callbackCOMObj,GetScript(scriptFile), method, crawlerUrl, completed);
        }

        public void ExecScript(Uri crawlerUrl, string scriptFile, string method, Action<object> completed)
        {
            ExecScript(new JsCallback { Output = Output }, crawlerUrl, scriptFile, method, completed);
        }
    }
}