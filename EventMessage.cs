using System;


namespace AeroCalcCore
{



    /// <summary>
    /// Classe destinée uniquement au traitement de la commande prost process, pour mise en forme des messages textes
    /// Utilisera le package de langue
    /// </summary>
    public class EventMessage : IEquatable<EventMessage>
    {

        public int eCode { get; private set; }

        public string eMessage { get; private set; }



        public EventMessage(int eventCode, string eventMessage)
        {
            eCode = eventCode;
            eMessage = eventMessage;
        }


        
        public bool Equals(EventMessage EM)
        {
            /// TODO: simplifier
            if (EM == null)
            {
                return false;
            }
            if (this.eCode == EM.eCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        public override string ToString()
        {
            return "[" + this.eCode + "] " + this.eMessage;
        }

    }

}