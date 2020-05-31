using System;


namespace AeroCalcCore
{



/// <summary>
/// Classe destinée uniquement au traitement 'prost process' des commandes, pour mise en forme des messages textes.
/// Utilisera le package de langue
/// </summary>
    public class PostProcessor
    {

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

    }
    
}