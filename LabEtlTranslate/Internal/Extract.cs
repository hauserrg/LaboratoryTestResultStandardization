using LabEtlTranslate.XmlInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabEtlTranslate
{
    public enum RunExtract { On, Off };  //Pulls in new data from CDW/Clarity
    public enum ExtractBy { DateRange, DateRange_KeepNullLoinc, LoincList, FileImport };
    public enum MultilinesYale { JoinLines, Remove } //Yale only

    public class ExtractContext
    {
        private RunExtract runExtract;
        public RunExtract RunExtract { get { return runExtract; } private set { } }

        private ExtractBy extractBy;
        public ExtractBy ExtractBy { get { return extractBy; } private set { } }

        private MultilinesYale multilinesYale;
        public MultilinesYale MultilinesYale { get { return multilinesYale; } private set { } }

        private DateTime startDate;
        public DateTime StartDate { get { return startDate;} private set {} }

        private DateTime endDate;
        public DateTime EndDate { get { return endDate;} private set {} }

        private string importFilePath;
        public string ImportFilePath { get { return importFilePath; } private set { } }

        public ExtractContext(RunExtract extract, DateTime startDate, DateTime endDate, string importFilePath = null
            , ExtractBy importFrom = ExtractBy.LoincList, MultilinesYale multilinesYale = MultilinesYale.Remove)
        {
            this.runExtract = extract;
            this.extractBy = importFrom;
            this.multilinesYale = multilinesYale;
            this.startDate = startDate;
            this.endDate = endDate.AddDays(1);  //add one day because query uses (x < endDate)
            if( importFrom == ExtractBy.FileImport)
                LoadImportFilePath(importFilePath);
        }

        public ExtractContext(ExtractXml xml)
            :this(xml.RunExtract, xml.StartDate, xml.EndDate, xml.ImportFilePath
            , xml.ExtractBy, xml.MultilinesYale) { }

        private void LoadImportFilePath(string importFilePath)
        {
            if (System.IO.File.Exists(importFilePath))
                this.importFilePath = importFilePath;
            else
                throw new Exception("File not found");
        }


    }
}
