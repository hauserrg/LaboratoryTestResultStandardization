using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabResultMap
{
    class LabResultMapYaleNotALab : LabResultMapYale
    {
        internal LabResultMapYaleNotALab() : base() { }

        /// <summary>
        /// The result is not from a lab (ie, ventilation mode)
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
