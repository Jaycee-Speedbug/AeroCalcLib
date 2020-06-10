using System;
using System.Collections.Generic;
using System.Text;

namespace AeroCalcCore
{
    public class MemoryStack
    {


        /*
         * Membres
         */
        List<CommandFactor> stackTable;


        /*
         * Constructeurs
         */

        /// <summary>
        /// Construit un registre mémoire par copie d'un registre existant
        /// </summary>
        /// <param name="otherStack"></param>
        public MemoryStack(List<CommandFactor> otherStack) {

        }



        /// <summary>
        /// Construit un registre vide
        /// </summary>
        public MemoryStack() {
            stackTable = new List<CommandFactor>();
        }



        /*
         * Services
         */

        /// <summary>
        /// Retourne un objet CommandFactor dont le nom est en passé en paramètre
        /// </summary>
        /// <param name="factorName"></param>
        /// <returns></returns>
        public CommandFactor findFactor(string factorName) {
            return stackTable.Find(x => x.name.Equals(factorName));
        }



        /// <summary>
        /// Retourne la valeur du CommandFactor dont le nom est passé en paramètre
        /// </summary>
        /// <param name="factorName"></param>
        /// <returns></returns>
        public double valueOfFactor(string factorName) {
            return stackTable.Find(x => x.name.Equals(factorName)).value;
        }



        /// <summary>
        /// Ajoute un CommandFactor au registre
        /// </summary>
        /// <param name="factor"></param>
        /// <returns>True en cas de succès</returns>
        public bool addFactor(CommandFactor factor) {
            if (stackTable.Exists(x => x.name.Equals(factor.name))) {
                return false;
            }
            else {
                stackTable.Add(factor);
                return true;
            }

        }



        /// <summary>
        /// Supprime du registre le facteur dont le nom est passé en argument
        /// </summary>
        /// <param name="factorName"></param>
        /// <returns>True en cas de succès</returns>
        public bool removeFactor(string factorName) {

            return stackTable.Remove(stackTable.Find(x => x.name.Equals(factorName)));
        }



        // TODO to be reworked
        public override bool Equals(object obj) {
            return base.Equals(obj);
        }



        // TODO To be reworked
        public override int GetHashCode() {
            return base.GetHashCode();
        }



        // TODO To be reworked
        public override string ToString() {
            return base.ToString();
        }

    }

}
