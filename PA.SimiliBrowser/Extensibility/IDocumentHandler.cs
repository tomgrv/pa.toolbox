using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.SimiliBrowser
{
    [InheritedExport]
    public interface IDocumentHandler
    {
        void Load(StreamReader data, Action<Uri, string[]> Submit);
    }
}
