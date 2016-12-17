using LabEtlTranslate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabEtlTranslate.XmlInputs
{
    public class TranslateXml
    {
        public string ProjectName { get; set; }
        public string ReportPath { get; set; }
        public Map Map { get; set; }
        public RunMap RunMap { get; set; }
        public RunReport RunReport { get; set; }
        public RunReportTables RunReportTables { get; set; }
        public ExtractXml ExtractXml { get; set; }
        public Debug Debug { get; set; }

        public TranslateXml() { }
        public TranslateXml(string projectName, string reportPath, Map map, ExtractXml extractXml
            , RunMap runMap, RunReport runReport, RunReportTables RunReportTables, Debug debug)
        {
            this.ProjectName = projectName;
            this.ReportPath = reportPath;
            this.Map = map;
            this.ExtractXml = extractXml;
            this.RunMap = runMap;
            this.RunReport = runReport;
            this.RunReportTables = RunReportTables;
            this.Debug = debug;
        }
    }
}
