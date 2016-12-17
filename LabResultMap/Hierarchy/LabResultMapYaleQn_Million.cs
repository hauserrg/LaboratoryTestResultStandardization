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

    class LabResultMapYaleQn_Million : LabResultMapYaleQn
    {
        internal static Dictionary<string, string> millionPhrases;
        internal LabResultMapYaleQn_Million() : base()
        {
            if (millionPhrases == null || millionPhrases.Count == 0)
                millionPhrases = SharedLibrary.LoadDictionary.Resource(Constants.Resource.LabResultMapYaleQn_Million,true);
        }

        internal override void MapRow(DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();

            //1.  Look for inequality, remove if found.
            string testValue = value.TrimStart();
            string inequality = String.Empty;
            testValue = Inequality(testValue, ref inequality);
            testValue = testValue.Trim();

            //2.
            //Take the string and separate it into two parts.
            //The first part contains only numbers and '.' at the beginning
            //The second part contains anything after end of the first part
            //If the first part is a double, and...
            //  the second part matches only the "millionPhrases"
            //Then this is ok; otherwise not mapped
            string[] parts = new string[2];  //2 parts: a number and a phrase
            if (!GetParts(testValue, parts))
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            if (! millionPhrases.ContainsKey(parts[1]))  //This is the word part
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            decimal d = 0;
            bool numericQ = decimal.TryParse(parts[0], out d);
            if (!numericQ)
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            //After decimal
            string numeric = (d * 1000000).ToString("0.############################");
            string afterDecimal = LabResultMapYaleQn.AfterDecimal(decimal.Parse(numeric));
            input["AfterDecimal"] = Int32.Parse(afterDecimal);


            input["MapFunc"] = this.ToString();
            //multiply by a million, remove added trailing zeros
            input["Number"] = numeric; 
            input["MappedYN"] = "Y";
            input["Inequality"] = inequality;
            input["Pretty"] = input["Number"].ToString();

            return;
        }

        private bool GetParts(string testValue, string[] parts)
        {
            //1.  Find the point without a digit or period
            int idx = 0;
            foreach (var letter in testValue)
            {
                if (Char.IsDigit(letter) || letter == '.')
                    idx++;
                else break;
            }

            //0: first char is not a digit or . -> not what we're looking for...
            //Length:  last char is a digit or . -> not what we're looking for...
            if (idx == 0 || idx == testValue.Length)
                return false;
            //else: first non-digit or .
            parts[0] = testValue.Substring(0, idx); //only digit or .
            parts[1] = testValue.Substring(idx).Trim();
            return true;
        }
    }
}
