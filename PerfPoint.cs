using System;
using System.Collections.Generic;



namespace AeroCalcCore
{



    /// <summary>
    /// Classe de dimension 1 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un point de performance de vol
    /// </summary>
    /// 
    public class PerfPoint : IComparer<PerfPoint>
    {

        // FIELDS ////////////////////////////////////////////////////////////////////////////////////


        // PROPERTIES ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Valeur du facteur (Abscisse) du point de performance
        /// </summary>
        /// <remarks>Dim x</remarks>
        public double factorValue
        {
            get;
            private set;
        }

        /// <summary>
        /// 'Ordonnée' du point de performance. Image de factorValue par la fonction de performance décrite par la
        /// série contenant ce point
        /// </summary>
        public double output
        {
            get;
            private set;
        }

        /// <summary>
        /// Etat 'breakpoint' du point de performance (rupture de linéarité dans le modèle)
        /// </summary>
        public bool isBreak
        {
            get;
            private set;
        }

        /// <summary>
        /// Méthode de prédiction préférée pour calculer l'image d'un réel à proximité du point de performance
        /// </summary>
        public int optimizedMethod
        {
            get;
            set;
        }

        /// <summary>
        /// Etat de sélection du point de performance. La sélection permet de ne prendre en compte
        /// que certains points pour les calculs de prédiction
        /// </summary>
        public bool selected
        {
            get;
            set;
        }


        // PRIVATE FIELDS ///////////////////////////////////////////////////////////////////////////////////


        // CONSTRUCTOR(S) ///////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Construit un point de performance
        /// </summary>
        /// <param name="pp1">Abscisse du point</param>
        /// <param name="pp2">Ordonnée du point</param>
        /// <param name="isBreakPoint">Breakpoint</param>
        ///
        public PerfPoint(double input, double output, bool isBreakPoint) {
            this.factorValue = input;
            this.output = output;
            this.isBreak = isBreakPoint;
        }


        /// <summary>
        /// Construit un point de performance par clonage
        /// </summary>
        /// <param name="pp">Point de performance à copier</param>
        ///
        public PerfPoint(PerfPoint pp) {
            this.factorValue = pp.factorValue;
            this.output = pp.output;
            this.isBreak = pp.isBreak;
            this.optimizedMethod = pp.optimizedMethod;
            this.selected = pp.selected;
        }


        // SERVICES /////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Vérifie si deux points disposent de la même abscisse
        /// </summary>
        /// <param name="pp1">Point 1</param>
        /// <param name="pp2">Point 2</param>
        /// <returns>True si les deux points ont la même abscisse, False dans les autres cas</returns>
        ///
        public static bool areColocated(PerfPoint pp1, PerfPoint pp2) {
            if (pp1.factorValue == pp2.factorValue) {
                return true;
            }
            else {
                return false;
            }
        }


        /// <summary>
        /// Retourne une chaine de caractère présentant toutes les caractéristiques du Point de performances
        /// </summary>
        /// <returns>String, descriptive du Point</returns>
        ///
        public override String ToString() {
            String msg = "";

            msg = "X= " + factorValue;
            msg += "\nY= " + output;
            if (isBreak) {
                msg += "\nBreak point : YES\n";
            }
            else {
                msg += "\nBreak point : NO\n";
            }
            msg += "Optimized method : " + optimizedMethod;
            if (selected) {
                msg += "\nSelected : YES\n\n";
            }
            else {
                msg += "\nSelected : NO\n\n";
            }
            return msg;
        }


        // INTERFACE(S) /////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Compare deux 'PerfPoint' entre eux.
        /// L'abscisse est le critère déterminant car dans une 'PerfSerie', les points
        /// sont ordonnés selon leurs abscisses
        /// </summary>
        /// <param name="pp1">Point de performance à comparer</param>
        /// <param name="pp2">Point de performance à comparer</param>
        /// <returns>-1 si pp1 est avant pp2, O si pp1 = pp2, 1 si pp1 est plus grand que pp2</returns>
        ///
        public int Compare(PerfPoint pp1, PerfPoint pp2) {
            if (pp1.factorValue < pp2.factorValue) {
                return -1;
            }
            if (pp1.factorValue > pp2.factorValue) {
                return 1;
            }
            return 0;
        }

    }

}