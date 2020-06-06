using System;
using System.IO; // TODO remove when messages filename integrated with EnvContext


namespace AeroCalcCore
{



    /// <summary>
    /// Classe destinée uniquement au traitement 'prost process' des commandes, pour mise en forme des messages textes.
    /// Utilisera le package de langue
    /// </summary>
    public class PostProcessor
    {

        /*
         * CONSTANTES
         */



        /*
         * PROPRIETES
         */



        /*
         * MEMBRES
         */
        private EventMessages EMsgLib;



        /*
         * CONSTRUCTEUR
         */
        public PostProcessor(EnvironmentContext EC)
        {
            // TODO A intégrer pleinement à l'object EnvironmentContext
            //! Remove when possible
            string fileName = EC.configDirPath + Path.AltDirectorySeparatorChar + "fr.xml";

            EventMessagesXMLFile xmlFile = new EventMessagesXMLFile(fileName);

            EMsgLib = xmlFile.getEventMessagesFromXML();



        }



        /*
         * SERVICES
         */
        public void postProcess(AeroCalcCommand Cmd)
        {

            if (Cmd.eventCode == AeroCalcCommand.EVENTCODE_INITIAL)
            {
                // Command totally unprocessed, not a normal situation
                string msg = "[" + Cmd.eventCode + "]" + " POSTPROC:UNPROCESSED COMMAND";
                Cmd.setResultText(msg);
            }
            if (Cmd.eventCode > 0)
            {
                // Successfull operations
                if (!double.IsNaN(Cmd.numericResult))
                {
                    // A numeric value has been produced
                    Cmd.setResultText(Cmd.rawTxtCommand + " = " + Cmd.numericResult);
                }
                else
                {
                    // No numeric value to expose
                    string msg = EMsgLib.getMessageWith(Cmd.eventCode);
                    if (!string.IsNullOrEmpty(Cmd.txtResult))
                    {
                        // A message has been prepared, NOTHING TO ADD
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(msg))
                        {
                            msg = "[" + Cmd.eventCode + "]" + " POSTPROC:RESULT MSG NOT IMPLEMENTED";
                        }
                        Cmd.setResultText(msg);
                    }
                }
            }
            if (Cmd.eventCode < 0)
            {
                // Error
                string msg = EMsgLib.getMessageWith(Cmd.eventCode);
                if (string.IsNullOrEmpty(msg))
                {
                    // Nothing returned
                    msg = "[" + Cmd.eventCode + "]" + " POSTPROC:ERROR MSG NOT IMPLEMENTED";
                }
                Cmd.setResultText(msg);
            }


        }



        /*
         * METHODES
         */


    }

}