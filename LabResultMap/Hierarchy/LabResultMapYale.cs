using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;


namespace LabResultMap
{
    internal partial class LabResultMapYale : LabResultMap
    {
        private static Dictionary<string, string> loincClasses;  
        private static Dictionary<string, string> scaleClasses;

        //constructor
        internal LabResultMapYale()
        {
            //initialize data needed for the class here.
            if( loincClasses == null || loincClasses.Count == 0)
                loincClasses = LabMapYaleFactory_LoincClasses();
            if (scaleClasses == null || scaleClasses.Count == 0)
                scaleClasses = LabMapYaleFactory_ScaleClasses();
        }

        //mapping
        internal override void MapRow(DataRow input)
        {
            //Check if null... 
            if (input[Column.Result.ToString()] == DBNull.Value)
            {
                MapRow_General(input);
                return;
            }

            //Normalize the result
            var origValue = input[Column.Result.ToString()].ToString();
            input[Column.Result.ToString()] = input[Column.Result.ToString()]
                .ToString().Replace("\"", "");

            //Choose the map class
            string loinc = input[Column.Loinc.ToString()].ToString();
            string scale = input[Column.LoincScale.ToString()].ToString();
            LabResultMapYale labMapYale = LabMapYaleFactory(loinc, scale);

            if (labMapYale == null)  //no class available
                throw new Exception("You must select a mapping method.");
            else
            {
                labMapYale.MapRow(input);

                //try other mapping strategies
                if (input["MappedYN"].ToString() == "N")
                {
                    var any = new LabResultMapYale_Any();
                    any.MapRow(input);
                }

                input[Column.Result.ToString()] = origValue;
                if( input["Number"] != DBNull.Value)
                    input["Number"] = input["Number"].ToString().Replace(",","");
                return;
            }
        }

        //other
        #region Factory Funcs
        private LabResultMapYale LabMapYaleFactory(string loinc, string scale)
        {
            //1.  Try to match loinc first
            if( loincClasses.ContainsKey(loinc) )
                return LabMapYaleFactory_Loinc(loincClasses[loinc]);
                

            //2.  Try to match scale second
            if (scaleClasses.ContainsKey(scale))
                return LabMapYaleFactory_Scale(scaleClasses[scale]);
            
            //3.  No match
            else
                return new LabResultMapYale_Any();
        }
        private LabResultMapYale LabMapYaleFactory_Scale(string scaleClass)
        {
            switch (scaleClass)
            {
                case "LabMapYaleQn":
                    return new LabResultMapYaleQn();
                case "LabMapYaleOrd":
                    return new LabResultMapYaleOrd();
                case "LabMapYaleNom":
                    return new LabResultMapYaleNom();
                default:
                    throw new Exception("Scale class recognized in dictionary, but not in function.");
            }   
        }
        private LabResultMapYale LabMapYaleFactory_Loinc(string loincClass)
        {
            switch (loincClass)
            {
                case "LabMapYaleQn_Titer":
                    return new LabResultMapYaleQn_Titer();
                case "LabMapYaleNom_AboRh":
                    return new LabResultMapYaleNom_AboRh();   
                case "LabMapYaleNom_Abo":
                    return new LabResultMapYaleNom_Abo();
                case "LabMapYaleNom_Ga":
                    return new LabResultMapYaleNom_Ga();
                case "LabResultMapYaleOrd_FirstOrLast":
                    return new LabResultMapYaleOrd_FirstOrLast();
                case "LabResultMapYaleQn_Log10":
                    return new LabResultMapYaleQn_Log10();
                case "LabResultMapYaleQn_ViralLoad":
                    return new LabResultMapYaleQn_ViralLoad();
                case "LabResultMapYaleHepCGenotype":
                    return new LabResultMapYaleHepCGenotype();
                case "LabResultMapYaleQn_Immune":
                    return new LabResultMapYaleQn_Immune();
                case "LabResultMapYaleQn_IgEAb":
                    return new LabResultMapYaleQn_IgEAb();
                case "LabResultMapYaleReviewSourceData":
                    return new LabResultMapYaleReviewSourceData();
                case "LabResultMapYaleNotALab":
                    return new LabResultMapYaleNotALab();
                default:
                    return null;
            }   
        }
        #endregion 
        #region Dictionary Funcs
        private static Dictionary<string,string> LabMapYaleFactory_LoincClasses()
        {
            string loincClass = Constants.Resource.LabMapYale_Loinc;
            return SharedLibrary.LoadDictionary.Resource(loincClass, false);            
        }
        private static Dictionary<string,string> LabMapYaleFactory_ScaleClasses()
        {
            string scaleClass = Constants.Resource.LabMapYale_Scale;
            return SharedLibrary.LoadDictionary.Resource(scaleClass, false);
        }
        #endregion
    }
}
