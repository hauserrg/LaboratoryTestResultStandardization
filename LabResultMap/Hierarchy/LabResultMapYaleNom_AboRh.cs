using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LabResultMap
{
    //Loinc:  882-1  ABO and Rh
    internal enum Abo { A, B, O, AB };
    internal enum Rh { Positive, Negative };
    class LabResultMapYaleNom_AboRh : LabResultMapYale
    {
        internal LabResultMapYaleNom_AboRh()
            : base() { }


        internal override void MapRow(System.Data.DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();
            string[] parts = value.Split(null);
            if( parts.Length != 2)
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.None";
                return;
            }

            //2.  check ABO and Rh
            Abo abo; 
            Rh rh;
            bool foundABO = FindAbo(parts[0], out abo);
            bool foundRh = FindRh(parts[1], out rh);
            if( !foundABO || !foundRh)
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.None";
                return;
            }
            else
            {
                //"Capitalize words"
                string output = abo.ToString() + " " + rh.ToString();
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Field1"] = output;
                input["Pretty"] = output;
                
                return;
            }
        }

        private static string CapitalizeWords(string value)
        {
            if( value == null )
                throw new ArgumentNullException("value");
            if( value.Length == 0 )
                return value;

            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for( int i = 1; i < result.Length; ++i )
            {
                if( char.IsWhiteSpace(result[i - 1]) )
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }

        private bool FindRh(string rh, out Rh r)
        {
            if (Enum.TryParse(rh, true, out r)) //true to ignore case
                if (Enum.IsDefined(typeof(Rh), r))
                    return true;
            return false;
        }

        internal static bool FindAbo(string abo, out Abo a)
        {
            //Validation
            if (String.Compare(abo, "0") == 0)
                abo = "O";
            if (Char.IsNumber(abo[0]))
            {
                a = Abo.A;
                return false;
            }
                
            //Execution
            if (Enum.TryParse(abo, out a)) 
                //http://stackoverflow.com/questions/6741649/enum-tryparse-returns-true-for-any-numeric-values
                if( Enum.IsDefined(typeof(Abo), a))
                    return true;

            return false;
        }
    }
}
