using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LabResultMap
{
    /// <summary>
    /// This did not work well because it recognized typos as dates. ,6.9 -> 6/9/2015
    /// </summary>
    class LabResultMapYale_DateTime : LabResultMapYale
    {
        internal LabResultMapYale_DateTime() : base() { }

        internal override void MapRow(System.Data.DataRow input)
        {

            //See if it fits the pattern.
            DateTime dateTime;
            if( DateTime.TryParse(input[Column.Result.ToString()].ToString(), out dateTime) )
            {
                input["Field1"] = "IsDate";
                input["Field2"] = dateTime.ToString("yyyy-MM-dd");
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Pretty"] = "Date=" + input["Field2"].ToString();
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";
            }
        }
    }
}
