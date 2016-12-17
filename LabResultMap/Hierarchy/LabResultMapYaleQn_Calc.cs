using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabResultMap
{
    //Find range results:
    //select distinct Loinc, 'LabMapYaleQn_Range' 
    //from [dbo].[Proj_ADay_ETL_2Transform] 
    //where MappedYN = 'N' and LoincScale = 'Qn' and Result like '%-%' 
    //order by Loinc

    //Note:  Merge this routine with "_RemoveUnitsEnd.cs"
    class LabResultMapYaleQn_Calc : LabResultMapYaleQn
    {
        internal LabResultMapYaleQn_Calc() : base() { }

        internal override void MapRow(DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();

            //1.  Look for inequality, remove if found.
            string testValue = value.TrimStart();
            string inequality = String.Empty;
            testValue = Inequality(testValue, ref inequality);
            testValue = testValue.Trim();

            //2.  Find "c" or "calc"
            decimal d = 0;
            if (testValue.EndsWith("c",StringComparison.InvariantCultureIgnoreCase))  
            {
                if (testValue.EndsWith("calc", StringComparison.InvariantCultureIgnoreCase))
                    testValue = StringUtil.RemoveFromEnd(testValue, "calc");
                else if (testValue.EndsWith("c", StringComparison.InvariantCultureIgnoreCase))
                    testValue = StringUtil.RemoveFromEnd(testValue, "c");

                bool numericQ = decimal.TryParse(testValue, out d);
                if (!numericQ)
                {
                    input["MappedYN"] = "N";
                    input["MapFunc"] = this.ToString();
                    return;
                }
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            string afterDecimal = LabResultMapYaleQn.AfterDecimal(decimal.Parse(testValue));
            input["AfterDecimal"] = Int32.Parse(afterDecimal);
            input["Number"] = testValue;
            input["MappedYN"] = "Y";
            input["Field1"] = "calc";
            input["Inequality"] = inequality;
            input["MapFunc"] = this.ToString();
            input["Pretty"] = testValue;
            return;
        }
    }
}
