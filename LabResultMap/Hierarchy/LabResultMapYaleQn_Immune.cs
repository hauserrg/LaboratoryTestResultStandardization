using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LabResultMap
{
    /// <summary>
    /// Used to map values like this "6.95 IMMUNE", "5.90  (Immune)"
    /// </summary>
    class LabResultMapYaleQn_Immune : LabResultMapYale
    {
        internal LabResultMapYaleQn_Immune() : base() { }

        internal override void MapRow(System.Data.DataRow input)
        {
            // Match the regular expression pattern against a text string.
            string pattern = @"\s*(IMMUNE|\(Immune\)|\(Immune|Immume|Immunr|Immmune|Immune\))\s*$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            string result = input[Column.Result.ToString()].ToString();
            Match m = regex.Match(result);
            if (m.Groups.Count == 1)
            {
                //This map works for rare cases, but it's not part of the "*_Any".
                var firstOrLast = new LabResultMapYaleOrd_FirstOrLast();
                firstOrLast.MapRow(input);

                if (input["MappedYN"].ToString() != "Y")
                {
                    input["MappedYN"] = "N";
                    input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
                }
                return;
            }
            string resultNew = regex.Replace(result, "");
            
            //See if it fits the pattern.
            decimal num;
            if( Decimal.TryParse(resultNew, out num) )
            {
                string afterDecimal = LabResultMapYaleQn.AfterDecimal(num);
                input["AfterDecimal"] = Int32.Parse(afterDecimal);
                input["Number"] = num.ToString();
                input["Field1"] = "Immune";
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Pretty"] = num.ToString();
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
            }
        }
    }
}
