using System;



namespace AeroCalcCore.FlightPerformanceEngine
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
        /// Valeur input/abscisse du point de performance.
        /// </summary>
        /// <remarks>Dim x</remarks>
        public double input
        {
            get;
        }

        /// <summary>
        /// Valeur output/ordonnée du point de performance.
        /// Image de input par la fonction de performance modélisée par la série contenant ce point.
        /// </summary>
        public double output
        {
            get;
        }

        /// <summary>
        /// Etat de linéarité du point de performance (TRUE si une rupture de linéarité dans le modèle est observée au point de performance)
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
        /// <param name="input">Abscisse du point</param>
        /// <param name="output">Ordonnée du point</param>
        /// <param name="isBreakPoint">Breakpoint</param>
        ///
        public PerfPoint(double input, double output, bool isBreakPoint)
        {
            this.input = input;
            this.output = output;
            this.isBreak = isBreakPoint;
        }



        /// <summary>
        /// Construit un point de performance par clonage
        /// </summary>
        /// <param name="pp">Point de performance à copier</param>
        ///
        public PerfPoint(PerfPoint pp)
        {
            this.input = pp.input;
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
            string msg = "X= " + input;
            msg += "\nY= " + output;
            if (isBreak)
            {
                msg += "\nBreak point : YES\n";
            }
            else
            {
                msg += "\nBreak point : NO\n";
            }
            return msg;
        }


        // INTERFACE(S) /////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Comparaison par rapport à un autre PerfPoint
        /// L'abscisse est le critère déterminant car dans une PerfSerie, les points sont ordonnés selon leurs abscisses
        /// </summary>
        /// <param name="pp">Point de performance à qui se comparer</param>
        /// <returns>-1 si pp1 est avant pp2, O si pp1 = pp2, 1 si pp1 est plus grand que pp2</returns>
        ///
        public int CompareTo(PerfPoint pp) => input.CompareTo(pp.input);


    }

}