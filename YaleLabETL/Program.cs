//using LabEtlTranslate;
using log4net;
using System;
using System.IO;
using LabEtlTranslate;
using System.Collections.Generic;
using System.Data;
using SharedLibrary;
using LabEtlTranslate.XmlInputs;
using MapResult.Resource;
using MapResult;

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

namespace Etl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Option 0: LabDataValidator | VACS (not sure which)
            //var inputTable = "VHACONHAUSER.GroupData"; //"X4.VA_Dim_ResultDistinct_ForMap"; "VA_Dim_ResultDistinct"; 
            //var inputDataQuery = String.Format("select Yoinc Loinc, LabChemResultValue Result, Count LoincScale from {0}", inputTable); //Loinc, Result, LoincScale
            //var outputTable = "VHACONHAUSER.CleanData"; //"X4.M0_Dim_ResultMap"; "VA_Dim_ResultMap";

            //Option 1: Loinc independent map
            //Note: This will not be able to handle results like "1" very well because they could be multiple things - HCV genotype, numeric, ordinal
            //MapResult.MapResult.MapWithoutLoinc(inputTable, outputTable);

            //Option 2: Loinc dependent map
            //2a.  Constants, dependency injection
            TranslateXml translateXml = SerializeHelper.Deserialize<TranslateXml>(MapResult.Settings.Resource.TranslateXml);
            var translate = Translate.TranslateFromXml(translateXml);
            translate.ConString = ConMgr.Get(Db.VaNationalMyDb);

            //2b.  Main
            var outputTable = "Pcs_LabMed.X4.Map";
            var inputDataQuery = String.Format("select Loinc, Result, LoincScale from Pcs_LabMed.X4.DistinctGroupData");
            var distinct = SqlTable.GetTable(inputDataQuery, translate.ConString); //Get data
            DataTable mapDistinct = LabResultMap.LabResultMap.GetMap(distinct); //Map data
            SqlTable.ExecuteNonQuery(translate.ConString, String.Format(Resource.CreateOutputTable, outputTable));  //Create output table
            SqlTable.AddIdCol(mapDistinct);
            SqlTable.BulkInsertDataTable(translate.ConString, outputTable, mapDistinct); //Load output table
            //SqlTable.BCP_GetTableToTxtFile(@"reds1t\reds", "select * from [dbo].[VA_Dim_ResultMap]", CommonPath.DesktopTempFile()); //Export
            
            //Option 3: Other
            //args = new string[1];
            //args[0] = @".\Settings\TranslateXml.xml";

            //3a.  xml path to "Translate" object
            //Translate translate = InputFuncs.CreateInput(InputFrom.Args, args);
            //translate.log.Debug("Translate loaded.");

            //3b.  Extract and map 
            //if (translate.extract.RunExtract == RunExtract.On)
            //    LabExtract.Extract(translate);
            //if (translate.RunMap == RunMap.On)
            //    EtlMap.Map(translate);
            //translate.log.Debug("ETL complete.");

            //3c.  Reports
            //if (translate.RunReport == RunReport.On || translate.RunReportTables == LabEtlTranslate.RunReportTables.On)
            //{
            //    Report2.DataQuality.Run(translate);  //title page, other reports awaiting refactoring
            //    EachLoinc.Write(translate);           //individual lab reports, add ordinal type graphs
            //    EachLoinc.MergeReports(translate);  //join pdf with Report2.DataQuality.Run()

            //    //Report2.Variance.Variance.Run(translate);
            //    //Report2.PosPct.PosPct.WritePosPctToFile(translate);
            //    //Report2.LoincReview.Run(translate);
            //}
        }
    }
}