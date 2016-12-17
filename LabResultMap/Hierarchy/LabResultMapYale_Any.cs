using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

/*
 * When a lab does not map to it's primary group. It's sent here,
 * to attempt to map to any group.  Later a function will find 
 * those labs mapped by multiple functions and label them for 
 * quality improvement.
 * 
 */ 
namespace LabResultMap
{
    class LabResultMapYale_Any : LabResultMapYale
    {
        private static List<string> classList;
        internal LabResultMapYale_Any() : base(){ }

        internal override void MapRow(DataRow input)
        {
            SetClassList(input["MapFunc"].ToString());
            if (classList != null && classList.Count != 0)  //Empty class lists = no degenerate match. 
            {
                int startIdx = 0;
                MapRowAny(input, startIdx);   
            } 

            if( input["MappedYN"].ToString() == "N" )
                UnableToMap(input);
        }

        
        private void MapRowAny(DataRow input, int anyClassListIdx)  //recursive
        {            
            //attempt to map        
    
            string className = classList[anyClassListIdx];

            if (className == "MapRow_General")  //cannot instantiate base class
                MapRow_General(input);
            else
            {
                LabResultMapYale labMapYale = LabMapYaleAnyFactory(className);
                labMapYale.MapRow(input);
            }

            //evaluate map
            if (input["MappedYN"].ToString() == "N")  //did not map
            {
                anyClassListIdx++;
                if (anyClassListIdx == classList.Count)  //no more maps to try
                    return;   
                else
                    MapRowAny(input, anyClassListIdx);
            }
            return; //mapped or returning from recursion
        }
        private void SetClassList(string mapClass)
        {
            switch (mapClass)
            {
                case "LabResultMap.LabResultMapYaleQn":
                    classList = new List<string>(){"LabResultMapYaleQn_Range", "LabResultMapYaleOrd", "LabResultMapYaleQn_Titer"
                        , "LabResultMapYaleQn_Calc", "LabResultMapYaleQn_Million", "MapRow_General", "LabResultMapYaleNom"
                        , "LabResultMapYaleQn_RemoveUnitsEnd", "LabResultMapYale_LowPriority"};
                    break;
                case "LabResultMap.LabResultMapYaleNom":
                    classList = new List<string>() { "LabResultMapYaleOrd", "MapRow_General", "LabResultMapYaleQn"
                        , "LabResultMapYale_LowPriority"};
                    break;
                case "LabResultMap.LabResultMapYaleOrd":
                    classList = new List<string>() { "LabResultMapYaleNom", "MapRow_General"
                        , "LabResultMapYaleQn", "LabResultMapYale_LowPriority" }; 
                    break;
                case "LabResultMap.LabResultMapYaleQn_Range":
                    classList = new List<string>(){"LabResultMapYaleQn", "MapRow_General", "LabResultMapYaleOrd"
                        , "LabResultMapYaleNom", "LabResultMapYale_LowPriority"}; 
                    break;
                case "LabResultMap.LabResultMapYaleQn_Titer":
                    classList = new List<string>(){"LabResultMapYaleQn", "MapRow_General", "LabResultMapYaleOrd"
                        , "LabResultMapYaleNom"}; 
                    break;
                case "LabResultMap.LabResultMapYaleQn_Million":
                case "":
                case "LabResultMap.LabResultMapYale_Any":
                    classList = new List<string>(){"LabResultMapYaleQn", "LabResultMapYaleQn_Range", "LabResultMapYaleOrd"
                        , "LabResultMapYaleQn_Titer", "MapRow_General", "LabResultMapYaleNom", "LabResultMapYaleQn_RemoveUnitsEnd"
                        , "LabResultMapYaleQn_Million", "LabResultMapYale_LowPriority" };
                    break;
                case "LabResultMap.LabResultMapYaleHepCGenotype":
                    classList = new List<string>() { "MapRow_General", "LabResultMapYaleOrd" };
                    break;
                case "LabResultMap.None":
                    classList = new List<string>(){};  //do not attempt to match with another class
                    break;
                case "LabResultMap.LabResultMapYaleReviewSourceData":
                    break;
                default:
                    throw new Exception("Add this class here: " + mapClass);
            }
        }
        private LabResultMapYale LabMapYaleAnyFactory(string className)
        {
            switch (className)
            {
                case "LabResultMapYaleQn":
                    return new LabResultMapYaleQn();
                case "LabResultMapYaleOrd":
                    return new LabResultMapYaleOrd();
                case "LabResultMapYaleNom":
                    return new LabResultMapYaleNom();
                case "LabResultMapYaleQn_Range":
                    return new LabResultMapYaleQn_Range();
                case "LabResultMapYaleQn_Titer":
                    return new LabResultMapYaleQn_Titer();
                case "LabResultMapYaleQn_Calc":
                    return new LabResultMapYaleQn_Calc();
                case "LabResultMapYaleQn_Million":
                    return new LabResultMapYaleQn_Million();
                case "LabResultMapYale_LowPriority":
                    return new LabResultMapYale_LowPriority();
                case "LabResultMapYaleQn_RemoveUnitsEnd":
                    return new LabResultMapYaleQn_RemoveUnitsEnd();
                default:
                    throw new Exception("Update this switch for the new class");
            }
        }

    }
}
