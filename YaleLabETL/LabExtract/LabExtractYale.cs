using LabEtlTranslate;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace MapResult
{
    /********************  Yale ********************************************/
    internal static partial class LabExtract
    {
        /********  Yale Functions  *********/
        private static void ExtractYale(Translate translate)
        {
            //extract       
            switch (translate.extract.ExtractBy)
            {
                case ExtractBy.DateRange:
                    Extract_Yale_Standard(translate);
                    break;
                case ExtractBy.DateRange_KeepNullLoinc:
                    Extract_Yale_Standard_NullLoinc(translate);
                    break;
                //case ExtractBy.LoincList:  --Deprecated
                //    Extract_Yale_LabList(translate);
                //    break;
                case ExtractBy.FileImport:
                    Extract_Yale_FileImport(translate);
                    break;
                default:
                    throw new Exception("Unrecognized import.");
            }
        }

        //Note:  The first two functions are wrapper functions for the second.
        private static void Extract_Yale_Standard_NullLoinc(Translate translate)
        {
            Extract_Yale_Standard(translate, Resource.Resource.YaleLabQuery_NullLoinc);
            //ToDo:  Add Loinc to table here.
        }
        private static void Extract_Yale_Standard(Translate translate)
        {
            Extract_Yale_Standard(translate, Resource.Resource.YaleLabQuery);
        }
        private static void Extract_Yale_Standard(Translate translate, string query)
        {
            TimeSpan ts = translate.extract.EndDate - translate.extract.StartDate;
            int daysBetween = ts.Days;
            if (daysBetween < 0) throw new Exception("Start and end dates are reversed.");
            if (daysBetween > maxDaysForSingleQuery)
            {
                //1.  Reformat queries
                string createTableQuery;
                string runQuery;
                ReformatQueries(query, out createTableQuery, out runQuery);

                //2.  Create table
                createTableQuery = String.Format(createTableQuery, translate.Get(Tables.Extract)
                    , translate.extract.StartDate, translate.extract.EndDate);
                SqlTable.ExecuteNonQuery(translate.ConString, createTableQuery);

                //3.  Run query for each date
                string conString = translate.ConString;
                string parameterizedQuery = runQuery;
                var parameters = new List<string> { translate.Get(Tables.Extract) };
                DateTime startDate = translate.extract.StartDate;
                DateTime endDate = translate.extract.EndDate;
                var insertStartEndDatesAt = new List<int> { 1, 2 };

                SqlTable.ExecuteNonQuery_OneDayAtATime(conString, parameterizedQuery
                    , parameters, startDate, endDate, insertStartEndDatesAt);
            }
            else
            {
                //run query once
                string queryFinal = String.Format(query, translate.Get(Tables.Extract)
                    , translate.extract.StartDate, translate.extract.EndDate);
                SqlTable.ExecuteNonQuery(translate.ConString, queryFinal);
            }
        }
        private static void ReformatQueries(string query, out string createTableQuery, out string runQuery)
        {
            //run query multiple times, once for each day
            //Note:  The following code makes certain assumptions about the query.
            //A:  The query has an "into table" statement that creates a new table
            //B:  The "into table" statement has it's own line
            //C:  The query has "no drop table if exists" header
            //D:  The query begins with "Select"
            //E:  The query contains only one "Select" string

            //1.  Create table
            //A.    Modify the query by adding "select top 0 "
            createTableQuery = query.Replace("select"
                , "select top 0 ");


            //2.  Create extract query
            var lines = query
                .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            string tableName = string.Empty;
            int removeLineNum = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("into"))
                {
                    var parts = lines[i].Split();
                    if (parts.Length != 2)
                        throw new Exception("The query line with into should have 2 parts.");
                    tableName = parts[1];
                    removeLineNum = i;
                }
            }
            lines.RemoveAt(removeLineNum);
            lines.Insert(0, "insert into " + tableName);
            runQuery = String.Join("\r\n", lines);
        }
        private static readonly int maxDaysForSingleQuery = 14;

        private static void Extract_Yale_FileImport(Translate translate)
        {
            var columnNames = new List<string> { "loinc_code", "LoincScale", "ord_value", "result_flag_c"
                , "EXTERNAL_NAME" };
            var columnTypes = new List<string> { "varchar(254)", "varchar(254)", "varchar(254)", "varchar(254)"
            , "varchar(75)"};

            DataTable dt = new DataTable();
            dt.Columns.Add(columnNames[0], typeof(string));
            dt.Columns.Add(columnNames[1], typeof(string));
            dt.Columns.Add(columnNames[2], typeof(string));
            dt.Columns.Add(columnNames[3], typeof(string));  //float
            dt.Columns.Add(columnNames[4], typeof(string));

            //error check to make sure dt is in sync with required columns.
            if (dt.Columns.Count != Enum.GetNames(typeof(Column)).Length)
                throw new Exception("Make sure the number of columns in the input file matches MapInput.Resources.ColumnMap");

            FileIO.ReadDataTableFromFile(ref dt, translate.extract.ImportFilePath);
            SqlTable.CreateTable(translate.ConString, translate.Database, translate.Get(Tables.Extract)
                , columnNames, columnTypes);
            SqlTable.BulkInsertDataTable(translate.ConString, translate.Get(Tables.Extract), dt);
        }

        //Deprecated:
        //private static void Extract_Yale_LabList(Translate translate)
        //{
        //    LabResultMap.LabList.LabList.LoadLabListFromFile(translate); //may derive from data itself   

        //    //Note: To implement a query for each day (not all at once) see Extract_Yale_Standard

        //    string query = String.Format(Resource.Resource.YaleLabQuery_LabList, translate.Get(Tables.Extract), translate.Get(Tables.LabList)
        //        , translate.extract.StartDate, translate.extract.EndDate);
        //    SqlTable.ExecuteNonQuery(translate.ConString, query);
        //}

    }
}
