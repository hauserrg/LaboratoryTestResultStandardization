using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabResultMap
{
    class LabResultMapYaleQn_RemoveUnitsEnd : LabResultMapYale
    {
        internal LabResultMapYaleQn_RemoveUnitsEnd() : base() { }

        internal override void MapRow(DataRow input) 
        {
            string valueOrig = input[Column.Result.ToString()].ToString().Trim();
            
            string valueReturn = valueOrig;
            string units;
            if( !Helper.Helper.RemoveUnitsFromEnd(ref valueReturn, out units) )
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;        
            }

            string question, inequality, percent = String.Empty;
            valueReturn.TrimStart();
            LabResultMapYaleQn.QnPrefix(ref valueReturn, out question, out inequality);
            valueReturn = valueReturn.TrimEnd();
            LabResultMapYaleQn.QnSuffix(ref valueReturn, out percent);
            valueReturn = valueReturn.Trim();

            //2.  Numeric?
            decimal d = 0;
            if ( !decimal.TryParse(valueReturn, out d))
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            //3. After decimal
            string afterDecimal = LabResultMapYaleQn.AfterDecimal(d);
            input["AfterDecimal"] = Int32.Parse(afterDecimal);

            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Number"] = d.ToString();
            input["Field1"] = "Unit: " + units;
            input["Pretty"] = d;

            //inequality
            if (inequality != String.Empty)
                input["Inequality"] = inequality;

            return;
        }
    }
}
