using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;
using System.ComponentModel.Composition;
using System.Globalization;

namespace PA.SimiliBrowser
{
    public partial class Browser : Component
    {
        private CookieCollection cookies = new CookieCollection();
        private HtmlDocument htmld = new HtmlDocument();

        [ImportMany]
        private IEnumerable<IDocumentHandler> handlers;

        [Browsable(false)]
        public IDocumentHandler CurrentHandler
        {
            get ; private set; 
        }

        [Browsable(false)]
        [Obsolete]
        public HtmlDocument CurrentDocument
        {
            get { return this.htmld; }
        }

        private string data;

        [Browsable(false)]
        [Obsolete]
        public string DownloadedData
        {
            get { return this.data; }
        }

        private Uri page;

        [Browsable(false)]
        public string CurrentPage
        {
            get { return this.page is Uri ? this.page.AbsoluteUri : null; }
        }

        private string accept;

        [DefaultValue("*/*;q=0.8")]
        public string HttpAccept
        {
            get { return this.accept is string && this.accept.Length > 0 ? this.accept : "*/*;q=0.8"; }
            set { this.accept = value; }
        }

        public Browser()
        {
            InitializeComponent();
        }

        public void GetIfNot(string u)
        {
            if (this.CurrentPage != u)
            {
                this.Get(new Uri(u));
            }
        }

        public void Submit(Uri uri, string[] formdata)
        {
            Uri u = new Uri(this.page, uri);

            string query = null;

            if (formdata is string[])
            {
                query = string.Empty;

                foreach (string fd in formdata)
                {
                    string[] d = fd.Split(new char[] { '=' }, 2);
                    query += (query.Length > 0 ? "&" : "") + UpperCaseUrlEncode(d[0]) + "=" + (d.Length > 1 ? UpperCaseUrlEncode(d[1]) : "");
                }
            }

            HttpWebRequest wr = CreateRequest(u, query);

            using (HttpWebResponse response = wr.GetResponse() as HttpWebResponse)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:

                        this.page = response.ResponseUri;
                        this.LoadHttpCookies(response);
                        this.LoadHttpData(response);
                        break;

                    case HttpStatusCode.Found:

                        this.page = response.ResponseUri;
                        this.LoadHttpCookies(response);
                        this.Submit(new Uri(this.page, response.Headers.Get("Location")), null);
                        break;

                    default:
                        throw new HttpUnhandledException();
                }
            }
        }

        public void Get(string u)
        {
            this.Get(new Uri(u));
        }

        public void Get(string u, params string[] formdata)
        {
            this.Get(new Uri(this.page, u), formdata);
        }

        public void Get(Uri u, params string[] formdata)
        {
            this.data = string.Empty;
            string query = string.Empty;

            foreach (string fd in formdata)
            {
                string[] d = fd.Split(new char[] { '=' }, 2);
                query += (query.Length > 0 ? "&" : "") + UpperCaseUrlEncode(d[0]) + "=" + (d.Length > 1 ? UpperCaseUrlEncode(d[1]) : "");
            }

            this.page = new Uri(u, (query.Length > 0 ? "?" + query : ""));

            HttpWebRequest wr = CreateRequest(this.page, null);
            wr.AllowAutoRedirect = true;

            using (HttpWebResponse response = wr.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Found)
                {
                    throw new OperationCanceledException("http error: <" + response.StatusCode + ">" + response.StatusDescription);
                }

                this.LoadHttpCookies(response);
                this.LoadHttpData(response);
            }
        }

       

        public void Post(string u)
        {
            this.Post(new Uri(u));
        }

        public void Post(string u, string query)
        {
            this.Post(new Uri(u), query);
        }

        public void Post(Uri u, params string[] formdata)
        {
            string query = "";

            foreach (string fd in formdata)
            {
                string[] d = fd.Split(new char[] { '=' }, 2);
                query += (query.Length > 0 ? "&" : "") + UpperCaseUrlEncode(d[0]) + "=" + (d.Length > 1 ? UpperCaseUrlEncode(d[1]) : "");
            }

            this.Post(u, query);
        }

        public void Post(Uri u, string query)
        {
            this.data = "";


            HttpWebRequest wr = CreateRequest(u, query);

            using (HttpWebResponse response = wr.GetResponse() as HttpWebResponse)
            {
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:

                        this.page = response.ResponseUri;
                        this.LoadHttpCookies(response);
                        this.LoadHttpData(response);
                        break;

                    case HttpStatusCode.Found:

                        this.page = response.ResponseUri;
                        this.LoadHttpCookies(response);
                        this.Get(new Uri(this.page, response.Headers.Get("Location")));
                        break;

                    default:
                        throw new HttpUnhandledException();
                }
            }
        }



        private HttpWebRequest CreateRequest(Uri u, string data)
        {
            HttpWebRequest wr = HttpWebRequest.Create(u) as HttpWebRequest;

            wr.Accept = this.accept is string && this.accept.Length > 0 ? this.accept + "," : "" + "*/*;q=0.8";
            wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            //wr.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            wr.KeepAlive = true;
            wr.Referer = this.page is Uri ? this.page.AbsoluteUri : "";
            wr.AllowAutoRedirect = false;
            wr.CachePolicy = new System.Net. Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

            if (this.cookies.Count > 0)
            {
                wr.CookieContainer = new CookieContainer();
                wr.CookieContainer.Add(this.cookies);
            }

            if (data is string && data.Length > 0)
            {
                wr.Method = "POST";

                if (data.StartsWith("{") && data.EndsWith("}"))
                {
                    wr.Accept = "application/json";
                    wr.ContentType = "application/json";
                }
                else
                {
                    wr.ContentType = "application/x-www-form-urlencoded";
                }

                wr.ContentLength = data.Length;

                using (Stream ws = wr.GetRequestStream())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(data);
                    ws.Write(bytes, 0, bytes.Length);
                }
            }

            return wr;
        }

        private IDocumentHandler LoadHttpHandler(string content)
        {
            foreach (IDocumentHandler dh in this.handlers)
            {
                foreach (AcceptAttribute aa in dh.GetType().GetCustomAttributes(typeof(AcceptAttribute), true))
                {
                    if (content.StartsWith(aa.Accept, true, CultureInfo.InvariantCulture))
                    {
                        return dh;
                    }
                }
            }

            return null;
        }

        private void LoadHttpData(HttpWebResponse response)
        {
            this.CurrentHandler = this.LoadHttpHandler(response.ContentType);

            if (response.ResponseUri.Scheme == "http" && this.CurrentHandler is IDocumentHandler)
            {
                using (Stream s = response.GetResponseStream())
                {
                    Encoding encode = FindEncoding(response);

                    using (StreamReader sr = new StreamReader(s, encode))
                    {
                        this.CurrentHandler.Load(sr, this.Submit);
                    }
                }
            }

            if (this.data.Length > 0 && response.ContentType.Contains("html"))
            {
                this.htmld.LoadHtml(this.data);
            }
        }

        private void LoadHttpCookies(HttpWebResponse wr)
        {
            string cookiestring = wr.Headers.Get("Set-Cookie");

            if (cookiestring is string && cookiestring.Length > 0)
            {
                string[] parts = cookiestring.Split(';');
                string[] data = parts[0].Split('=');
                string[] host = this.page.Host.Split('.');

                Cookie c = new Cookie(data[0], data[1], "/", "." + string.Join(".", host, host.Length - 2, 2));

                for (int i = 1; i < parts.Length; i++)
                {
                    data = parts[i].Split('=');

                    switch (data[0].Trim().ToLower())
                    {
                        case "path":
                            c.Path = data[1];
                            break;
                        case "domain":
                            c.Domain = data[1];
                            break;
                        default:
                            break;
                    }
                }



                this.cookies.Add(c);
            }
        }

        public static string UpperCaseUrlEncode(string s)
        {
            char[] temp = HttpUtility.UrlEncode(s).ToCharArray();
            for (int i = 0; i < temp.Length - 2; i++)
            {
                if (temp[i] == '%')
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
            return new string(temp);
        }



        private static Encoding FindEncoding(HttpWebResponse wr)
        {
            foreach (string cts in wr.ContentType.Split(';'))
            {
                if (cts.Trim().StartsWith("charset="))
                {
                    return System.Text.Encoding.GetEncoding(cts.Trim().Substring(8).Trim());
                }
            }

            return System.Text.Encoding.UTF8;
        }
    }
}
