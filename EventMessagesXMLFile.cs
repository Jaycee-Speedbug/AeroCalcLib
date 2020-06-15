using System;
using System.Xml.Linq;



namespace AeroCalcCore
{



    public class EventMessagesXMLFile : XMLFile
    {
        /*
         * CONSTANTES
         */
        //! Placer les constantes de mapping XML dans la classe de base XMLFile



        /*
         * CONSTRUCTEURS
         */

        public EventMessagesXMLFile(string absoluteFilePath) : base("", absoluteFilePath)
        {
            // xDoc should be loaded with XML file content
        }



        /*
         * SERVICES
         */

        public EventMessages getEventMessagesFromXML()
        {
            EventMessages EMLib = new EventMessages();
            string[] splitters = { "\r\n", "\n" };

            if (xDoc != null) {
                foreach (XElement item in xDoc.Descendants(NODE_MESSAGE)) {
                    // This is a Message node, let's get the data
                    string msg = item.Value;
                    int codeNb = getIntOrMinValue(item.Attribute(ATTRIB_ID).Value);
                    if (codeNb != int.MinValue) {
                        // Triming the leading white spaces
                        string msgTrimmed = "";
                        string[] table = msg.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < table.Length; i++) {
                            msgTrimmed += table[i].TrimStart(' ') + Environment.NewLine;
                        }
                        // Addition to library
                        EMLib.Add(new EventMessage(codeNb, msgTrimmed));
                    }
                }
            }
            return EMLib;
        }

        public EventMessages getEventMessagesFromXML(string absoluteFilePath)
        {
            EventMessages EMLib = new EventMessages();
            if (setInputFileAbsolutePath(absoluteFilePath))
            {
                return getEventMessagesFromXML();
            }
            return EMLib;
        }



        /*
         *  METHODES 
         */

    }

}