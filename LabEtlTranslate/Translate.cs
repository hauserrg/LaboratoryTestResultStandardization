using LabEtlTranslate.XmlInputs;
using log4net;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace LabEtlTranslate
{
    public enum Map { Yale, VA };  //input the type of map
    public enum Database { Yale, VA };
    public enum Schema { Yale, VA };
    public enum Tables { Extract, Transform, Load, ReportResults, LabList, ReportExtr1, ReportExtr2
        , ReportMapFunc, ReportGlobalSum, ReportResultBySite }
    public enum Column { Loinc, LoincScale, Result, Abnormal, LabName, LabId };
    public enum Debug { On, Off };  //Only reports unmapped
    public enum RunMap { On, Off };
    public enum RunReport { On, Off };
    public enum RunReportTables {On, Off};

    public class Translate
    {
        public string InputTable { get; set; }
        public string OutputTable {get; set;}
        private Map map;
        public Map Map
        {
            get { return map; }
            private set { }
        }
        
        private Debug debug;
        public Debug Debug { get { return debug; } private set { } }

        private RunMap runMap;
        public RunMap RunMap { get { return runMap; } private set { } }

        private RunReport runReport;
        public RunReport RunReport { get { return runReport; } private set { } }
        public RunReportTables RunReportTables;

        private string conString;
        public string ConString
        {
            get { return conString; }
            set { conString = value; }
        }

        private string localDbConString;
        public string LocalDbConString
        {
            get { return localDbConString; }
            private set { }
        }

        public ExtractContext extract;

        private string importLab_InitialDir;
        public string ImportLab_InitialDir{ get { return importLab_InitialDir; } private set {} }

        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            private set { }
        }

        private string database;
        public string Database
        {
            get { return database; }
            private set { }
        }

        private string schema;
        public string Schema
        {
            get { return schema; }
            private set { }
        }

        private string reportName;
        public string ReportName
        {
            get { return reportName; }
            private set { }
        }

        private string reportTitle;
        public string ReportTitle { get { return reportTitle; } private set { } }

        private string reportLocation;
        public string ReportLocation { get { return reportLocation; } private set { } }

        private string reportSubdir;
        public string  ReportSubdir { get { return reportSubdir; } private set { } }

        private Image reportImage;
        public Image ReportImage { get { return reportImage; } private set { } }

        private string reportExtrCriteria;
        public string ReportExtrCriteria { get { return reportExtrCriteria; } private set { } }

        public string ReportMergeFile { get { return ReportPath + ProjectName + ".pdf"; } }
        public string DataFileLabList { get { return ReportPath + projectName + "_LabList.txt"; } private set { } }
        public string DataFileTransform { get { return ReportPath + projectName + "_Transform.txt"; } private set { } }
        public string DataFileClean { get { return ReportPath + projectName + "_Clean.txt"; } private set { } }
        public string DataFileExtr1 { get { return ReportPath + projectName + "_Extr1.txt"; } private set { } }
        public string DataFileExtr2 { get { return ReportPath + projectName + "_Extr2.txt"; } private set { } }
        public string DataFileGlobalSum { get { return ReportPath + projectName + "_GlobalSum.txt"; } private set { } }
        public string DataFileMapFunc { get { return ReportPath + projectName + "_MapFunc.txt"; } private set { } }
        public string DataFileVarByLab { get { return ReportPath + projectName + "_ReportVarByLab.txt"; } private set { } }
        public string DataFilePosPct { get { return ReportPath + projectName + "_ReportPosPct.txt"; } private set { } }
        public string DataFileLoincScale { get { return ReportPath + projectName + "_LoincScaleCheck.txt"; } private set { } }

        public string DirtyDataPath { get { return ReportPath + projectName + "_Dirty.txt"; } private set { } }

        private string reportPath;
        public string ReportPath
        {
            get { return this.reportPath; }
            private set { /*WinDir.CreateFolder(value); this.reportPath = value;*/ }
        }
        private Dictionary<string, string> columns;
        public ILog log;

        //constructor
        public Translate(string projectName, string reportPath, Map map, ExtractContext extract, RunMap runMap
            , RunReport runReport, RunReportTables runReportTables, Debug debug)
        {
            this.projectName = projectName;
            ReportPath = reportPath;
            this.map = map;
            this.debug = debug;

            //change to switch parameter
            //Non-service
            //this.localDbConString = String.Format(@"Data Source={0}\Resource\Loinc.sdf",Environment.CurrentDirectory);
            //service
            this.localDbConString = String.Format(@"Data Source={0}\Resource\Loinc.sdf", AppDomain.CurrentDomain.BaseDirectory);

            this.extract = extract;
            this.runMap = runMap;
            this.runReport = runReport;
            this.RunReportTables = runReportTables;
            LoadLog();
            LoadConString();
            LoadDatabase();
            LoadColumns();
            LoadSchema();
            LoadReportSubdir();
            LoadReportName();
            LoadReportTitle();
            LoadReportLocation();
            LoadReportImage();
            LoadReportExtrCriteria();
            LoadImportLab_InitialDir();

            if( extract.RunExtract == RunExtract.On)
                new DbProject(this.ProjectName, this.ConString, dropIfExists: true);            
        }

        private void LoadLog()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "App.config"));
            this.log = LogManager.GetLogger("log");
        }
        public Translate(TranslateXml xml, ExtractContext extract, ILog log)
            : this(xml.ProjectName, xml.ReportPath, xml.Map, extract, xml.RunMap, xml.RunReport, xml.RunReportTables
            , xml.Debug) { }
        public static Translate TranslateFromXml(TranslateXml xml)
        {
            ExtractXml extractXml = xml.ExtractXml;
            ExtractContext extract = new ExtractContext(extractXml);

            Translate translateFromXml = new Translate(xml.ProjectName, xml.ReportPath
                , xml.Map, extract, xml.RunMap, xml.RunReport, xml.RunReportTables, xml.Debug);
            return translateFromXml;
        }

        //functions
        public string Get(Tables table)
        {
            switch (table)
	        {
                case Tables.Extract:
                    return "Proj_" + projectName + "_ETL_1Extract";
                case Tables.Transform:
                    return "Proj_" + projectName + "_ETL_2Transform";
                case Tables.Load:
                    return "Proj_" + projectName + "_ETL_3Load";
                case Tables.ReportResults:
                    return "Proj_" + projectName + "_ETLReport_Results";
                case Tables.LabList:
                    return "Proj_" + projectName + "_ETL_LabList";
                case Tables.ReportExtr1:
                    return "Proj_" + projectName + "_ETLReport_Extraction1";
                case Tables.ReportExtr2:
                    return "Proj_" + projectName + "_ETLReport_Extraction2";
                case Tables.ReportMapFunc:
                    return "Proj_" + projectName + "_ETLReport_MapFunc";
                case Tables.ReportGlobalSum:
                    return "Proj_" + projectName + "_ETLReport_GlobalSum";
                case Tables.ReportResultBySite:
                    return "Proj_" + projectName + "_ETLReport_ResultsBySite";
		        default:
                    throw new Exception("Table name not found.");
	        } 
        }       
        public string Get(Column column)
        {
            return columns[column.ToString()];
        }

        private void LoadConString()
        {
            switch (map)
            {
                case Map.Yale:
                    conString = SharedLibrary.ConMgr.Get(Db.RedsDev);
                    break;
                case Map.VA:
                    //conString = "Data Source=r04phidwh50; Initial Catalog=PCS_LABMed; Integrated Security=True;";  //For some reason ConMgr.Get(Db.Va) -> RedsDev???
                    conString = "Data Source=VHACONSQLE; Initial Catalog=BUILD_UPDATES_RAW; Integrated Security=True;";
                    break;
                default:
                    throw new Exception("ConString not found.");
            }
        }
        private void LoadDatabase()
        {
            switch (map)
            {
                case Map.Yale:
                    database = "Reds3_Dev";
                    break;
                case Map.VA:
                    database = "PCS_LabMed";
                    break;
                default:
                    throw new Exception("Database not found.");
            }
        }
        private void LoadColumns()
        {
            switch (map)
            {
                case Map.Yale:
                    columns = LoadDictionary.Resource(Resources.Resource.ColumnMapYale, false);
                    break;
                case Map.VA:
                    columns = LoadDictionary.Resource(Resources.Resource.ColumnMapVA, false);
                    break;
                default:
                    break;
            }
        }
        private void LoadSchema()
        {
            switch (map)
            {
                case Map.VA:
                    schema = "Dflt";
                    return;
                case Map.Yale:
                    schema = "Dbo";
                    return;
                default:
                    throw new Exception("Schema not found.");
            }

        }
        private void LoadReportSubdir()
        {
            string subdir = "Reports";
            reportSubdir = ReportPath + subdir;
            if (!System.IO.Directory.Exists(reportSubdir))
                System.IO.Directory.CreateDirectory(reportSubdir);
            reportSubdir = ReportPath + subdir + @"\";
        }
        private void LoadReportName()
        {
            reportName = "Proj_" + projectName + "_DataQuality.pdf";
        }
        private void LoadReportTitle()
        {
            switch (map)
            {
                case Map.Yale:
                    reportTitle = "Lab Informatics";
                    break;
                case Map.VA:
                    reportTitle = "VA Informatics";
                    break;
                default:
                    throw new Exception("Report title not found.");
            }
        }
        private void LoadReportLocation()
        {
            switch (map)
            {
                case Map.Yale:
                    reportLocation = "New Haven";
                    break;
                case Map.VA:
                    reportLocation = "West Haven";
                    break;
                default:
                    throw new Exception("Report location not found.");
            }
        }
        private void LoadReportImage()
        {
            switch (map)
            {
                case Map.Yale:
                    reportImage = Resources.Resource.YNHHLogo;
                    break;
                case Map.VA:
                    reportImage = Resources.Resource.US_DeptOfVeteransAffairs_Seal;
                    break;
                default:
                    throw new Exception("Report image path not found.");
            }
        }
        private void LoadReportExtrCriteria()
        {
            //string startDate = ((DateTime)(dtArrayMap[2].Rows[0]["startDate"])).ToShortDateString();
            //string endDate = ((DateTime)(dtArrayMap[2].Rows[0]["endDate"])).ToShortDateString();
            //string extractCriteria = startDate + " - " + endDate + " Inclusive";
            reportExtrCriteria = "reportExtrCriteria";
        }
        private void LoadImportLab_InitialDir()
        {
            switch (Map)
	        {
		        case Map.Yale:
                    importLab_InitialDir = @"C:\Users\hauserrx\Desktop\";
                    break;
                case Map.VA:
                    importLab_InitialDir = @"C:\Users\vhaconhauser\Desktop\";
                    break;
                default:
                    throw new Exception("Unrecognized Map.");
	        }
        }
    }
}
