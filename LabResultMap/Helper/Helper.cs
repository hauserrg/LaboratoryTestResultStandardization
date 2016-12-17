using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabResultMap.Helper
{
    /// <summary>
    /// This class helps the Hierarchy to perform certain tasks
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Separates "Pos12.4mIU/mL" into {"Pos", "12.4", "mIU/mL"}.  Returns true if successful.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static bool SeparateDigitsAndWords(string value, List<string> words, List<string> nums)
        {
            if (value == String.Empty) return false;

            StringBuilder currentWord = new StringBuilder();
            CharType currentWordCharType = CharType.Start;
            foreach( var c in value )
            {
                CharType thisCharType = GetCharType(c);
                if (currentWordCharType == CharType.Start) currentWordCharType = thisCharType;
                bool keepGrowing = GrowCurrentWord(currentWord, currentWordCharType, thisCharType, c);
                if (!keepGrowing)
                {
                    StopGrowingWord(words, nums, currentWord, currentWordCharType);
                    currentWord.Append(c);
                    currentWordCharType = thisCharType;
                }
            }
            StopGrowingWord(words, nums, currentWord, currentWordCharType);
            return true;
        }
        #region SeparateDigitsAndWords functions
        private static void StopGrowingWord(List<string> words, List<string> nums, StringBuilder currentWord, CharType currentWordCharType)
        {
            switch (currentWordCharType)
            {
                case CharType.Digit:
                    decimal d;
                    if (Decimal.TryParse(currentWord.ToString(), out d))
                        nums.Add(currentWord.ToString());
                    break;
                case CharType.Whitespace:
                    break;
                case CharType.Other:
                case CharType.Inequality:
                    words.Add(currentWord.ToString());
                    break;
                case CharType.Start:
                    break;
                default:
                    throw new Exception("Add CharType here.");
            }
            currentWord.Clear();
        }
        private static bool GrowCurrentWord(StringBuilder currentWord, CharType currentWordCharType, CharType thisCharType, char c)
        {
            if (currentWordCharType == thisCharType)
            {
                currentWord.Append(c);
                return true;
            }
            else
                return false;
        }
        private enum CharType {Digit, Whitespace, Inequality, Other, Start};
        private static CharType GetCharType(char c)
        {
            if (Char.IsDigit(c) || c == '.')
                return CharType.Digit;
            if (Char.IsWhiteSpace(c))
                return CharType.Whitespace;
            if (c == '>' || c == '<' || c == '=')
                return CharType.Inequality;
            else
                return CharType.Other;
        }
        #endregion

        /// <summary>
        /// Removes the units from the end of a result.  Returns true if successful.
        /// </summary>
        /// <param name="value">Updated by the function to have no units.</param>
        /// <param name="units">Contains a mapped version of the units removed</param>
        /// <returns></returns>
        internal static bool RemoveUnitsFromEnd(ref string value, out string units)
        {
            if (unitDict == null)
            {
                unitDict = SharedLibrary.LoadDictionary.Resource(Constants.Resource.Helper_Units, true);
                unitDictMinLen = unitDict.Keys.Min(x => x.Length);
                unitDictMaxLen = unitDict.Keys.Max(x => x.Length);
            }

            bool foundUnit = false;
            string valueNew = String.Empty, unitsNew = String.Empty, unitsStd = String.Empty;
            for (int i = unitDictMinLen; i <= unitDictMaxLen; i++)
            {
                if (value.Length - i < 0) break;

                string endOfString = value.Substring(value.Length-i);
                if (unitDict.ContainsKey(endOfString))
                {
                    unitsStd = unitDict[endOfString];
                    valueNew = StringUtil.RemoveFromEnd(value, endOfString);
                    unitsNew = endOfString;
                    foundUnit = true;
                }
            }

            if (foundUnit)
            {
                value = valueNew;
                units = unitsStd;
                return true;
            }
            else
            {
                units = String.Empty;
                return false;  //no units at end of string
            }
        }
        private static Dictionary<string, string> unitDict;
        private static int unitDictMaxLen, unitDictMinLen;
    }
}
