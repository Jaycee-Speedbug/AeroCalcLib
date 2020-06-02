using System;


namespace AeroCalcCore
{



    /// <summary>
    /// Classe destinée uniquement au traitement de la commande prost process, pour mise en forme des messages textes
    /// Utilisera le package de langue
    /// </summary>
    public class EventMessage : IEquatable<EventMessage>
    {

        public int msgID { get; private set; }

        public string msgStr { get; private set; }



        public EventMessage(int messageID, string messageString)
        {
            msgID = messageID;
            msgStr = messageString;
        }



        public bool Equals(EventMessage EM)
        {
            /// TODO: simplifier
            if (EM == null)
            {
                return false;
            }
            if (this.msgID == EM.msgID)
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
            return string.Format("[{0:D5}]", this.msgID) + this.msgStr;
        }

    }

}