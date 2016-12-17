using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedLibrary;

namespace LabResultMap
{
    //Property == 'ACnc' == Arbitrary concentration == titer?

    class LabResultMapYaleQn_Titer : LabResultMapYaleQn
    {
        internal LabResultMapYaleQn_Titer() : base() {}

        internal override void MapRow(DataRow input)
        {
            string value = input[Column.Result.ToString()].ToString();

            //1.  Look for inequality, remove if found.
            string testValue = value.TrimStart();
            string inequality = String.Empty;
            testValue = Inequality(testValue, ref inequality);
            testValue = testValue.Trim();

            //2.  Find colon
            string[] colonSides = testValue.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            if (colonSides.Length != 2)  //more or less than a single colon
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            //3.  assemble titer
            string titer = String.Empty;
            decimal left, right;
            if( decimal.TryParse(colonSides[0], out left)
                && decimal.TryParse(colonSides[1], out right))
            {
                if (colonSides[1].EndsWith("."))
                    colonSides[1] = colonSides[1].RemoveFromEnd(".");
                titer = colonSides[0] + ":" + colonSides[1];
            }

            //4.  Save
            if (titer == String.Empty)
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
                return;
            }

            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Field1"] = titer;
            input["Inequality"] = inequality;

            return;
        }
    }
}
