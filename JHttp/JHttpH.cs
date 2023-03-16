using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JHttp
{
    /// <summary>
    /// http对象
    /// </summary>
    public class JHttpH
    {
        #region 常量
        const string defAccept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7";
        const string defAccept_Encoding = "gzip, deflate";
        const string defAccept_Language = "Accept_Language";
        const string defCache_Control = "no-cache";
        const string defConnection = "keep-alive";
        const string defHost = "";
        const string defPragma = "no-cache";
        const string defUpgrade_Insecure_Requests = "1";
        const string defUser_Agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
        #endregion
        public string Accept { get; set; } = defAccept;
        public string Accept_Encoding { get; set; } = defAccept_Encoding;
        public string Accept_Language { get; set; } = defAccept_Language;
        public string Cache_Control { get; set; } = defCache_Control;
        public string Connection { get; set; } = defConnection;
        public Dictionary<string, string> Cookie { get; set; } = new Dictionary<string, string>();
        public string Host { get; set; } = defHost;
        public string Pragma { get; set; } = defPragma;
        public string Upgrade_Insecure_Requests { get; set; } = defUpgrade_Insecure_Requests;
        public string User_Agent { get; set; } = defUser_Agent;

        public string Get(string url)
        {
            return Get(url, null, null);
        }
        public string Get(string url, object bodys)
        {
            return Get(url, null, bodys);
        }
        public string Get(string url, object headers, object bodys)
        {
            var bodysDic = UnnamedHelper.ObjToDic<string>(bodys);
            var urlF = $"{url}{(bodysDic.Count > 0 ? "?" : "")}{string.Join("&", bodysDic.Select(a => $"{a.Key}={HttpUtility.UrlEncode(a.Value)}"))}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlF);
            var header = defaultHeader();
            header.CopyFrom(headers);
            var headerkv = toHeaderDic(header.ToDic<string>());
            headerkv.Select(a => {
                SetHeaderValue(request.Headers, a.Key, a.Value);
                return 0;
            }).ToArray();
            WebResponse req;
            try
            {
                req = request.GetResponse();
            }
            catch (WebException ex)
            {
                req = ex.Response;
            }
            processHeader(req);
            var stream = req.GetResponseStream();
            var encoding = req.Headers["Content-Encoding"];
            var data = new byte[0];
            if (encoding.Contains("gzip"))
            {
                data = decompress(stream);
            }
            else {
                data=new byte[stream.Length];
                stream.Read(data,0,data.Length);
            }

            var contentType = req.Headers["Content-Type"];//text/html; charset=utf-8
            var reg = new System.Text.RegularExpressions.Regex(@"^\s*(.+?)\s*\/\s*(.+?)\s*(?:;\s*charset\s*=\s*(.+?)){0,1}\s*$");
            var match = reg.Match(contentType);
            var textencoding = "utf-8";
            if (match.Success && match.Groups.Count==4 && match.Groups[3].Value!="") { 
                textencoding = match.Groups[3].Value;
            }
            return Encoding.GetEncoding(textencoding).GetString(data);
        }

        private void processHeader(WebResponse req) {
            var hdic = req.Headers.AllKeys.ToDictionary(a => a, a => req.Headers[a]);
            var setCookie = req.Headers["Set-Cookie"] + "";
            var reg = new System.Text.RegularExpressions.Regex(@"(?<!expires\=[A-Z][a-z]{2})\,");
            var cookies = reg.Split(setCookie);
            cookies.ToList().ForEach(a => {
                var cps = a.Split(';');
                var cp=cps[0];
                var findex = cp.IndexOf("=");
                if (findex < 0)
                {
                    return;
                }
                var key = cp.Substring(0,findex);
                var value = cp.Substring(findex+1);
                if (key == "")
                {
                    return;
                }
                Cookie[key] = value;
            });
        }

        private Dictionary<string, T> toHeaderDic<T>(Dictionary<string, T> dic)
        {
            return dic.ToDictionary(a=> a.Key.Replace("_", "-"),a=>a.Value);
        }
        private object defaultHeader()
        {
            return new
            {
                Accept = Accept,
                Accept_Encoding = Accept_Encoding,
                Accept_Language = Accept_Language,
                Cache_Control = Cache_Control,
                Connection = Connection,
                Cookie = string.Join("; ", Cookie.Select(a => $"{a.Key}={a.Value}")),
                Host = Host,
                Pragma = Pragma,
                Upgrade_Insecure_Requests = Upgrade_Insecure_Requests,
                User_Agent = User_Agent,
            };
        }

        public void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }

        //解压缩字节
        //1.创建被压缩的数据流
        //2.创建zipStream对象，并传入解压的文件流
        //3.创建目标流
        //4.zipStream拷贝到目标流
        //5.返回目标流输出字节
        private static byte[] decompress(Stream compressStream)
        {
            using (var zipStream = new GZipStream(compressStream, CompressionMode.Decompress))
            {
                using (var resultStream = new MemoryStream())
                {
                    zipStream.CopyTo(resultStream);
                    return resultStream.ToArray();
                }
            }
        }
    }

    /// <summary>
    /// 匿名对象转换
    /// </summary>
    internal static class UnnamedHelper
    {
        public static Dictionary<string, T> ToDic<T>(this object obj)
        {
            return ObjToDic<T>(obj);
        }
        /// <summary>
        /// 对象转字典
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Dictionary<string, T> ObjToDic<T>(object obj)
        {
            var res = new Dictionary<string, T>();
            if (obj == null)
            {
                return res;
            }
            var type = obj.GetType();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                object val = prop.GetValue(obj);
                T valt = val == null || !(val is T) ? default(T) : (T)val;
                if (typeof(T) == typeof(string))
                {
                    valt = (T)(object)(val + "");
                }
                res[prop.Name] = valt;
            }

            return res;
        }
    }

    internal static class ModelHelper
    {
        public static void CopyFrom<T1, T2>(this T1 obj, T2 obj2) where T1 : class, new() where T2 : class, new()
        {
            if (obj2 == null)
            {
                return;
            }
            var t2props = obj2.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance).ToDictionary(a => a.Name.ToLower());
            if (t2props.Count <= 0)
            {
                return;
            }
            var t1props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.IgnoreCase | BindingFlags.Instance);
            foreach (var prop in t1props)
            {
                try
                {
                    var name = prop.Name.ToLower();
                    var pf = obj.GetType().GetField($"<{prop.Name}>i__Field", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (!t2props.ContainsKey(name))
                    {
                        continue;
                    }
                    var prop2 = t2props[name];
                    var p1type = prop.PropertyType;
                    var p2type = prop2.PropertyType;

                    var p2v = prop2.GetValue(obj2);
                    if (p1type == p2type)
                    {
                        //prop.SetValue(obj, p2v);
                        if (prop.SetMethod == null && pf != null)
                        {
                            pf.SetValue(obj, p2v);
                        }
                        else if (prop.SetMethod != null)
                        {
                            prop.SetValue(obj, p2v);
                        }
                        continue;
                    }
                    bool p1null = false;
                    bool p2null = false;
                    if (p1type.FullName.StartsWith("System.Nullable") && p1type.GenericTypeArguments != null && p1type.GenericTypeArguments.Length > 0)
                    {
                        p1type = p1type.GenericTypeArguments[0];
                        p1null = true;
                    }
                    if (p2type.FullName.StartsWith("System.Nullable") && p2type.GenericTypeArguments != null && p2type.GenericTypeArguments.Length > 0)
                    {
                        p2type = p2type.GenericTypeArguments[0];
                        p2null = true;
                    }
                    if (p1type != p2type)
                    {
                        continue;
                    }
                    if (!p1null && p2null && p2v == null)
                    {
                        p2v = Activator.CreateInstance(p2type);
                    }
                    //prop.SetValue(obj, p2v);
                    if (prop.SetMethod == null && pf != null)
                    {
                        pf.SetValue(obj, p2v);
                    }
                    else if (prop.SetMethod != null)
                    {
                        prop.SetValue(obj, p2v);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

            }
        }
    }
}
