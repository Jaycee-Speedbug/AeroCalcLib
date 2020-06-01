using System;


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
            EventMessagesXMLFile xmlFile = new EventMessagesXMLFile("");

            EMsgLib = xmlFile.getEventMessagesFromXML();



        }



        /*
         * SERVICES
         */
        public void processCommand(AeroCalcCommand Cmd)
        {

            if (Cmd.eventCode == AeroCalcCommand.EVENTCODE_INIT_VALUE)
            {
                // Command totally unprocessed, not a normal situation
                string msg = "[" + Cmd.eventCode + "]" + " POSTPROC:UNPROCESSED COMMAND";
                Cmd.setResultText(msg);
            }
            if (Cmd.eventCode > 0)
            {
                // Successfull operations
                if (Cmd.numericResult != double.NaN)
                {
                    // A numeric value has been produced
                    Cmd.setResultText("");
                }
                else
                {
                    // No numeric value to expose
                    string msg = EMsgLib.getMessageWith(Cmd.eventCode);
                    if (string.IsNullOrEmpty(msg))
                    {
                        msg = "[" + Cmd.eventCode + "]" + " POSTPROC:ERROR MSG NOT IMPLEMENTED";
                    }
                    Cmd.setResultText(msg);
                }
            }
            if (Cmd.eventCode < 0)
            {
                // Error
                string msg = EMsgLib.getMessageWith(Cmd.eventCode);
                if (string.IsNullOrEmpty(msg))
                {
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