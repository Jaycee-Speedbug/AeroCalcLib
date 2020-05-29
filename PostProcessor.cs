using System;


namespace AeroCalcCore
{



/// <summary>
/// Classe destinée uniquement au traitement de la commande prost process, pour mise en forme des messages textes
/// Utilisera le package de langue
/// </summary>
    public class PostProcessor
    {

        private EventMessages EMsgLib;

        public PostProcessor(EnvironmentContext EC)
        {
            EMsgLib = new EventMessages();


        }



    }
    
}