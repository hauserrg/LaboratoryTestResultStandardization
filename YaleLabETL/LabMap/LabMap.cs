using LabEtlTranslate;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MapResult
{
    internal static partial class EtlMap
    {
        internal static void Map(Translate translate)
        {
            //phase 1: input
            //A.  Excel
            //string path = @"C:\Users\hauserrx\Desktop\YaleLabExtract\Documentation\LabResults.xlsx";
            //var excel = new ExcelShortcut(path);
            //var worksheets = excel.GetWorksheetNames();
            //DataTable distinct = excel.GetWorkSheetData(worksheets[0]);
            //excel.Close();

            //B.  File (use this for debug)
            //DataTable distinct = new DataTable();
            //distinct.Columns.Add("Loinc", typeof(string));
            //distinct.Columns.Add("Result", typeof(string));
            //distinct.Columns.Add("LoincScale", typeof(string));
            //string path = @"C:\Users\hauserrx\Desktop\DistinctMapSimple.txt"; //Requires headers
            ////SharedLibrary.FileIO.RemoveCharactersFromFile(path, new char[] { '"' });  //<- only do once...
            //SharedLibrary.FileIO.ReadDataTableFromFile(ref distinct, path, header: true);

            //C.  database
            DataTable distinct = SqlTable.GetDistinctTable(translate.ConString, translate.Schema
                , "Proj_VAMap"
                , new List<string>() { Column.Loinc.ToString(), Column.Result.ToString(), Column.LoincScale.ToString() });

            //phase 2:  map            
            DataTable mapDistinct = LabResultMap.LabResultMap.GetMap(distinct);

            //phase 3:  load
            //a. debug only - write to file
            //string pathOut = @"C:\Users\hauserrx\Desktop\output.txt";
            //FileIO.WriteDataTableToFile(mapDistinct, pathOut, "\t", false, "");

            //b.  create table + load (Proj_*_ETL_2Transform)
            string tableName = "Proj_VAMap_Map7";
            CreateMapTable(translate, tableName);
            SqlTable.BulkInsertDataTable(translate.ConString, tableName, mapDistinct); //,300
        }
        internal static void CreateMapTable(Translate translate, string tableName)
        {
            var columnNames = new List<string>() { "Loinc", "Result", "LoincScale"
                , "MappedYN", "MapFunc", "Inequality", "Number", "AfterDecimal", "Field1", "Field2"
                , "General", "Pretty" };
            //, primary key (Loinc, Result)
            var columnTypes = new List<string>() { "varchar(50)", "varchar(254)", "varchar(254)"
                , "varchar(1)", "varchar(50)", "varchar(2)", "varchar(100)", "int", "varchar(100)", "varchar(100)"
                , "varchar(30)", "varchar(100)" };
            //var columnTypes = new List<string>() { "varchar(50)", "varchar(254)", "varchar(254)"
            //    , "varchar(1)", "varchar(50)", "varchar(2)", "decimal", "int", "varchar(100)", "varchar(100)"
            //    , "varchar(30)", "varchar(100)" };
            SqlTable.CreateTable(translate.ConString, translate.Database, tableName
                , columnNames, columnTypes);
        }        

//For review:
//select distinct * from [Proj_VAMap] 
//    where Loinc in (
//    '3167-4'
//    ) order by Result

//select * from [Proj_VAMap_Map] where Loinc = '16249-5' and MappedYN = 'N'
//select * from [Proj_VAMap_Map2] where Loinc = '16249-5' and MappedYN = 'N'
//select * from [Proj_VAMap_ReviewHelper1] --loinc, number of unmapped
//select * from [Proj_VAMapSimple]  --100 results
//select * from Proj_Dim_Loinc where Loinc_num = '29114-6'
        
    }
}
