using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap
{
    class LabResultMapYaleOrd_FirstOrLast : LabResultMapYale
    {
        Dictionary<string, string> negationWords;
        internal LabResultMapYaleOrd_FirstOrLast() : base()
        {
            if (negationWords == null || negationWords.Count == 0)
                negationWords = SharedLibrary.LoadDictionary.Resource(Constants.Resource.LabMapYale_NegationWords, true);
        }

        internal override void MapRow(System.Data.DataRow input)
        {
            string result = input[Column.Result.ToString()].ToString();
            List<string> words = new List<string>();
            List<string> nums = new List<string>();
            if( !Helper.Helper.SeparateDigitsAndWords(result, words, nums) )
                UpInputMappedN(input);

            //Try ordinal map first, especially useful for tricky maps
            LabResultMapYale ordinal = new LabResultMapYaleOrd();
            ordinal.MapRow(input);
            if (String.Compare((string)input["MappedYN"],"Y") == 0)
                return;

            if( words.Count == 0)
                UpInputMappedN(input);
            else if (LabResultMapYaleOrd.OrdinalGroups.ContainsKey(words[0]) )//first
            {
                int saveIndex = 0;
                UpdateInputMappedY(input, words, saveIndex);
            }
            else if( words.Count > 1 &&  //"Screen positive"
                ( words[0] == "SCR" || words[0] == "SCREEN" || words[0] == "SCRN" || words[0] == "SRN"  ) &&
                LabResultMapYaleOrd.OrdinalGroups.ContainsKey(words[1])
            )
            {
                int saveIndex = 1;
                UpdateInputMappedY(input, words, saveIndex);
            }
            else if (LabResultMapYaleOrd.OrdinalGroups.ContainsKey(words[words.Count - 1])) //last
            {
                //check if second to last word is a negation word ("NOT DETECTED")
                if (words.Count - 1 >= 0 && negationWords.ContainsKey(words[words.Count - 2]))
                {
                    UpInputMappedNegation(input);
                    return;
                }
                    
                int saveIndex = words.Count - 1;
                UpdateInputMappedY(input, words, saveIndex);
            }
            else
            {
                input["MappedYN"] = "N";
                input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";  //Try to map with another function.
            }
        }

        private void UpInputMappedNegation(System.Data.DataRow input)
        {
            //This assumes no double negations (NOT NOT DETECTED)
            input["Field1"] = "Neg";  //Assumed
            input["Field2"] = "Group:Binary";   //Assumed
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Pretty"] = "Neg";    //Assumed
        }

        private static void UpInputMappedN(System.Data.DataRow input)
        {
            input["MappedYN"] = "N";
            input["MapFunc"] = "LabResultMap.LabResultMapYale_Any";  //Try to map with another function.            
        }

        private void UpdateInputMappedY(System.Data.DataRow input, List<string> words, int saveIndex)
        {
            var og = LabResultMapYaleOrd.OrdinalGroups[words[saveIndex]];
            input["Field1"] = og.map;
            input["Field2"] = "Group:" + og.group;
            input["MappedYN"] = "Y";
            input["MapFunc"] = this.ToString();
            input["Pretty"] = og.map;
        }
    }
}
