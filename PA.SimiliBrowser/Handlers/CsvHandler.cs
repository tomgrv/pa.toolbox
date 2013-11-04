using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.SimiliBrowser
{
    [Accept("text/csv")]
    public partial class CsvHandler : Component, IDocumentHandler
    {
        public string Document { get; private set; }

        public CsvHandler()
        {
           
        }

        public void Load(StreamReader data,Action<Uri, string[]> Submit)
        {
            this.Document = data.ReadToEnd();
        }
    }
}
