using System;
using System.Xml.Linq;
using System.Collections.Generic;



namespace AeroCalcCore
{



    public class ConfigXMLFile : XMLFile
    {
        /*
         * CONSTANTES
         */
        //! Placer les constantes de mapping XML dans la classe de base XMLFile


        /*
         * CONSTRUCTEURS
         */

        public ConfigXMLFile(string absoluteFilePath) : base("", absoluteFilePath)
        {
            // xDoc should be loaded with XML file content
        }



        /// <summary>
        /// 
        /// </summary>
        public Languages GetLanguagesFromXML()
        {
            Languages langs = new Languages();

            if (IOStatus != FileIO.FILEOP_SUCCESSFUL) { return null; }
            // Nodes Languages
            foreach (XElement xe in xDoc.Descendants(NODE_LANGUAGE))
            {
                // This is a Language Node
                string shortName = xe.Attribute(ATTRIB_SHORT_NAME).Value;
                string name = xe.Attribute(ATTRIB_NAME).Value;
                string filePath = xe.Value;
                if (string.IsNullOrEmpty(shortName) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(filePath))
                {
                    // Not a valid Language block
                    break;
                }
                langs.Add(new Language(name, shortName, filePath, true));
            }
            return langs;
        }

    }

}