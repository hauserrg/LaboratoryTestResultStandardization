using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap
{
    class LabResultMapYaleNom : LabResultMapYale
    {
        internal static Dictionary<string, NominalGroup> nominalGroups;
        internal LabResultMapYaleNom() : base()
        {
            if (nominalGroups == null || nominalGroups.Count == 0)
                LoadNominalGroups();
        }

        internal override void MapRow(System.Data.DataRow input)
        {
            string key = input[Column.Result.ToString()].ToString(); //No trim compared to ordinal
            if (nominalGroups.ContainsKey(key))
            {
                NominalGroup ng = nominalGroups[key];
                input["Field1"] = ng.map;
                input["Field2"] = "Group:" + ng.group;
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Pretty"] = ng.map;
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
            }
        }


        internal class NominalGroup
        {
            internal string group;
            internal string map;  //raw or unmapped input

            public NominalGroup(string group, string map)
            {
                this.group = group;
                this.map = map;
            }
        }
        internal void LoadNominalGroups()
        {
            nominalGroups = new Dictionary<string, NominalGroup>(StringComparer.InvariantCultureIgnoreCase);
            string[] lines = Constants.Resource.LabMapYale_Nom.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var phrases = line.Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
                if (phrases.Length != 3)
                    continue;

                nominalGroups.Add(phrases[1], new NominalGroup(phrases[0], phrases[2]));
            }
        }
    }
}
