using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap
{
    class LabResultMapYaleHepCGenotype : LabResultMapYale
    {
        private static Dictionary<string, string> genotypes;
        internal LabResultMapYaleHepCGenotype()
            : base()
        {
            if (genotypes == null || genotypes.Count == 0)
                genotypes = SharedLibrary.LoadDictionary.Resource(Constants.Resource.LabMapYale_HepCGenotype, true);
        }

        internal override void MapRow(System.Data.DataRow input)
        {
            string key = input[Column.Result.ToString()].ToString().Trim();
            if (genotypes.ContainsKey(key))
            {
                input["Field1"] = "HepatitisCGenotype:" + genotypes[key];
                input["MappedYN"] = "Y";
                input["MapFunc"] = this.ToString();
                input["Pretty"] = genotypes[key];
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = this.ToString();
            }
        }
    }
}
