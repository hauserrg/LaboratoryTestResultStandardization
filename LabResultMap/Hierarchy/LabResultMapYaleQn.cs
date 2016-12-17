using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LabResultMap
{
    class LabResultMapYaleQn : LabResultMapYale
    {
        internal LabResultMapYaleQn() : base() { }

        internal override void MapRow(DataRow input)  
        {
            string value = input[Column.Result.ToString()].ToString().Trim();

            //1.  Prefix
            //a.  Inequality
            //b.  VA "?"  (?12.12)
            string question, inequality;
            string testValue = value;
            QnPrefix(ref testValue, out question, out inequality);
            
            //2.  Remove suffix L or H (low or high)
            string percent;
            QnSuffix(ref testValue, out percent);
            
            //Inequality suffix
            if (LabResultMapYaleQn_IgEAb.EndInequality(input))
            {
                input["MapFunc"] = "LabResultMap.LabResultMapYaleQn";
                return;
            }

            testValue = testValue.Trim();

            //3.  Numeric?
            decimal deci = 0;
            double doub = 0;
            string numeric = String.Empty;
            bool noPlus = NoPlus(testValue);
            bool deciQ = decimal.TryParse(testValue, out deci);
            if( noPlus && deciQ)
                numeric = testValue.ToString();

            //Try conversion to double for exponential notation...
            if (!deciQ)
                if( noPlus && double.TryParse(testValue, out doub) )
                    numeric = StringUtil.ToFloatingPointString(doub);

            if (numeric.EndsWith("-")) //No "-" ending... which is a number.
                numeric = String.Empty;

            //Improve the format of certain numbers (-0, 0., 1.)
            if(numeric.ToString().StartsWith("-") ||
                numeric.ToString().StartsWith("0") ||
                numeric.ToString().StartsWith(".") ||
                numeric.ToString().EndsWith(".") )
                numeric = ImproveFormat(numeric);

            //4.  Significant digits
            string afterDecimal = String.Empty;
            if (deciQ)                      
                afterDecimal = AfterDecimal(deci);    


            //5.  Save

            if (numeric == String.Empty )
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;        
            }
                
            
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Number"] = numeric;

            //inequality
            if (inequality == String.Empty)
                input["Inequality"] = DBNull.Value;
            else
                input["Inequality"] = inequality;

            //question
            if (question != String.Empty)
                input["Field1"] = "Prefix '?'";

            //percent
            if (percent != String.Empty)
                input["Field2"] = "Suffix '%'";

            //afterDecimal
            if (afterDecimal == String.Empty)
                input["AfterDecimal"] = DBNull.Value;
            else
                input["AfterDecimal"] = Int32.Parse(afterDecimal);

            //pretty print
            input["Pretty"] = inequality + numeric;

            return;
        }

        internal string ImproveFormat(string numeric)
        {
            /* Meant to improve the format of certain numeric results:
             * Trailing decimal: -12. --> -12
               Negative zero: -0 --> 0
               Leading zero: 00 --> 0
               Leading decimal: .001 --> 0.001
               (A combination of these.)
             * */

            if( numeric.EndsWith("."))
                numeric = numeric.RemoveFromEnd(".");

            if( 0 == String.Compare(numeric, "-0"))
                numeric = "0";

            while( numeric.StartsWith("0") && numeric.Length > 1)
                numeric = numeric.RemoveFromStart(1);

            if( numeric.StartsWith("."))
                numeric = "0" + numeric;
            
            return numeric;
        }

        internal static void QnSuffix(ref string testValue, out string percent)
        {
            percent = String.Empty;

            if (testValue.EndsWith("L"))
                testValue = SharedLibrary.StringUtil.RemoveFromEnd(testValue, "L");
            else if (testValue.EndsWith("H"))
                testValue = SharedLibrary.StringUtil.RemoveFromEnd(testValue, "H");
            else if (testValue.EndsWith("H*"))
                testValue = SharedLibrary.StringUtil.RemoveFromEnd(testValue, "H*");
            else if (testValue.EndsWith("*"))
                testValue = SharedLibrary.StringUtil.RemoveFromEnd(testValue, "*");
            else if (testValue.EndsWith("%"))
            {
                testValue = SharedLibrary.StringUtil.RemoveFromEnd(testValue, "%");
                percent = "%";
            }
        }

        internal static void QnPrefix(ref string testValue, out string question, out string inequality)
        {
            question = inequality = String.Empty;

            //1.  Create pattern
            string prefixPattern = @"^(?<Question>\?)?(?<Inequality><=|>=|=>|<|>)?"; //? or inequality
            Regex regex = new Regex(prefixPattern);

            //2.  Match
            Match m = regex.Match(testValue);
            if (m.Groups["Question"].ToString() != String.Empty)
            {
                question = @"?";
                testValue = testValue.Substring(1);
            }
            if (m.Groups["Inequality"].ToString() != String.Empty)
            {
                //save inequality
                if (m.Groups["Inequality"].ToString() != "=>" )
                    inequality = m.Groups["Inequality"].ToString();
                else if (m.Groups["Inequality"].ToString() == "=>" )
                    inequality = ">=";

                //remove from string
                if (inequality.Length == 1)
                    testValue = testValue.Substring(1);
                else if (inequality.Length == 2)
                    testValue = testValue.Substring(2);
            }
        }

        private bool NoPlus(string testValue)
        {
            if (testValue.Contains('+'))
                return false;
            else
                return true;
        }

        internal static string AfterDecimal(decimal d)
        {
            int count = BitConverter.GetBytes(decimal.GetBits(d)[3])[2];
            return count.ToString();
        }

        protected string Inequality(string value, ref string inequality)
        {
            if (value.StartsWith("<="))
            {
                inequality = "<=";
                return value.Substring(2,value.Length-2);
            }

            if (value.StartsWith(">="))
            {
                inequality = ">=";
                return value.Substring(2, value.Length - 2);
            }

            if (value.StartsWith("<"))
            {
                inequality = "<";
                return value.Substring(1, value.Length - 1);
            }

            if (value.StartsWith(">"))
            {
                inequality = ">";
                return value.Substring(1, value.Length - 1);
            }

            inequality = null;  //To prevent blank inequality 
            return value;
        }
    }
}
