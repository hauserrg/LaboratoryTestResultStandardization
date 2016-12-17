//using LabEtlTranslate;
using log4net;
using System;
using System.IO;
using LabEtlTranslate;
using System.Collections.Generic;
using System.Data;
using SharedLibrary;
using LabEtlTranslate.XmlInputs;

/*
High Priority:
To match unmatched Loinc code see the query in Other->DP

Lower Priority:
Yale also has multiline results (What length should multiline results have (varchar(100)?)

Out of scope:
Involve ExtractWriter, currently loading ETL_LabList with original program
Setup the program to work with creating a data mart (ie, list of encounters).

Not going to do:
Remove Loinc_scale as a requried column <- why fix if not broken?

Added to Yale when finished:
--ALTER TABLE [Proj_OneYear_ETL_1Extract] ALTER COLUMN order_proc_id numeric(18,0) NOT NULL
--ALTER TABLE [Proj_OneYear_ETL_1Extract] ALTER COLUMN component_id numeric(18,0) NOT NULL
--alter table [Proj_OneYear_ETL_1Extract] add primary key(order_proc_id, component_id) --clustered index
--create nonclustered index IX_ORDER_INST on Proj_OneYear_ETL_1Extract (Order_inst)
 
select distinct PAT_ENC_CSN_ID
into Proj_OneYear_DistinctEncList
from [dbo].[Proj_OneYear_ETL_3Load]
  
select x.PAT_ENC_CSN_ID, y.PAT_ID
into Proj_OneYear_EncSubjList
from (select distinct PAT_ENC_CSN_ID from [Proj_OneYear_ETL_3Load]) x 
left join [EPICCLARSQLP].[CLARITY].[DBO].pat_enc y on y.pat_enc_csn_id = x.PAT_ENC_CSN_ID
	--left join to keep all encounters, even those without a subjectId

select distinct PAT_ID
into Proj_OneYear_DistinctSubjList
from Proj_OneYear_EncSubjList 
*/

namespace MapResult
{
    public static class MapResult
    {
        /// <summary>
        /// Main map function for VA.  (Updated 15-05-18)
        /// </summary>
        public static void Map(string inputTable, string outputTable)
        {
            //Constants, dependency injection
            TranslateXml translateXml = SerializeHelper.Deserialize<TranslateXml>(Settings.Resource.TranslateXml);
            var translate = Translate.TranslateFromXml(translateXml);

            //VA-only
            //var inputTable = "X2.VA_Dim_ResultDistinct";
            //var outputTable = "X2.M0_Dim_ResultMap";

            //Main
            var distinct = SqlTable.GetTable("select * from " + inputTable, translate.ConString); //Get data
            DataTable mapDistinct = LabResultMap.LabResultMap.GetMap(distinct); //Map data
            SqlTable.ExecuteNonQuery(translate.ConString, String.Format(Resource.Resource.CreateOutputTable,outputTable));  //Create output table
            SqlTable.AddIdCol(mapDistinct);
            SqlTable.BulkInsertDataTable(translate.ConString, outputTable, mapDistinct); //Load output table
        }

        /// <summary>
        /// Map function without Loinc for VA.  Used to auto-map the Loinc code.  (Updated 15-10-09)
        /// </summary>
        public static void MapWithoutLoinc(string inputTable, string outputTable)
        {
            //Constants, dependency injection
            TranslateXml translateXml = SerializeHelper.Deserialize<TranslateXml>(Settings.Resource.TranslateXml);
            var translate = Translate.TranslateFromXml(translateXml);

            //VA-only
            //var inputTable = "X2.VA_Dim_ResultDistinct";
            //var outputTable = "X2.M0_Dim_ResultMap";

            //Main
            var distinct = SqlTable.GetTable("select Yoinc Loinc, LabChemResultValue Result, 'X' LoincScale from " + inputTable, translate.ConString); //Get data
            DataTable mapDistinct = LabResultMap.LabResultMap.GetMap(distinct); //Map data

            //mapDistinct.Columns.Remove("Loinc");
            //mapDistinct.Columns.Remove("LoincScale");
            //mapDistinct.Columns.Remove("Abnormal");
            //mapDistinct.Columns.Remove("LabName");
            mapDistinct.Columns["Result"].SetOrdinal(1);
            
            //var createOutputQuery = String.Format(Resource.Resource.CreateOutputTable_MapWithoutLoinc, outputTable);
            var createOutputQuery = String.Format(Resource.Resource.CreateOutputTable, outputTable);
            SqlTable.ExecuteNonQuery(translate.ConString, createOutputQuery);  //Create output table
            
            SqlTable.AddIdCol(mapDistinct);
            SqlTable.BulkInsertDataTable(translate.ConString, outputTable, mapDistinct); //Load output table
        }
    }
}