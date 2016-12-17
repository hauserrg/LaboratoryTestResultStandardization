//using SharedLibrary;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Text;

//namespace LabResultMap.LabList
//{
//    /*  The LabList contains a unique list of loinc codes and their descriptors.
//     * 
//     *  It may be created initially, if a list of loincs is used for extraction.
//     *  
//     *  Or, it may occur after the extraction, using the distinct loinc codes of 
//     *  the output.
//     * 
//     * */
//    public class LabList
//    {
//        //feeder method #1:  User selects file list
//        public static void LoadLabListFromFile(Translate translate)
//        {
//            List<string> loincList = GetLabsFromFile(translate);
//            DataTable loincTable = CreateLoincTable(loincList.ToArray(), translate);
//            LoadLoincTable(loincTable, translate);   
//        }
        

//        //feeder method #2:  Import list from extracted results
//        public static void LoadLabListFromExtract(Translate translate)
//        {
//            //if table exists return //feeder method #1 was used.
//            if (SqlTable.TableExists(translate.ConString, translate.Database, translate.Get(Tables.LabList)))
//                return;
            
//            string[] loincList = GetLabsFromDb(translate);
//            DataTable loincTable = CreateLoincTable(loincList, translate);
//            LoadLoincTable(loincTable, translate);   
//        }

//        private static string[] GetLabsFromDb(Translate translate)
//        {
//            string query = String.Format(ResLabList.LoincListFromExtract
//                , translate.Get(Tables.Extract), translate.Get(Column.Loinc));
//            DataTable loincListDt = SqlTable.GetTable(query, translate.ConString);
//            string[] loincList = loincListDt.AsEnumerable().Select(x => x["Loinc"].ToString()).ToArray();
//            return loincList;
//        }

//        //common functions used by both feeder methods (CreateLoincTable and LoadLoincTable)
//        #region CreateLoincTable()
//        private static DataTable CreateLoincTable(string[] loincList, Translate translate)
//        {
//            if (!SqlTableCe.TableExists(translate.LocalDbConString, "Loinc"))
//            {
//                translate.log.Debug("Loading the loinc database table.");
//                CreateLoincTable(translate);
//            }

//            CreateUserTable(loincList, translate);

//            string query = ResLabList.LoincLookup;
//            DataTable loincTable = SqlTableCe.GetTable(query, translate.LocalDbConString);
//            return loincTable;
//        }
//        private static void CreateUserTable(string[] loincList, Translate translate)
//        {
//            string table = "UserLoincs";
//            List<string> columnNames = new List<string>() { "LOINC_NUM" };
//            List<string> columnTypes = new List<string>() { "nvarchar(254)" };

//            if (SqlTableCe.TableExists(translate.LocalDbConString, table))
//                SqlTableCe.DropTable(translate.LocalDbConString, table);
//            SqlTableCe.CreateTable(translate.LocalDbConString, "Loinc", table, columnNames, columnTypes);

//            //load data
//            DataTable loinc = new DataTable();
//            loinc.Columns.Add(columnNames[0], typeof(string));

//            foreach (var l in loincList)
//            {
//                DataRow r = loinc.NewRow();
//                r[columnNames[0]] = l;
//                loinc.Rows.Add(r);
//            }

//            SqlTableCe.BulkInsert(loinc, table, translate.LocalDbConString);
//        }
//        public static void CreateLoincTable(Translate translate)
//        {
//            List<string> columnNames = new List<string>() { 
//                "LOINC_NUM", "COMPONENT", "PROPERTY", "TIME_ASPCT", "SYSTEM", "SCALE_TYP"
//                , "METHOD_TYP", "CLASS", "SOURCE", "DATE_LAST_CHANGED", "STATUS", "SHORTNAME", "LONG_COMMON_NAME"
//            };

//            List<string> columnTypes = new List<string>() { "nvarchar(254), primary key(LOINC_NUM)", "nvarchar(254)", "nvarchar(254)", "nvarchar(254)", "nvarchar(254)", "nvarchar(254)"
//                , "nvarchar(254)", "nvarchar(254)", "nvarchar(254)", "nvarchar(254)", "nvarchar(254)", "nvarchar(254)", "nvarchar(500)"};

//            SqlTableCe.CreateTable(translate.LocalDbConString, "Loinc", "Loinc", columnNames, columnTypes);
//            CreateDbTable_Load(translate, columnNames);
//        }
//        private static void CreateDbTable_Load(Translate translate, List<string> columnNames)
//        {
//            //looks for a file on the desktop called loinc.csv
//            string path = translate.ImportLab_InitialDir + "loinc.csv";


//            DataTable loinc = new DataTable();
//            foreach (var column in columnNames)
//                loinc.Columns.Add(column, typeof(string));  //Assume all strings.


//            FileIO.ReadDataTableFromFile(ref loinc, path, ',', header: true);
//            SqlTableCe.BulkInsert(loinc, "Loinc", translate.LocalDbConString);
//        }
//        #endregion
//        private static void LoadLoincTable(DataTable loincTable, Translate translate)
//        {
//            //Todo Handle invalid loinc codes
//            var columnNames = new List<string>() { "Loinc", "Component", "ScaleType", "ShortName" };
//            var columnTypes = new List<string>() { "varchar(50), primary key(Loinc)", "varchar(250)"
//                , "varchar(70)", "varchar(254)" };
//            SqlTable.CreateTable(translate.ConString, translate.Database, translate.Get(Tables.LabList), columnNames, columnTypes);
//            SqlTable.BulkInsertDataTable(translate.ConString, translate.Get(Tables.LabList), loincTable);
//        }

//        //specific to feeder method #1
//        private static List<string> GetLabsFromFile(Translate translate)
//        {
//            List<string> loincList = new List<string>();
//            //Expecting...
//            //LOINC: A list of Loinc codes (i.e. '101-8\n205-2\n') 

//            string[] files = null;
//            while (files == null)
//                files = WinDir.SelectFiles(translate.ImportLab_InitialDir, title: "Select you loinc list.");

//            foreach (var file in files)
//            {
//                string[] lines = System.IO.File.ReadAllLines(file);
//                foreach (var line in lines)
//                {
//                    string[] words = line.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
//                    if (words.Length > 1)
//                        break;
//                    loincList.Add(words[0]);
//                }
//            }
//            return loincList;
//        }
//    }
//}
