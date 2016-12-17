using LabEtlTranslate;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapResult
{
    /********************  Veterans Affairs **********************************/
    internal static partial class LabExtract
    {
        private static void RunVa(Translate translate)
        {
            ExtractVa(translate);
            MapLoincTypeVa(translate);  //ToDo:  Lookup loinc type from this project, not VA database
        }


        /********  Veterans Affairs Functions  *********/
        private static void ExtractVa(Translate translate)
        {
            SqlTable.ExecuteNonQuery(translate.ConString
                , "select * into Dflt.Proj_" + translate.ProjectName + "_ETL_LabList from ETL_LabList");

            string query = String.Format(Resource.Resource.VaLabQuery, translate.Get(Tables.Extract));
            SqlTable.ExecuteNonQuery(translate.ConString, query);
        }

        private static void MapLoincTypeVa(Translate translate)
        {
            //VA only - map loinc scale type
            SqlTable.ExecuteNonQuery(translate.ConString, String.Format(Resource.Resource.MapLoincScaleType, translate.Get(Tables.Extract)));
        }
    }
}
