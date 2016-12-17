using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap
{
    //Loinc:  21299-3  Gestational age method
    class LabResultMapYaleNom_Ga : LabResultMapYale
    {
        internal LabResultMapYaleNom_Ga()
            : base() { }

        private enum Ga { Ultrasound, HistoryPhysical, LMP };
        internal override void MapRow(System.Data.DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();

            //2.  
            bool foundGa = FindGa(value);
            if( !foundGa )
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }
            else
            {
                string output = value.ToUpper();
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Field1"] = output;
                input["Pretty"] = output;
                
                return;
            }
        }

        private bool FindGa(string gestationalAge)
        {
            Ga a;
            if (Enum.TryParse(gestationalAge, out a))
                return true;
            else
                return false;
        }
    }
}
