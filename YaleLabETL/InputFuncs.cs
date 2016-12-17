using LabEtlTranslate;
using LabEtlTranslate.XmlInputs;
using log4net;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapResult
{
    enum InputFrom { Args, Resource, Code };

    class InputFuncs
    {
        public static Translate CreateInput(InputFrom inputFrom, string[] args)
        {
            //create serialization strings
            string translateXmlStr;
            switch (inputFrom)
            {
                case InputFrom.Args:
                    LoadInputFromArgs(args, out translateXmlStr);
                    break;
                default:
                    throw new Exception("Unrecognized input source.");
            }

            //initialize classes
            TranslateXml translateXml = SerializeHelper.Deserialize<TranslateXml>(translateXmlStr);
            Translate translate = Translate.TranslateFromXml(translateXml);
            return translate;
        }
        private static void LoadInputFromArgs(string[] args, out string translateXmlStr)
        {
            //args are paths to xml files

            if (args == null || args.Length != 1)
                throw new Exception("Invalid args length (!=1)");

            if (!System.IO.File.Exists(args[0]))
                throw new Exception("Check args file paths");

            translateXmlStr = System.IO.File.ReadAllText(args[0]);
        }
    }
}
