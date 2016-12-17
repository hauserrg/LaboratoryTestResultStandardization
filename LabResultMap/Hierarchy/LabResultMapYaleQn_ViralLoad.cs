using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace LabResultMap
{
    /// <summary>
    /// Used to map values like this "27753 (4.4 LOG10)"
    /// </summary>
    class LabResultMapYaleQn_ViralLoad : LabResultMapYale
    {
        internal LabResultMapYaleQn_ViralLoad() : base() { }

        internal override void MapRow(System.Data.DataRow input)
        {
            // Match the regular expression pattern against a text string.
            //string pattern = @"([0-9.]*)\s*\(([0-9.]*)\s*LOG\s*10\)";
            //Note:  Groups=  Inequality, NotLog, Inequality2, Log
            XmlDocument numericXml = new XmlDocument();
            numericXml.LoadXml(Constants.Resource.LabMapYale_ViralLoad);
            var regexNode = numericXml.GetElementsByTagName("regex");
            string pattern_million = regexNode[0].InnerText.Trim();
            string pattern_numeric = regexNode[1].InnerText.Trim();
            string pattern_exponentialLog = regexNode[2].InnerText.Trim();
            string pattern_notExponentialLog = regexNode[3].InnerText.Trim();

            Regex regex_notExponentialLog = new Regex(pattern_notExponentialLog, RegexOptions.IgnoreCase);
            Regex regex_exponentialLog = new Regex(pattern_exponentialLog, RegexOptions.IgnoreCase);
            Regex regex_numeric = new Regex(pattern_numeric, RegexOptions.IgnoreCase);
            //Regex regex_million = new Regex(pattern_million, RegexOptions.IgnoreCase);

            string result = input[Column.Result.ToString()].ToString();

            Match match_notExponentialLog = regex_notExponentialLog.Match(result);
            if (match_notExponentialLog.Success)
                Match_NotExponentialLog(input, match_notExponentialLog);
            else
            {
                Match match_exponentialLog = regex_exponentialLog.Match(result); //groups 1 (inequality), 2 (viral load), 11 (log viral load)
                if (match_exponentialLog.Success)
                    Match_ExponentialLog(input, match_exponentialLog);
                else
                {
                    Match match_numeric = regex_numeric.Match(result); //groups 1 (inequality), 2 (viral load), 11 (log viral load)
                    if (match_numeric.Success)
                        Match_Numeric(input, match_numeric);
                    else
                    {
                        //Match match_million = regex_notExponentialLog.Match(result); //groups 1 (inequality), 2 (viral load), 11 (log viral load)
                        //if (match_million.Success)
                        //    Match_Million(input, match_million);
                        //else
                        //{
                        input["MappedYN"] = "N";
                        input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
                        //}
                    }
                }
            }
        }

        //private void Match_Million(System.Data.DataRow input, Match match_million)
        //{
        //    //groups 1 (inequality), 2 (viral load), 11 (log viral load)

        //    var inequality = match_million.Groups[1].Value.ToString();
        //    var num = match_million.Groups[2].Value.ToString();
        //    var log = match_million.Groups[11].Value.ToString();

        //    if (inequality != String.Empty)
        //        input["Inequality"] = inequality;
        //    input["Number"] = num;
        //    input["AfterDecimal"] = 0;
        //    if (log != String.Empty)
        //        input["Field1"] = "Log10:" + log;
        //    input["MappedYN"] = "Y";
        //    input["MapFunc"] = this.ToString();
        //    input["Pretty"] = num.ToString();
        //}

        private void Match_Numeric(System.Data.DataRow input, Match match_numeric)
        {
            //groups 1 (inequality), 2 (viral load), 11 (log viral load)

            var inequality = match_numeric.Groups[1].Value.ToString();
            var num = match_numeric.Groups[2].Value.ToString();

            if (inequality != String.Empty)
                input["Inequality"] = inequality;
            input["Number"] = num;
            input["AfterDecimal"] = 0;
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Pretty"] = num.ToString();
        }

        private void Match_ExponentialLog(System.Data.DataRow input, Match match_exponentialLog)
        {
            //groups 1 (inequality), 2 (viral load), 11 (log viral load)

            var inequality = match_exponentialLog.Groups[1].Value.ToString();
            var num = match_exponentialLog.Groups[2].Value.ToString();
            var exp = match_exponentialLog.Groups[9].Value.ToString();

            Double numDouble, expDouble = 0d;
            if (!Double.TryParse(num, out numDouble) || !Double.TryParse(exp, out expDouble))
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
            }
            var numInt = (int)(numDouble * Math.Pow(10d, expDouble));

            if (inequality != String.Empty)
                input["Inequality"] = inequality;
            input["Number"] = numInt.ToString();
            input["AfterDecimal"] = 0;
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Pretty"] = num.ToString();
        }

        private void Match_NotExponentialLog(System.Data.DataRow input, Match match_notExponentialLog)
        {
            //groups 1 (inequality), 2 (viral load), 11 (log viral load)

            var inequality = match_notExponentialLog.Groups[1].Value.ToString();
            var num = match_notExponentialLog.Groups[2].Value.ToString();
            var log = match_notExponentialLog.Groups[12].Value.ToString();

            if (inequality != String.Empty)
                input["Inequality"] = inequality;
            input["Number"] = num;
            input["AfterDecimal"] = 0;
            if (log != String.Empty)
                input["Field1"] = "Log10:" + log;
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Pretty"] = num.ToString();
        }
    }
}
