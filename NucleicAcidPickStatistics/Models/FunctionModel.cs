using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NucleicAcidPickStatistics
{
    public class FunctionModel
    {
        public string id { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
        public string description { get; set; }
        public string href { get; set; }
        public string pId { get; set; }
        public int seq { get; set; }
        public int status { get; set; }
        public int spread { get; set; }
        public int resourceType { get; set; }
        public List<FunctionModel> children { get; set; }
    }
}