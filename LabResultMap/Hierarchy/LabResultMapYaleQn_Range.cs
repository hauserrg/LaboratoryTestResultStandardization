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

    class LabResultMapYaleQn_Range : LabResultMapYaleQn
    {
        internal LabResultMapYaleQn_Range() : base() { }

        internal override void MapRow(DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();

            //1.  Look for inequality, remove if found.
            string testValue = value.TrimStart();
            string inequality = String.Empty;
            testValue = Inequality(testValue, ref inequality);
            testValue = testValue.Trim();

            //2.  Find "-"
            string[] colonSides = testValue.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
            if (colonSides.Length != 2 || testValue.EndsWith("-"))  //more or less than a single colon.  No "5-1-"
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            //3.  assemble range
            string range = String.Empty;
            decimal left, right;
            if( decimal.TryParse(colonSides[0], out left)
                && decimal.TryParse(colonSides[1], out right))
            {
                range = colonSides[0] + "-" + colonSides[1];
            }

            //4.  Save
            if (range == String.Empty )
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            input["MappedYN"] = "Y";
            input["Field1"] = range;
            input["Inequality"] = inequality;
            input["MapFunc"] = this.ToString();
            return;
        }
    }
}
