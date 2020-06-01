using System;
using System.Xml.Linq;



namespace AeroCalcCore
{


    
    public class EventMessagesXMLFile : XMLFile
    {
        /*
         * CONSTANTES
         */
        // XML mapping
        public const string NODE_LANG_PACK = "LanguagePack";
        public const string NODE_MESSAGES = "Messages";
        public const string NODE_MESSAGE = "Message";



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

            // TODO load the file here

            foreach (XElement item in xDoc.Descendants(NODE_MESSAGE))
            {
                // This is a Message node, let's get the data
                string msg = item.Value;
                int codeNb = getIntOrMinValue(item.Attribute(ATTRIB_ID).Value);
                if (codeNb != int.MinValue)
                {
                    EMLib.Add(new EventMessage(codeNb, msg));
                }
            }

            /*
            // Node principal des messages
            XElement mainElement = xDoc.Element(NODE_MESSAGES);
            if (mainElement is null) return EMLib;

            foreach (XElement xe in mainElement.Elements())
            {
                if (xe.Name.LocalName == NODE_MESSAGE)
                {
                    // This is a Message node, let's get the data
                    string msg = xe.Value;
                    int codeNb = getIntOrMinValue(xe.Attribute(ATTRIB_ID).Value);
                    if (codeNb != int.MinValue)
                    {
                        EMLib.Add(new EventMessage(codeNb, msg));
                    }

                }

            }
            */
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