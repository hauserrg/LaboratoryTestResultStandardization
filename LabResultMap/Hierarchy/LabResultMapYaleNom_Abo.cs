using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap
{
    //Loinc:  883-9  ABO (compare to 882-1  ABO and Rh)
    class LabResultMapYaleNom_Abo : LabResultMapYale
    {
        internal LabResultMapYaleNom_Abo()
            : base() { }

        internal override void MapRow(System.Data.DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();

            //2.  check ABO and Rh
            Abo abo;
            bool foundABO = LabResultMapYaleNom_AboRh.FindAbo(value, out abo);
            if( !foundABO )
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.None";
                return;
            }
            else
            {
                string output = abo.ToString();
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Field1"] = output;
                input["Pretty"] = output;
                
                return;
            }
        }
    }
}
