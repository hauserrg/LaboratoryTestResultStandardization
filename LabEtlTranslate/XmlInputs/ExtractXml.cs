using LabEtlTranslate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabEtlTranslate.XmlInputs
{
    public class ExtractXml
    {
        public RunExtract RunExtract { get; set; }
        public ExtractBy ExtractBy { get; set; }
        public MultilinesYale MultilinesYale { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ImportFilePath { get; set; }

        ExtractXml() { }

        public ExtractXml(RunExtract runExtract, DateTime startDate, DateTime endDate
            , string importFilePath, ExtractBy extractBy, MultilinesYale multilinesYale)
        {
            this.RunExtract = runExtract;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.ImportFilePath = importFilePath;
            this.ExtractBy = extractBy;
            this.MultilinesYale = multilinesYale;
        }
    }
}
