using System;
using System.Collections.Generic;



namespace AeroCalcCore
{

    /// <summary>
    /// Classe de dimension 1 du package 'Calculateur de performances de vol'
    /// Enregistre les caractéristiques d'un point de performance de vol
    /// </summary>
    /// 
    public readonly struct PerfPoint : IComparable<PerfPoint>
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
        }

        /// <summary>
        /// 'Ordonnée' du point de performance. Image de factorValue par la fonction de performance décrite par la
        /// série contenant ce point
        /// </summary>
        public double output
        {
            get;
        }

        /// <summary>
        /// Etat 'breakpoint' du point de performance (rupture de linéarité dans le modèle)
        /// </summary>
        public bool isBreak
        {
            get;
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
        }



        // SERVICES /////////////////////////////////////////////////////////////////////////////////////////



        /// <summary>
        /// Retourne une chaine de caractère présentant toutes les caractéristiques du Point de performances
        /// </summary>
        /// <returns>String, descriptive du Point</returns>
        ///
        public override String ToString()
        {
            string msg = "X= " + factorValue;
            msg += "\nY= " + output;
            if (isBreak) {
                msg += "\nBreak point : YES\n";
            }
            else {
                msg += "\nBreak point : NO\n";
            }
            return "";
        }


        // INTERFACE(S) /////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Comparaison par rapport à un autre PerfPoint
        /// L'abscisse est le critère déterminant car dans une PerfSerie, les points sont ordonnés selon leurs abscisses
        /// </summary>
        /// <param name="pp">Point de performance à qui se comparer</param>
        /// <returns>-1 si pp1 est avant pp2, O si pp1 = pp2, 1 si pp1 est plus grand que pp2</returns>
        ///
        public int CompareTo(PerfPoint pp) => factorValue.CompareTo(pp.factorValue);


    }

}