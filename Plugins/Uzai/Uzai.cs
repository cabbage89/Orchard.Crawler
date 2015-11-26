using Cabbage.Crawler.Analysis;
using Cabbage.Crawler.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Cabbage.Crawler.Plugins.Uzai
{
    public class Uzai : PluginBase
    {
        string xmlPath = "e:\\visalist.xml";
        string visaItemListPath = "e:\\visaitemlist.xml";

        public override void Start()
        {
            var thread = new Thread(new ParameterizedThreadStart((p) =>
            {
                Analyzer.CreteTask(() =>
                {
                    //分析器准备完毕后进入此回调(在此本实现逻辑中，实际上是窗口定时关闭事件)

                    AutoResetEvent locker = new AutoResetEvent(false);
                    //新开启执行线程
                    var thread2 = new Thread(new ParameterizedThreadStart((a) =>
                    {
                        List<Visa> VisaList = null;
                        if (!File.Exists(xmlPath))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Output.WriteLine("\r\n-------正在爬取国家与领区信息--------");

                            ExecScript(new Uri("http://www.uzai.com/visa/"), "getCountries.js", "getCountries", (result) =>
                            {
                                //此方法体在浏览器文档完成事件中新开启的线程中执行！！
                                if (JsCallback.Countries != null)
                                    foreach (var item in JsCallback.Countries)
                                    {
                                        //请在此写插入你数据库的逻辑！
                                        Output.WriteLine(string.Format("\r\n国家:{0}", item.text));
                                    }
                                //通知执行线程可以继续执行下一任务
                                locker.Set();
                            });
                            //必须等待浏览器回调完后才能进行下一个任务
                            locker.WaitOne();

                            VisaList = new List<Visa>();

                            if (JsCallback.Countries != null)
                            {
                                //遍历上次浏览器返回的爬行结果
                                foreach (var item in JsCallback.Countries)
                                {
                                    //根据上次结果进行二级页面爬行
                                    ExecScript(new Uri(item.url), "GetConsulate.js", "GetConsulate", (_r) =>
                                    {
                                        if (JsCallback.Visa != null)
                                        {
                                            JsCallback.Visa.Country = item.text;
                                            VisaList.Add(JsCallback.Visa);
                                            Console.ForegroundColor = ConsoleColor.White;
                                            Output.WriteLine("\r\n---------------------------------------------");
                                            Output.WriteLine(string.Format("\r\n领馆:{0}\r\n{1}\r\n{2}"
                                                , JsCallback.Visa.Name, JsCallback.Visa.Desc, JsCallback.Visa.ImgUrl));
                                            if (JsCallback.Visa.ConsulateList != null)
                                                foreach (var c in JsCallback.Visa.ConsulateList)
                                                {
                                                    //请在此写插入你数据库的逻辑！
                                                    Output.WriteLine(string.Format("\r\n领区:{0}\r\n{1}", c.name, c.desc));
                                                    foreach (var visa in c.visaList)
                                                    {
                                                        Output.WriteLine(string.Format("\r\n包含签证:{0}", visa.text));
                                                    }
                                                }
                                            Output.WriteLine("\r\n---------------------------------------------");
                                        }
                                        locker.Set();
                                    });
                                    locker.WaitOne();
                                }
                            }
                            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Visa>));
                            var file = new System.IO.StreamWriter(xmlPath);
                            serializer.Serialize(file, VisaList);
                            file.Close();
                        }
                        else
                        {
                            using (FileStream fsReader = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
                            {
                                var xs = new XmlSerializer(typeof(List<Visa>));
                                VisaList = xs.Deserialize(fsReader) as List<Visa>;
                            }
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Output.WriteLine("\r\n--------读取到上次国家与领区信息爬取结果--------");
                        }




                        List<VisaItem> VisaItemList = new List<VisaItem>();
                        if (!File.Exists(visaItemListPath))
                        {
                            foreach (var visa in VisaList)
                            {
                                foreach (var consulate in visa.ConsulateList)
                                {
                                    foreach (var visaItem in consulate.visaList)
                                    {
                                        //根据上次结果进行详情页面爬行
                                        ExecScript(new Uri(visaItem.url), "GetVisa.js", "GetVisa", (_r) =>
                                        {
                                            JsCallback.VisaItem.City = consulate.name;
                                            JsCallback.VisaItem.Country = visa.Country;
                                            VisaItemList.Add(JsCallback.VisaItem);
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Output.WriteLine(string.Format("\r\n获取签证[ {0} ]", JsCallback.VisaItem.Name));
                                            locker.Set();
                                        });
                                        locker.WaitOne();

                                    }

                                }
                            }
                            var serializer2 = new System.Xml.Serialization.XmlSerializer(typeof(List<VisaItem>));
                            var file2 = new System.IO.StreamWriter(visaItemListPath);
                            serializer2.Serialize(file2, VisaItemList);
                            file2.Close();
                        }
                        else
                        {
                            using (FileStream fsReader = new FileStream(visaItemListPath, FileMode.Open, FileAccess.Read))
                            {
                                var xs = new XmlSerializer(typeof(List<VisaItem>));
                                VisaItemList = xs.Deserialize(fsReader) as List<VisaItem>;
                            }
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Output.WriteLine("\r\n--------读取到上次签证列表爬取结果--------");
                        }


                        Console.ForegroundColor = ConsoleColor.Green;
                        Output.WriteLine("\r\n-------------爬取完毕-----------");

                    }));
                    thread2.Start();
                });
            }));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Console.ForegroundColor = ConsoleColor.Green;
            Output.WriteLine("\r\n-----------悠哉爬虫程序已启动-----------");
        }

        #region 入库操作

        void InsertCountry(string name, string desc)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Output.WriteLine(string.Format("\r\n插入国家[ {0} ]", name));
        }

        void InsertConsulate(string name, string desc)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Output.WriteLine(string.Format("\r\n └插入领区[ {0} ]", name));
        }

        #endregion

    }
}