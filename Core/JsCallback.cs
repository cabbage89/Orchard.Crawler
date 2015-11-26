using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;

namespace Cabbage.Crawler.Core
{
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class JsCallback : IJsCallback
    {
        public TextWriter Output { get; set; }

        #region 注册COM可见方法

        public void Handle(string handle, string data)
        {
            var args = handle.Split(new char[] { ',' });
            var assembly = Assembly.GetExecutingAssembly();
            var type = assembly.GetType(args[0]);
            var method = type.GetMethod(args[1]);
            var instance = assembly.CreateInstance(args[0]);
            method.Invoke(instance, new object[] { Output, data });
        }

        #endregion
    }
}