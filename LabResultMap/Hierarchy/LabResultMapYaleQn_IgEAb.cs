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
    class LabResultMapYaleQn_IgEAb : LabResultMapYale
    {
        internal LabResultMapYaleQn_IgEAb() : base() { }

        internal override void MapRow(System.Data.DataRow input)
        {
            if (NumAsm(input)) return;
            else if (EndInequality(input)) return;
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
            }
        }

        /// <summary>
        /// Matches something like this "2.71>"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static bool EndInequality(System.Data.DataRow input)
        {
            string pattern = @"(?<Num>0?.?\d+)(?<Inequality><|>|<=|>=)$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            Match m = regex.Match(input[Column.Result.ToString()].ToString());
            if (m.Groups["Num"].ToString() != String.Empty)
            {
                decimal num;
                if (Decimal.TryParse(m.Groups["Num"].ToString(), out num))
                {
                    string afterDecimal = LabResultMapYaleQn.AfterDecimal(num);
                    input["AfterDecimal"] = Int32.Parse(afterDecimal);
                    input["Number"] = num.ToString();
                    input["MappedYN"] = "Y";
                    input["MapFunc"] = "LabResultMap.LabResultMapYaleQn_IgEAb";
                    input["Pretty"] = num.ToString();

                    //">1" is different than "1<"...flip the inequality
                    if (m.Groups["Inequality"].ToString() != String.Empty)
                    {
                        if (m.Groups["Inequality"].ToString() == "<")
                            input["Inequality"] = ">";
                        else if (m.Groups["Inequality"].ToString() == "<=")
                            input["Inequality"] = ">=";
                        else if (m.Groups["Inequality"].ToString() == ">")
                            input["Inequality"] = "<";
                        else if (m.Groups["Inequality"].ToString() == ">=")
                            input["Inequality"] = "<=";
                        else
                            throw new Exception("Unrecognized inequality.");
                    }
                        

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Matches something like this... ".35KU/L, %ASM:12"
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool NumAsm(System.Data.DataRow input)
        {
            //1.  remove spaces
            string value = input[Column.Result.ToString()].ToString();
            string result = value.Replace(" ", String.Empty).Replace("\"", "");

            //2.  attempt to match
            string pattern = @"^(?<Inequality>>=|<=|<|>)?(?<Num>\d*\.?\d*)(:|;)?(KU/L)?(,|;|:)?\(?%?ASM:?(?<Asm>\d*)\)?";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            Match m = regex.Match(result);
            if (m.Groups["Num"].ToString() != String.Empty)
            {
                decimal num;
                if (Decimal.TryParse(m.Groups["Num"].ToString(), out num))
                {
                    string afterDecimal = LabResultMapYaleQn.AfterDecimal(num);
                    input["AfterDecimal"] = Int32.Parse(afterDecimal);
                    input["Number"] = num.ToString();
                    input["MappedYN"] = "Y";
                    input["MapFunc"] = this.ToString();
                    input["Pretty"] = num.ToString();

                    if (m.Groups["Asm"].ToString() != String.Empty)
                    {
                        decimal num2;
                        if (Decimal.TryParse(m.Groups["Asm"].ToString(), out num2))
                            input["Field1"] = num2.ToString();
                    }
                    if (m.Groups["Inequality"].ToString() != String.Empty)
                        input["Inequality"] = m.Groups["Inequality"].ToString();
                    
                    return true;
                }
            }
            return false;
        }
    }
}
