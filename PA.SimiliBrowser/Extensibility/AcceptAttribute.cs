using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.SimiliBrowser
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true), MetadataAttribute]
    public class AcceptAttribute : Attribute
    {
        public string Accept { get; private set; }

        public AcceptAttribute(string Accept)
        {
            this.Accept = Accept;
        }
    }
}
