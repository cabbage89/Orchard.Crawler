using Cabbage.Crawler.Core;
using mshtml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace Cabbage.Crawler.Analysis
{
    public class MsWebBrowserAnalysis : IAnalysis
    {
        private List<EventHandler> _documentCompletedEventList = new List<EventHandler>();

        private WebBrowser _browser;

        /// <summary>
        /// 在新线程中处理事件
        /// </summary>
        event EventHandler MyDocumentCompleted
        {
            add
            {
                _documentCompletedEventList.Add(value);
            }
            remove
            {
                _documentCompletedEventList.Remove(value);
            }
        }

        void ClearDocumentCompletedEvent()
        {
            _documentCompletedEventList.Clear();
        }

        /// <param name="ready">任务主体</param>
        public void CreteTask(Action ready)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form
            {
                AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F),
                AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font,
                ClientSize = new System.Drawing.Size(525, 427),
                Name = "Form1",
                Text = "爬虫",
                ControlBox = false,
                ShowInTaskbar = false
            };
            _browser = new WebBrowser
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                Location = new System.Drawing.Point(0, 0),
                ScriptErrorsSuppressed = true,
            };
            _browser.DocumentCompleted += (s, e) =>
            {
                form.Text = string.Format("{0}", _browser.Url);
                if (_documentCompletedEventList != null)
                    foreach (var handle in _documentCompletedEventList)
                        handle(s, e);
            };
            var _timer = new System.Windows.Forms.Timer()
            {
                Interval = 10,
                Enabled = true
            };
            form.Controls.Add(_browser);
            _timer.Tick += (s, e) =>
            {
                form.Close();
                _timer.Stop();
            };
            form.Load += (s, e) =>
            {
                _timer.Start();
            };
            form.FormClosing += (s, e) =>
            {
                ready();
                e.Cancel = true;
            };
            form.Activated += (s, e) =>
            {
                //此处可以将窗口隐藏
                //form.Hide();
            };
            Application.Run(form);
        }

        public void Analyze(object callbackCOMObj, string script, string method, Uri url, Action<object> completed)
        {
            _browser.ObjectForScripting = callbackCOMObj;
            ClearDocumentCompletedEvent();
            MyDocumentCompleted += (sender, e) =>
            {
                if (_browser.ReadyState < WebBrowserReadyState.Complete)
                    return;
                HtmlElement head = _browser.Document.GetElementsByTagName("head")[0];
                HtmlElement scriptEl = _browser.Document.CreateElement("script");
                IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
                element.text = string.Format(";{0};", script);
                //注入
                head.AppendChild(scriptEl);
                //C#调用Script
                var args = method.Split(new char[] { '|' });
                object[] objects = new object[args.Length - 1];
                Array.Copy(args, 1, objects, 0, objects.Length);
                var result = _browser.Document.InvokeScript(args[0], objects);
                var thread = new Thread(new ParameterizedThreadStart(completed));
                thread.Start(result);//此线程用于通知数据已经获取完毕,执行线程将往下执行.
            };
            _browser.Navigate(url);
        }
    }
}