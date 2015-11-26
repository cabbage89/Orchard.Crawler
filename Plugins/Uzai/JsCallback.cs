using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Cabbage.Crawler.Plugins.Uzai
{
    public class JsCallback
    {
        public void GetCountries(TextWriter Output, string result)
        {
            Countries = JsonConvert.DeserializeObject<List<UrlData>>(result);
        }

        public void GetConsulate(TextWriter Output, string result)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(Visa));
            using (StringReader rdr = new StringReader(result))
            {
                Visa = (Visa)serializer.Deserialize(rdr);
            }
        }

        public void GetVisaItem(TextWriter Output, string result)
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(VisaItem));
            using (StringReader rdr = new StringReader(result))
            {
                VisaItem = (VisaItem)serializer.Deserialize(rdr);
            }
        }

        public static VisaItem VisaItem { get; set; }

        public static List<UrlData> Countries { get; set; }

        public static Visa Visa { get; set; }
    }

    public class Visa
    {
        public string Country;
        public string Name;
        public string ImgUrl;
        public string Desc;
        public List<Consulate> ConsulateList;
    }

    public class Consulate
    {
        public string name;
        public string desc;
        public List<VisaItem> visaList;
    }

    public class UrlData
    {
        public string url;
        public string text;
    }

    public class VisaItem : UrlData
    {
        public string Country;
        public string City;
        public string Name;
        public string ImgUrl;
        public string 办理费用;
        public string 最长停留时间;
        public string 入境次数;
        public string 办理时长;
        public string 面试;
        public string 有效期;
        public string 友情提示;
        public string 所需材料;
    }
}