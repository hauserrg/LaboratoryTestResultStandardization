using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabResultMap
{
    class LabResultMapYaleReviewSourceData : LabResultMapYale
    {
        internal LabResultMapYaleReviewSourceData() : base() { }

        /// <summary>
        /// Used for Loinc codes with unreliable source data.
        /// </summary>
        /// <param name="input"></param>
        internal override void MapRow(DataRow input)
        {
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            return;
        }
    }
}
