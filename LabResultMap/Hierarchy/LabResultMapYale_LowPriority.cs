using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabResultMap
{
    /// <summary>
    /// Used to identify loinc codes with values that have a low yield to map.
    /// </summary>
    class LabResultMapYale_LowPriority : LabResultMapYale
    {
        private static Dictionary<string, string> lowPriorityLoinc;
        internal LabResultMapYale_LowPriority() : base() 
        {
            if( lowPriorityLoinc == null || lowPriorityLoinc.Count == 0)
                lowPriorityLoinc = LoadDictionary.Resource(Constants.Resource.LabMapYale_LowPriority, false);
        }

        internal override void MapRow(DataRow input)
        {
            string loinc = input[Column.Loinc.ToString()].ToString();
            if( lowPriorityLoinc.ContainsKey(loinc) )
            {
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
            }
        }
    }
}
