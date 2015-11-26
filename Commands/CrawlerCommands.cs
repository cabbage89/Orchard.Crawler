using Cabbage.Crawler.Core;
using Orchard.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cabbage.Crawler.Commands
{
    public class CrawlerCommands : DefaultOrchardCommandHandler
    {
        private IEnumerable<IPlugin> _plugins;

        public CrawlerCommands(IEnumerable<IPlugin> plugins)
        {
            _plugins = plugins;
        }

        [OrchardSwitch]
        public string PluginName { get; set; }

        [CommandName("crawler start")]
        [CommandHelp("crawler start /PluginName:<name>\r\n\t" + "启动新的爬行任务")]
        [OrchardSwitches("PluginName")]
        public void Start()
        {
            var plugin = (from p in _plugins 
                          where string.Compare(p.GetType().Name, PluginName, true) == 0 
                          select p).FirstOrDefault();
            if (plugin != null)
            {
                plugin.Output = Context.Output;
                plugin.Start();
            }
            else
            {
                Context.Output.WriteLine(string.Format("插件:[{0}]未发现!", PluginName));
            }
        }
    }
}