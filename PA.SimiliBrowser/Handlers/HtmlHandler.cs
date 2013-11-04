using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.SimiliBrowser
{
    [Accept("text/html")]
    [Accept("application/html")]
    public partial class HtmlHandler : Component, IDocumentHandler
    {
        public HtmlDocument Document { get; private set; }
        private Action<Uri, string[]> Submit;

        public HtmlHandler()
        {
            this.Document = new HtmlDocument();
        }

        public void Load(StreamReader data, Action<Uri, string[]> Submit)
        {
            this.Submit = Submit;
           // this.Document.Load(data.ReadToEnd());
        }

        public bool SetValueById(string element, string value)
        {
            return this.SetValue(this.Document.GetElementbyId(element), value);
        }

        public bool SetValueByName(string element, string value)
        {
            return this.SetValue(this.Document.DocumentNode.SelectSingleNode("//input[@name='" + element + "']|//select[@name='" + element + "']"), value);
        }

        public bool SetValue(HtmlNode n, string value)
        {
            if (n is HtmlNode && n.Name == "select")
            {
                foreach (HtmlNode o in n.SelectNodes("option"))
                {
                    o.SetAttributeValue("selected", o.GetAttributeValue("value", "").Equals(value) ? "selected" : "");
                }
                return true;
            }

            if (n is HtmlNode && n.Name == "input")
            {
                switch (n.GetAttributeValue("type", ""))
                {
                    case "radio":
                        n.SetAttributeValue("checked", n.GetAttributeValue("value", "").Equals(value) ? "checked" : "");
                        break;
                    default:
                        n.SetAttributeValue("value", value);
                        break;
                }
                n.SetAttributeValue("value", value);
                return true;
            }

            return false;
        }

        public void SetRadioByName(string element, string value)
        {
            foreach (HtmlNode o in this.Document.DocumentNode.SelectNodes(string.Concat("//input[@name='", element, "']")))
            {
                this.SetValue(o, value);
            }
        }

        public void SubmitForm(string buttonName, params string[] additionnalData)
        {
            Dictionary<string, string> formdata = new Dictionary<string, string>();

            foreach (string ad in additionnalData)
            {
                string[] a = ad.Split(new char[] { '=' }, 2);

                if (!formdata.ContainsKey(a[0]))
                {
                    formdata.Add(a[0], a[0] + "=" + (a.Length > 1 ? a[1] : ""));
                }
            }

            foreach (HtmlNode f in this.Document.DocumentNode.SelectNodes("//form"))
            {
                if (buttonName == null || f.SelectSingleNode("//*[@name='" + buttonName + "']") is HtmlNode)
                {
                    HtmlNodeCollection selects = f.SelectNodes("//select[string-length(@name)>0]");

                    if (selects is HtmlNodeCollection)
                    {
                        foreach (HtmlNode s in selects)
                        {
                            string n = s.GetAttributeValue("name", "");

                            foreach (HtmlNode o in s.SelectNodes("option"))
                            {
                                if (n.Length > 0 && o.GetAttributeValue("selected", "").Length > 0 && !formdata.ContainsKey(n))
                                {
                                    formdata.Add(n, n + "=" + o.GetAttributeValue("value", ""));
                                }
                            }
                        }
                    }

                    HtmlNodeCollection inputs = f.SelectNodes("//input[string-length(@name)>0]");

                    if (inputs is HtmlNodeCollection)
                    {
                        foreach (HtmlNode i in inputs)
                        {
                            string n = i.GetAttributeValue("name", "");
                            string v = i.GetAttributeValue("value", "");
                            string t = i.GetAttributeValue("type", "");

                            switch (t)
                            {
                                case "radio":
                                    if (n.Length > 0 && i.GetAttributeValue("checked", "").Length > 0 && !formdata.ContainsKey(n))
                                    {
                                        formdata.Add(n, n + "=" + v);
                                    }
                                    break;

                                case "submit":
                                    if (n.Length > 0 && n.Equals(buttonName) && !formdata.ContainsKey(n))
                                    {
                                        formdata.Add(n, n + "=" + v);
                                    }
                                    break;

                                case "text":
                                case "password":
                                    if (n.Length > 0 && !formdata.ContainsKey(n))
                                    {
                                        formdata.Add(n, n + "=" + v);
                                    }
                                    break;

                                default:
                                    if (n.Length > 0 && v.Length > 0 && !formdata.ContainsKey(n))
                                    {
                                        formdata.Add(n, n + "=" + v);
                                    }
                                    break;
                            }
                        }
                    }

                    if (f.GetAttributeValue("method", "post").ToLower().Equals("post"))
                    {
                        this.Submit(new Uri(f.GetAttributeValue("action", ".")), formdata.Values.ToArray());
                    }

                    if (f.GetAttributeValue("method", "get").ToLower().Equals("get"))
                    {
                        string query = string.Empty;

                        foreach (string fd in formdata.Values)
                        {
                            string[] d = fd.Split(new char[] { '=' }, 2);
                            query += (query.Length > 0 ? "&" : "") + Browser.UpperCaseUrlEncode(d[0]) + "=" + (d.Length > 1 ? Browser.UpperCaseUrlEncode(d[1]) : "");
                        }

                        this.Submit(new Uri(f.GetAttributeValue("action", ".") + (query.Length > 0 ? "?" + query : string.Empty)), null);
                    }

                    break;
                }
            }

        }

        public void PostBack(string target, string argument = "")
        {
            this.SetValueByName("__EVENTTARGET", target);
            this.SetValueByName("__EVENTARGUMENT", argument);
            this.SubmitForm(null);
        }

        public override string ToString()
        {
            return this.Document.ToString();
        }
    }
}
