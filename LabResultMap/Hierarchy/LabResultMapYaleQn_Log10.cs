using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LabResultMap
{
    /// <summary>
    /// Used to map values like this "27753 (4.4 LOG10)"
    /// </summary>
    class LabResultMapYaleQn_Log10 : LabResultMapYale
    {
        internal LabResultMapYaleQn_Log10() : base() {}

        internal override void MapRow(System.Data.DataRow input)
        {
            // Match the regular expression pattern against a text string.
            //string pattern = @"([0-9.]*)\s*\(([0-9.]*)\s*LOG\s*10\)";
            //Note:  Groups=  Inequality, NotLog, Inequality2, Log
            string pattern = @"^(?<Inequality><=|>=|<|>)?(?<NotLog>[0-9]*\.?,?[0-9]*)\s*\(?(?<Inequality2><=|>=|<|>)?(?<Log>[0-9]*\.?[0-9]*)\s*LG?OG\s*10\)?";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

            string result = input[Column.Result.ToString()].ToString();
            Match m = regex.Match(result);
            
            //See if it fits the pattern.
            decimal num, numLog;
            bool isNum, isNumLog;
            isNum = Decimal.TryParse(m.Groups["NotLog"].Value, out num);
            isNumLog = Decimal.TryParse(m.Groups["Log"].Value, out numLog);

            //Options:
            //isNum Ok, isNumLog Ok -> both number and log
            //isNum Ok, isNumLog X  -> only a log
            if (isNum && isNumLog)
            {
                string afterDecimal = LabResultMapYaleQn.AfterDecimal(num);
                input["AfterDecimal"] = Int32.Parse(afterDecimal);
                input["Number"] = num.ToString();
                if (m.Groups["Inequality"].Value.ToString() != String.Empty)
                    input["Inequality"] = m.Groups["Inequality"].Value.ToString();
                input["Field1"] = "Log10:" + numLog;
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Pretty"] = num.ToString();
            }
            else if (isNum && !isNumLog)  //only a log
            {
                //Check if number is of reasonable size
                if (Math.Pow(10d, (double)num) == Double.PositiveInfinity )
                {
                    input["MappedYN"] = "Y";
                    input["General"] = "Non-standard Result";
                    input["MapFunc"] = this.ToString();
                }
                //Number is of reasonable size
                else
                {
                    decimal newNum = Math.Round((decimal)Math.Pow(10d, (double)num));
                    string afterDecimal = LabResultMapYaleQn.AfterDecimal(num);
                    input["AfterDecimal"] = Int32.Parse(afterDecimal);
                    input["Number"] = newNum.ToString();
                    if (m.Groups["Inequality"].Value.ToString() != String.Empty)
                        input["Inequality"] = m.Groups["Inequality"].Value.ToString();
                    input["Field1"] = "Log10:" + num.ToString();
                    input["MappedYN"] = "Y";
                    input["MapFunc"] = this.ToString();
                    input["Pretty"] = num.ToString();
                }
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
                return;
            }
        }
    }
}
