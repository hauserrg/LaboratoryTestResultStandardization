using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
/*
 *  This class just provides a wrapper for the underlying map protocols.
 *  
 *  The user can choose from the LabMapTypes and that type is implemented for mapping.
 * 
 *  To implement a new mapping, just add it to the LabMapType and LabMapFactory
 * */


namespace LabResultMap
{
    public abstract class LabResultMap //BASE CLASS
    {
        internal enum Column { Loinc, LoincScale, Result, Abnormal, LabName, LabId };

        //mapping
        /// <summary>
        /// A distinct list of {columns,value,mapValue}
        /// </summary>
        /// <param name="columns">The names of the columns involved in the mapping</param>
        /// <param name="mapFcn">A function to produce the mapValue given the columns</param>
        public static DataTable GetMap(DataTable distinct)
        {
            //Example call:
            //DataTable distinct = new DataTable();
            //distinct.Columns.Add("loinc_code", typeof(string));
            //distinct.Columns.Add("ord_value", typeof(string));
            //distinct.Columns.Add("LoincScale", typeof(string));
            //string path = @"C:\Users\rgh28\Desktop\DistinctMapSimple.txt";
            //SharedLibrary.FileIO.ReadDataTableFromFile(ref distinct, path, header: true);

            LabResultMap labMap = LabResultMap.Factory();
            labMap.MapTable(distinct);
            return distinct;
        }
        private static LabResultMap Factory()
        {
            return new LabResultMapYale();
        }
        private void MapTable(DataTable distinct)
        {
            //1.  Add map columns 
            distinct.Columns.Add(new DataColumn("MappedYN", typeof(string)));
            distinct.Columns.Add(new DataColumn("MapFunc", typeof(string)));
            distinct.Columns.Add(new DataColumn("Inequality", typeof(string)));
            distinct.Columns.Add(new DataColumn("Number", typeof(string)));
            distinct.Columns.Add(new DataColumn("AfterDecimal", typeof(int))); //string
            distinct.Columns.Add(new DataColumn("Field1", typeof(string)));
            distinct.Columns.Add(new DataColumn("Field2", typeof(string)));
            distinct.Columns.Add(new DataColumn("General", typeof(string)));
            distinct.Columns.Add(new DataColumn("Pretty", typeof(string)));  //result used for report
            //Note:  if you add more columns here, also add them to the CreateMapTable func.

            //2.  Perform mapping
            for (int i = 0; i < distinct.Rows.Count; i++)
                MapRow(distinct.Rows[i]);
        }
        internal abstract void MapRow(DataRow input); //abstract requires override

        //mapping general results
        private static Dictionary<string, string> generalResults;
        private Dictionary<string, string> LabMapYaleFactory_GeneralResults()
        {
            string dictString = Constants.Resource.GeneralResults;
            return SharedLibrary.LoadDictionary.Resource(dictString, true);
        }
        protected LabResultMap() //constructor 
        {
            if( generalResults == null || generalResults.Count == 0)
                generalResults = LabMapYaleFactory_GeneralResults();
        }
        protected void MapRow_General(DataRow input)  //Cancelled, comments, TNP, ect.
        {
            string keyQ = input[Column.Result.ToString()].ToString().Trim();

            //1.  Try to match general results 
            if (generalResults.ContainsKey(keyQ) )
            {
                input["General"] = generalResults[keyQ];
                input["MappedYN"] = "Y";
                input["MapFunc"] = "LabMap.MapRow_General";
                input["Pretty"] = input["General"];
            }
            else if (keyQ == String.Empty || input[Column.Result.ToString()] == DBNull.Value) 
            {
                input["General"] = "Non-standard Result";
                input["MappedYN"] = "Y";
                input["MapFunc"] = "LabMap.MapRow_General";
            }
            else  
            {
                input["MappedYN"] = "N";
                //input["MapFunc"] = this.ToString();  //Debug only - make sure each has tried MapRow_General()            
            }
        }

        //cannot map
        protected static void UnableToMap(DataRow input)
        {
            input["MappedYN"] = "N";
            input["MapFunc"] = DBNull.Value;
            input["Pretty"] = "Unable to map";
        }
    } 
}
