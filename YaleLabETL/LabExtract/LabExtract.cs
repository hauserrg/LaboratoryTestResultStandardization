using LabEtlTranslate;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MapResult
{
    internal static partial class LabExtract
    {
        public static void Extract(Translate translate)
        {
            //Extraction (mini-factory design pattern)
            switch (translate.Map)
            {
                case Map.Yale:
                    ExtractYale(translate);
                    break;
                case Map.VA:
                    RunVa(translate);
                    break;
                default:
                    throw new Exception("Unrecognized Map.");
            }
        }
    }
}
