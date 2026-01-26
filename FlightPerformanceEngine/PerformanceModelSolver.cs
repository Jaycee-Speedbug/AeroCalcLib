using System;


namespace AeroCalcCore.FlightPerformanceEngine
{

    /// <summary>
    /// Classe chargée de réaliser une interpolation polynomiale de degré n
    /// </summary>
    public class PerformanceModelSolver
    {

        // Public fields


        // Private fields

        PerfSerie perfSerie;

        double[] ppX;
        double[] ppY;


        // CONSTRUCTEUR(S)


        /// <summary>
        /// Construit un objet destiné à réaliser des interpolations polynomiales
        /// </summary>
        /// <param name="ps">
        /// Série de points de performances de vol
        /// </param>
        /// <remarks>
        /// TODO: Améliorer la gestion des exceptions (cas du système avec moins de 2 points)
        /// </remarks>
        public PerformanceModelSolver(PerfSerie ps)
        {
            this.perfSerie = ps;
            if (ps == null)
            {
                throw new ModelException(AeroCalc.E_VOID_SYSTEM, "", "", Double.NaN);
            }
            if (ps.count < 2)
            {
                throw new ModelException(AeroCalc.E_TOO_SHORT_SERIE, "", "", Double.NaN);
            }

            ppX = new double[ps.count];
            ppY = new double[ps.count];
            for (int count = 0; count < ps.count; count++)
            {
                ppX[count] = ps.pointAt(count).input;
                ppY[count] = ps.pointAt(count).output;
            }
        }



        /*
         * SERVICES
         */

        /// <summary>
        /// Interpolation polynomiale de degrés n.
        /// Pour une valeur x passée en argument, sur la base des points de performance de la PerfSerie
        /// passée au constructeur, renvoie la valeur interpolée.
        /// </summary>
        /// <param name="x">Valeur pour laquelle l'interpolation est demandée</param>
        /// <returns>
        /// Renvoie la valeur prédite par interpolation, NaN en cas d'échec
        /// </returns>
        public double interpolateLagrange(double x)
        {

            // Recherche des points de plus grande proximité
            //int[] orderedPointsIndexes = orderedIndexesByDistance(x);

            // Calcul des polynomes
            double[] p = new double[ppX.Length];
            double numerateur;
            double denominateur;

            for (int count = 0; count < p.Length; count++)
            {
                numerateur = 1;
                denominateur = 1;
                // Calcul du numerateur
                for (int counter = 0; counter < p.Length; counter++)
                {
                    if (count == counter)
                    {
                        // Pas de produit à calculer
                    }
                    else
                    {
                        numerateur *= x - ppX[counter];
                    }
                }
                // Calcul du dénominateur
                for (int counter = 0; counter < p.Length; counter++)
                {
                    if (count == counter)
                    {
                        // Pas de produit à calculer
                    }
                    else
                    {
                        denominateur *= ppX[count] - ppX[counter];
                    }
                }
                p[count] = numerateur / denominateur;
            }
            // Somme des polynômes
            double interpolation = 0;
            for (int count = 0; count < p.Length; count++)
            {
                interpolation += ppY[count] * p[count];
            }
            return interpolation;
        }



        /*
         * METHODES
         */



        /// <summary>
        /// Returns the indexes of all reference points ordered by their distance from the specified value.
        /// </summary>
        /// <remarks>This method can be used to identify which reference points are nearest to a given
        /// value. The length of the returned array matches the number of reference points.</remarks>
        /// <param name="x">The value to compare against each reference point when calculating distances.</param>
        /// <returns>An array of indexes representing the reference points, sorted in ascending order of their distance from
        /// <paramref name="x"/>. The first element corresponds to the closest point.</returns>
        private int[] orderedIndexesByDistance(double x)
        {
            int[] pts = new int[ppX.Length];
            double[] dist = distances(x);
            bool[] taken = new bool[ppX.Length];

            // Triage
            int shortestIndex = 0;
            double shortestDistance = dist[shortestIndex];

            for (int i = 0; i < pts.Length; i++)
            {
                shortestDistance = Double.MaxValue;
                for (int j = 0; j < dist.Length; j++)
                {
                    if (!taken[j])
                    {
                        if (dist[j] < shortestDistance)
                        {
                            shortestDistance = dist[j];
                            shortestIndex = j;
                        }
                    }
                }
                pts[i] = shortestIndex;
                taken[shortestIndex] = true;
            }
            return pts;
        }
        public int[] _A_orderedIndexesByDistance(double x)
        {
            return orderedIndexesByDistance(x);
        }


        /// <summary>
        /// Construit une table des distances entre une valeur x et les abscisses des points de performance
        /// </summary>
        /// <param name="x">Valeur de l'abscisse pour laquelle on recherche les distances</param>
        /// <returns>Table de double des distances entre chaque abscisse de point de performance et x</returns>
        private double[] distances(double x)
        {
            double[] distances = new double[ppX.Length];
            for (int count = 0; count < ppX.Length; count++)
            {
                distances[count] = Math.Abs(x - ppX[count]);
            }
            return distances;
        }

    }
}