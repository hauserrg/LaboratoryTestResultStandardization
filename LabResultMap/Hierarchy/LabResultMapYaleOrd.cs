using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap
{
    class LabResultMapYaleOrd : LabResultMapYale
    {
        internal static Dictionary<string, OrdinalGroup> ordinalGroups;
        internal static Dictionary<string, OrdinalGroup> OrdinalGroups 
        { 
            get 
            {
                if (ordinalGroups == null || ordinalGroups.Count == 0)
                    LoadOrdinalGroups();
 
                return ordinalGroups; 
            }            
        }
        internal LabResultMapYaleOrd() : base()
        {
            if (ordinalGroups == null || ordinalGroups.Count == 0)
                LoadOrdinalGroups();
        }

        internal override void MapRow(System.Data.DataRow input)
        {
            string key = input[Column.Result.ToString()].ToString().Trim();
            if (ordinalGroups.ContainsKey(key))
            {
                OrdinalGroup og = ordinalGroups[key];
                input["Field1"] = og.map;
                input["Field2"] = "Group:" + og.group;
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Pretty"] = og.map;
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
            }            
        }


        internal class OrdinalGroup
        {
            internal string group;
            internal string map;  //raw or unmapped input

            public OrdinalGroup(string group, string map)
            {
                this.group = group;
                this.map = map;
            }
        }
        internal static void LoadOrdinalGroups()
        {
            ordinalGroups = new Dictionary<string, OrdinalGroup>(StringComparer.InvariantCultureIgnoreCase);
            string[] lines = Constants.Resource.LabMapYale_Ord.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var phrases = line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (phrases.Length != 3)
                    continue;

                ordinalGroups.Add(phrases[1], new OrdinalGroup(phrases[0], phrases[2]));
            }
        }
    }
}
